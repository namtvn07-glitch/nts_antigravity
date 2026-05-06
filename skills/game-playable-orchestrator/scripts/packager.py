import sys
import os
import base64
import re

def package_project(project_dir):
    index_path = os.path.join(project_dir, 'index.html')
    build_dir = os.path.join(project_dir, 'build')
    assets_dir = os.path.join(project_dir, 'assets')
    
    if not os.path.exists(index_path):
        print(f"Error: {index_path} not found. Are you sure Phase 4 DEV is completed and index.html is in {project_dir}?")
        return
        
    os.makedirs(build_dir, exist_ok=True)
    build_path = os.path.join(build_dir, 'index.html')

    with open(index_path, 'r', encoding='utf-8') as f:
        html_content = f.read()

    # Find ASSET_B64 definition in the script and inject the dictionary contents
    b64_dict_entries = []

    if os.path.exists(assets_dir):
        for filename in os.listdir(assets_dir):
            if filename.lower().endswith('.webp'):
                file_path = os.path.join(assets_dir, filename)
                key = os.path.splitext(filename)[0]
                with open(file_path, 'rb') as img_file:
                    b64_data = base64.b64encode(img_file.read()).decode('utf-8')
                    b64_string = f"data:image/webp;base64,{b64_data}"
                    b64_dict_entries.append(f"'{key}': '{b64_string}'")
                    print(f"Base64 generated for: {key}")

    b64_injection = ",\n        ".join(b64_dict_entries)
    
    # Replace the empty ASSET_B64 object with our populated dictionary
    html_content = re.sub(
        r'var\s+ASSET_B64\s*=\s*\{[^\}]*\};',
        f'var ASSET_B64 = {{\n        {b64_injection}\n    }};',
        html_content
    )

    with open(build_path, 'w', encoding='utf-8') as f:
        f.write(html_content)
        
    print(f"\n[SUCCESS] Packaging complete. Monolithic Ad File saved to: {build_path}")
    print(f"Final Payload Size: {os.path.getsize(build_path) / (1024*1024):.2f} MB")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        package_project(sys.argv[1])
    else:
        print("Usage: python packager.py <project_directory>")
