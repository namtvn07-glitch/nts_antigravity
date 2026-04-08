import os
import sys
import base64
import json
from pathlib import Path
import mimetypes

# Đảm bảo có thư viện Pillow để nén ảnh nếu cần
try:
    from PIL import Image
except ImportError:
    print("Vui lòng cài đặt thư viện Pillow: pip install Pillow")
    sys.exit(1)

MAX_SIZE_MB = 5.0
RESIZE_IF_LARGER_THAN_WIDTH = 1024

def file_to_base64_data_uri(filepath):
    mime_type, _ = mimetypes.guess_type(filepath)
    if not mime_type:
        mime_type = "application/octet-stream"
        
    with open(filepath, "rb") as f:
        encoded_string = base64.b64encode(f.read()).decode('utf-8')
        
    return f"data:{mime_type};base64,{encoded_string}"

def compress_image(filepath, output_path):
    # Optimize hình ảnh bằng PIL
    # Nếu ảnh là lossless png thì sẽ convert hoặc optimize
    try:
        with Image.open(filepath) as img:
            # Resize nếu ảnh quá lớn
            if img.width > RESIZE_IF_LARGER_THAN_WIDTH:
                ratio = RESIZE_IF_LARGER_THAN_WIDTH / float(img.width)
                new_height = int((float(img.height) * float(ratio)))
                img = img.resize((RESIZE_IF_LARGER_THAN_WIDTH, new_height), Image.Resampling.LANCZOS)
                
            format = "JPEG" if img.mode == "RGB" else "PNG"
            if format == "JPEG":
                img.save(output_path, "JPEG", optimize=True, quality=80)
            else:
                img.save(output_path, "PNG", optimize=True)
            return True
    except Exception as e:
        print(f"Không thể compress {filepath}: {e}")
        import shutil
        shutil.copy2(filepath, output_path)
        return False

def main():
    if len(sys.argv) < 2:
        print("Usage: python asset_transformer.py <đường_dẫn_đến_thư_mục_asset>")
        sys.exit(1)

    asset_dir = sys.argv[1]
    if not os.path.exists(asset_dir):
        print(f"Lỗi: Thư mục {asset_dir} không tồn tại.")
        sys.exit(1)

    assets = {}
    temp_dir = os.path.join(asset_dir, ".temp_opt")
    os.makedirs(temp_dir, exist_ok=True)
    
    total_size_bytes = 0

    print(f"Bắt đầu quét và chuyển đổi tại: {asset_dir}")
    for root, dirs, files in os.walk(asset_dir):
        # Bỏ qua folder temp
        if ".temp_opt" in root: continue
            
        for file in files:
            file_path = os.path.join(root, file)
            # Chỉ xử lý ảnh (png, jpg, webp) và audio (mp3, wav, ogg)
            ext = os.path.splitext(file)[1].lower()
            if ext not in ['.png', '.jpg', '.jpeg', '.webp', '.mp3', '.wav', '.ogg']:
                continue
                
            asset_key = file # dùng tên file có extension làm key, ví dụ "bg.png"
            
            # Compress image if applicable
            opt_path = os.path.join(temp_dir, file)
            if ext in ['.png', '.jpg', '.jpeg', '.webp']:
                print(f"Đang nén ảnh: {file}...")
                compress_image(file_path, opt_path)
                final_path = opt_path
            else:
                final_path = file_path # Audio hiện tại giữ nguyên
                
            print(f"Đang mã hóa Base64: {file}...")
            base64_str = file_to_base64_data_uri(final_path)
            total_size_bytes += len(base64_str.encode('utf-8'))
            
            assets[asset_key] = base64_str

    # Xóa temp
    import shutil
    shutil.rmtree(temp_dir)

    output_json = os.path.join(asset_dir, "assets_b64.json")
    with open(output_json, "w", encoding="utf-8") as f:
        json.dump(assets, f, indent=2)

    total_mb = total_size_bytes / (1024 * 1024)
    print(f"====================================")
    print(f"HOÀN THÀNH. Đã tạo: {output_json}")
    print(f"Tổng dung lượng Base64 ước tính: {total_mb:.2f} MB")
    if total_mb > MAX_SIZE_MB:
        print("CẢNH BÁO: Dung lượng vượt quá giới hạn 5MB! Hãy bỏ bớt tài nguyên hoặc nén mạnh hơn.")
    else:
        print("TUYỆT VỜI: Dung lượng tiêu chuẩn (< 5MB).")

if __name__ == "__main__":
    main()
