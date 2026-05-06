import sys
import os
from PIL import Image, ImageDraw
from rembg import remove


def optimize_images(directory):
    if not os.path.exists(directory):
        print(f"Directory {directory} does not exist.")
        return

    for filename in os.listdir(directory):
        if filename.lower().endswith(('.png', '.jpg', '.jpeg', '.webp')):
            file_path = os.path.join(directory, filename)
            try:
                img = Image.open(file_path).convert("RGBA")
                
                # Auto remove background via AI rembg
                if "bg." not in filename.lower():
                    img = remove(img)
                
                # Auto crop transparent background
                bbox = img.getbbox()
                if bbox:
                    img = img.crop(bbox)
                
                # Resize if too large to conserve < 3MB budget for playables
                max_size = 512
                if img.width > max_size or img.height > max_size:
                    img.thumbnail((max_size, max_size), Image.Resampling.LANCZOS)
                
                # Save as WebP
                webp_path = os.path.splitext(file_path)[0] + '.webp'
                img.save(webp_path, 'webp', quality=85)
                print(f"Optimized to WebP: {webp_path}")
                
                # Remove original
                img.close()
                if file_path != webp_path:
                    os.remove(file_path)
            except Exception as e:
                print(f"Error processing {filename}: {e}")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        optimize_images(sys.argv[1])
    else:
        print("Usage: python image_optimizer.py <directory_path>")
