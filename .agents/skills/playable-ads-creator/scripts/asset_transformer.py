import os
import sys
import base64
import json
from pathlib import Path
import mimetypes

try:
    from PIL import Image
except ImportError:
    print("Vui lòng cài đặt thư viện Pillow: pip install Pillow")
    sys.exit(1)

# --- CẤU HÌNH ---
MAX_TOTAL_BASE64_MB = 3.5  # Budget cho assets (dành ~1.5MB cho code)
RESIZE_PASS_1 = 1024       # Lần 1: max 1024px
RESIZE_PASS_2 = 512        # Lần 2 (nếu vượt budget): max 512px
RESIZE_PASS_3 = 256        # Lần 3 (nếu vẫn vượt): max 256px
QUALITY_PASS_1 = 80
QUALITY_PASS_2 = 60
QUALITY_PASS_3 = 40
SUPPORTED_IMAGE_EXT = ['.png', '.jpg', '.jpeg', '.webp', '.gif']
SUPPORTED_AUDIO_EXT = ['.mp3', '.wav', '.ogg']
SUPPORTED_EXT = SUPPORTED_IMAGE_EXT + SUPPORTED_AUDIO_EXT


def file_to_base64_data_uri(filepath):
    """Convert file thành Data URI Base64."""
    mime_type, _ = mimetypes.guess_type(filepath)
    if not mime_type:
        mime_type = "application/octet-stream"
    with open(filepath, "rb") as f:
        encoded_string = base64.b64encode(f.read()).decode('utf-8')
    return f"data:{mime_type};base64,{encoded_string}"


def process_image(filepath, output_path, max_width, quality):
    """Resize + nén ảnh. Luôn xuất WebP nếu có alpha, JPEG nếu không."""
    try:
        with Image.open(filepath) as img:
            # Resize nếu cần
            if img.width > max_width:
                ratio = max_width / float(img.width)
                new_height = int(float(img.height) * ratio)
                img = img.resize((max_width, new_height), Image.Resampling.LANCZOS)

            # Chọn format output tối ưu
            has_alpha = img.mode in ('RGBA', 'LA', 'PA')
            if has_alpha:
                # WebP hỗ trợ alpha tốt hơn PNG và nhẹ hơn nhiều
                out = output_path.rsplit('.', 1)[0] + '.webp'
                img.save(out, 'WEBP', quality=quality, method=6)
            else:
                img = img.convert('RGB')
                out = output_path.rsplit('.', 1)[0] + '.webp'
                img.save(out, 'WEBP', quality=quality, method=6)
            return out
    except Exception as e:
        print(f"  ⚠️ Không thể xử lý {filepath}: {e}")
        import shutil
        shutil.copy2(filepath, output_path)
        return output_path


def scan_files(asset_dir):
    """Quét tất cả file hỗ trợ trong thư mục (không đệ quy vào .temp_opt)."""
    files = []
    for root, dirs, filenames in os.walk(asset_dir):
        # Bỏ qua thư mục temp
        dirs[:] = [d for d in dirs if d != '.temp_opt']
        for f in filenames:
            ext = os.path.splitext(f)[1].lower()
            if ext in SUPPORTED_EXT:
                files.append(os.path.join(root, f))
    return files


def run_pass(files, asset_dir, max_width, quality, pass_num):
    """Chạy 1 vòng nén, trả về dict assets và tổng size."""
    temp_dir = os.path.join(asset_dir, ".temp_opt")
    os.makedirs(temp_dir, exist_ok=True)

    assets = {}
    total_size = 0
    print(f"\n--- Pass {pass_num}: max_width={max_width}px, quality={quality}% ---")

    for filepath in files:
        filename = os.path.basename(filepath)
        ext = os.path.splitext(filename)[1].lower()

        if ext in SUPPORTED_IMAGE_EXT:
            print(f"  🖼 Đang xử lý ảnh: {filename}")
            opt_path = os.path.join(temp_dir, filename)
            final_path = process_image(filepath, opt_path, max_width, quality)
            # Key dùng tên file webp
            asset_key = os.path.basename(final_path)
        else:
            print(f"  🔊 Đang xử lý audio: {filename}")
            final_path = filepath
            asset_key = filename

        data_uri = file_to_base64_data_uri(final_path)
        size = len(data_uri.encode('utf-8'))
        total_size += size
        assets[asset_key] = data_uri
        print(f"    → {asset_key}: {size / 1024:.1f} KB")

    # Cleanup temp
    import shutil
    if os.path.exists(temp_dir):
        shutil.rmtree(temp_dir)

    return assets, total_size


def main():
    if len(sys.argv) < 2:
        print("Usage: python asset_transformer.py <đường_dẫn_thư_mục_asset>")
        print("  Tùy chọn: python asset_transformer.py <path> --budget <MB>")
        sys.exit(1)

    asset_dir = sys.argv[1]
    if not os.path.exists(asset_dir):
        print(f"❌ Lỗi: Thư mục '{asset_dir}' không tồn tại.")
        sys.exit(1)

    # Đọc budget tùy chọn
    budget_mb = MAX_TOTAL_BASE64_MB
    if '--budget' in sys.argv:
        idx = sys.argv.index('--budget')
        if idx + 1 < len(sys.argv):
            budget_mb = float(sys.argv[idx + 1])

    files = scan_files(asset_dir)
    if not files:
        print(f"⚠️ Không tìm thấy file ảnh/audio nào trong {asset_dir}")
        sys.exit(1)

    print(f"📁 Tìm thấy {len(files)} file(s) trong {asset_dir}")
    print(f"💰 Budget: {budget_mb:.1f} MB")

    # Multi-pass compression
    passes = [
        (RESIZE_PASS_1, QUALITY_PASS_1),
        (RESIZE_PASS_2, QUALITY_PASS_2),
        (RESIZE_PASS_3, QUALITY_PASS_3),
    ]

    for i, (max_w, qual) in enumerate(passes, 1):
        assets, total_size = run_pass(files, asset_dir, max_w, qual, i)
        total_mb = total_size / (1024 * 1024)

        print(f"\n📊 Pass {i} kết quả: {total_mb:.2f} MB / {budget_mb:.1f} MB budget")

        if total_mb <= budget_mb:
            print(f"✅ Nằm trong budget!")
            break
        elif i < len(passes):
            print(f"⚠️ Vượt budget, thử pass tiếp theo với nén mạnh hơn...")
        else:
            print(f"❌ CẢNH BÁO: Vẫn vượt budget sau {len(passes)} pass!")
            print(f"   Đề xuất: Bỏ bớt assets hoặc giảm kích thước ảnh thủ công.")

    # Xuất JSON
    output_json = os.path.join(asset_dir, "assets_b64.json")
    with open(output_json, "w", encoding="utf-8") as f:
        json.dump(assets, f, indent=2)

    # Xuất JS (alternative format)
    output_js = os.path.join(asset_dir, "assets_b64.js")
    with open(output_js, "w", encoding="utf-8") as f:
        f.write("const GAME_ASSETS = ")
        json.dump(assets, f, indent=2)
        f.write(";\n")

    total_mb = total_size / (1024 * 1024)
    print(f"\n{'='*50}")
    print(f"✅ HOÀN THÀNH!")
    print(f"📄 JSON: {output_json}")
    print(f"📄 JS:   {output_js}")
    print(f"📦 Tổng dung lượng Base64: {total_mb:.2f} MB")
    print(f"📦 Budget còn lại: {max(0, budget_mb - total_mb):.2f} MB")
    print(f"🖼 Số lượng assets: {len(assets)}")
    print(f"{'='*50}")

    # Trả về tóm tắt dạng JSON để AI đọc được
    summary = {
        "status": "OK" if total_mb <= budget_mb else "OVER_BUDGET",
        "total_mb": round(total_mb, 2),
        "budget_mb": budget_mb,
        "asset_count": len(assets),
        "assets": list(assets.keys()),
        "output_json": output_json,
        "output_js": output_js
    }
    summary_path = os.path.join(asset_dir, "transform_summary.json")
    with open(summary_path, "w", encoding="utf-8") as f:
        json.dump(summary, f, indent=2)


if __name__ == "__main__":
    main()
