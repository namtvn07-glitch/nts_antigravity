import argparse
from PIL import Image
import os

def resize_and_crop(image_path, target_width, target_height, output_path):
    try:
        img = Image.open(image_path).convert("RGBA")
        img_w, img_h = img.size
        
        target_ratio = target_width / target_height
        img_ratio = img_w / img_h
        
        if target_ratio > img_ratio:
            # Target is wider than original. Resize to match width, then crop height
            new_w = target_width
            new_h = int(target_width / img_ratio)
            img = img.resize((new_w, new_h), Image.Resampling.LANCZOS)
            
            # Crop height (center crop)
            top = (new_h - target_height) // 2
            bottom = top + target_height
            img = img.crop((0, top, target_width, bottom))
        else:
            # Target is taller than original. Resize to match height, then crop width
            new_h = target_height
            new_w = int(target_height * img_ratio)
            img = img.resize((new_w, new_h), Image.Resampling.LANCZOS)
            
            # Crop width (center crop)
            left = (new_w - target_width) // 2
            right = left + target_width
            img = img.crop((left, 0, right, target_height))
            
        final_img = img.convert("RGB")
        final_img.save(output_path)
        print(f"Center cropped and resized {image_path} to {target_width}x{target_height} -> {output_path}")
        
    except Exception as e:
        print(f"Error resizing {image_path}: {e}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--image", required=True)
    parser.add_argument("--width", type=int, required=True)
    parser.add_argument("--height", type=int, required=True)
    parser.add_argument("--out", required=True)
    
    args = parser.parse_args()
    resize_and_crop(args.image, args.width, args.height, args.out)
