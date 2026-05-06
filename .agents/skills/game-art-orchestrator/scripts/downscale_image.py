#!/usr/bin/env python3
import sys
import os
from PIL import Image

def downscale_image(image_path, max_size=512):
    """
    Downscales an image so its longest edge is max_size, maintaining aspect ratio.
    Saves the result as a new file appended with '_downscaled'.
    """
    if not os.path.exists(image_path):
        print(f"Error: {image_path} not found.")
        sys.exit(1)

    try:
        with Image.open(image_path) as img:
            # Calculate new dimensions
            ratio = max_size / max(img.size)
            if ratio >= 1.0:
                print(f"Image '{image_path}' is already smaller than {max_size}px. Skipping.")
                return image_path
                
            new_size = (int(img.size[0] * ratio), int(img.size[1] * ratio))
            img_resized = img.resize(new_size, Image.Resampling.LANCZOS)
            
            # Save new file
            name, ext = os.path.splitext(image_path)
            new_path = f"{name}_downscaled{ext}"
            img_resized.save(new_path)
            print(f"Successfully downscaled '{image_path}' to {new_size}. Saved as '{new_path}'")
            return new_path
    except Exception as e:
        print(f"Failed to process image: {e}")
        sys.exit(1)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python downscale_image.py <image_path_1> <image_path_2> ...")
        sys.exit(1)
        
    for path in sys.argv[1:]:
        downscale_image(path)
