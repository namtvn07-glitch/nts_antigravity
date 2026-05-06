#!/usr/bin/env python3
import sys
import os
import json
import numpy as np

try:
    from sentence_transformers import SentenceTransformer
except ImportError:
    print("Error: sentence-transformers is not installed. Please run: pip install sentence-transformers", file=sys.stderr)
    sys.exit(1)

def cosine_similarity(vec_a, vec_b):
    a = np.array(vec_a)
    b = np.array(vec_b)
    norm_a = np.linalg.norm(a)
    norm_b = np.linalg.norm(b)
    if norm_a == 0 or norm_b == 0:
        return 0.0
    return np.dot(a, b) / (norm_a * norm_b)

def get_style_score(style_dir, prompt_vector):
    # style_dir is the full absolute path
    style_index_path = os.path.join(style_dir, "style_index.json")
    if not os.path.exists(style_index_path):
        return 0.0, ""

    best_score = 0.0
    best_caption = ""
    with open(style_index_path, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)
            for filepath, block in data.items():
                if "vector" in block:
                    score = cosine_similarity(prompt_vector, block["vector"])
                    if score > best_score:
                        best_score = score
                        best_caption = block.get("caption", "No caption")
        except:
            pass
            
    return best_score, best_caption

def main():
    if len(sys.argv) < 2:
        print("Usage: python retrieve_aso_style.py <vibe_text>", file=sys.stderr)
        sys.exit(1)
        
    vibe_text = sys.argv[1]
    
    # Load model once
    try:
        model = SentenceTransformer('all-MiniLM-L6-v2')
    except Exception as e:
        print(f"Error loading model: {e}", file=sys.stderr)
        sys.exit(1)
        
    prompt_vector = model.encode([vibe_text])[0]
    
    base_dir = os.path.abspath(os.curdir)
    style_lib_path = os.path.join(base_dir, "Assets", "GameArtist", "StyleLibrary")
    
    if not os.path.exists(style_lib_path):
        print(f"Error: {style_lib_path} does not exist.", file=sys.stderr)
        sys.exit(1)

    folder_scores = []
    
    for folder_name in os.listdir(style_lib_path):
        full_path = os.path.join(style_lib_path, folder_name)
        if os.path.isdir(full_path):
            score, caption = get_style_score(full_path, prompt_vector)
            if score > 0:
                rel_path = os.path.join("Assets", "GameArtist", "StyleLibrary", folder_name)
                # converting to forward slash for consistency in shell outputs
                rel_path = rel_path.replace("\\", "/")
                folder_scores.append((score, rel_path, caption))
                
    folder_scores.sort(key=lambda x: x[0], reverse=True)
    
    top_n = folder_scores[:3]
    
    print("=== ASO RAG STYLE RECOMMENDATIONS ===")
    if not top_n:
        print("- (No valid styles found in StyleLibrary)")
    else:
        for idx, item in enumerate(top_n):
            print(f"{idx+1}. Style Path: {item[1]} | Confidence: {item[0]:.4f}")
            print(f"   Sample Match: {item[2]}")
            print()
    print("=====================================")

if __name__ == "__main__":
    main()
