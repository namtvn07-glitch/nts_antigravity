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

def retrieve_context(style_dir, prompt):
    # Determine base directory assumes running from workspace root
    base_dir = os.path.abspath(os.curdir)
    
    global_index_path = os.path.join(base_dir, "Assets", "GameArtist", "global_index.json")
    style_index_path = os.path.join(base_dir, style_dir, "style_index.json")
    
    # 1. Load data
    global_data = {}
    if os.path.exists(global_index_path):
        with open(global_index_path, "r", encoding="utf-8") as f:
            global_data = json.load(f)
            
    style_data = {}
    if os.path.exists(style_index_path):
        with open(style_index_path, "r", encoding="utf-8") as f:
            style_data = json.load(f)
            
    if not global_data and not style_data:
        print(f"Error: Could not load any indexes. Looked for {global_index_path} and {style_index_path}.", file=sys.stderr)
        sys.exit(1)

    # 2. Encode Prompt
    try:
        model = SentenceTransformer('all-MiniLM-L6-v2')
    except Exception as e:
        print(f"Error loading model: {e}", file=sys.stderr)
        sys.exit(1)
        
    prompt_vector = model.encode([prompt])[0]
    
    # 3. Retrieve Global Rules
    global_scores = []
    if isinstance(global_data, list):
        for block in global_data:
            vec_key = "embedding" if "embedding" in block else "vector"
            if vec_key in block and "text" in block:
                score = cosine_similarity(prompt_vector, block[vec_key])
                global_scores.append((score, block["text"]))
    elif isinstance(global_data, dict):
        for heading, block in global_data.items():
            vec_key = "embedding" if "embedding" in block else "vector"
            if vec_key in block and "text" in block:
                score = cosine_similarity(prompt_vector, block[vec_key])
                global_scores.append((score, block["text"]))
            
    global_scores.sort(key=lambda x: x[0], reverse=True)
    top_global_rules = [item[1] for item in global_scores[:3]]  # Extract top 3 rules

    # 4. Retrieve Few-Shot Images
    image_scores = []
    fallback_archetypes = []
    if isinstance(style_data, dict):
        style_list = [{"filename": k, **v} for k, v in style_data.items()]
    else:
        style_list = style_data
        
    for block in style_list:
        filepath = block.get("filename", "")
        if "is_archetype" in block and block["is_archetype"]:
            abs_img_path = os.path.abspath(os.path.join(base_dir, style_dir, filepath)) if not os.path.isabs(filepath) else filepath
            fallback_archetypes.append(abs_img_path)
            
        vec_key = "embedding" if "embedding" in block else "vector"
        if vec_key in block:
            score = cosine_similarity(prompt_vector, block[vec_key])
            abs_img_path = os.path.abspath(os.path.join(base_dir, style_dir, filepath)) if not os.path.isabs(filepath) else filepath
            image_scores.append((score, abs_img_path))

    image_scores.sort(key=lambda x: x[0], reverse=True)
    
    # Filter >= 0.60
    valid_images = [item[1] for item in image_scores if item[0] >= 0.60]
    
    # Output images: max 3
    if len(valid_images) > 0:
        out_images = valid_images[:3]
    else:
        out_images = fallback_archetypes[:3]

    # 5. Output Payload Markdown exactly formatted
    print("=== ORCHESTRATOR CONTEXT PAYLOAD ===")
    print("[GLOBAL RULES RAG]")
    if top_global_rules:
        for rule in top_global_rules:
            # remove line breaks so it looks unified or print it as-is
            print(rule.strip())
            print()
    else:
        print("- (No Global Rules found)")
        print()

    print("[FEW-SHOT REFERENCES]")
    if out_images:
        for img in out_images:
            print(img)
    else:
        print("(No suitable reference images found)")
        
    print("====================================")


if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python retrieve_orchestrator_context.py <style_directory> <prompt>", file=sys.stderr)
        sys.exit(1)
        
    target_dir = sys.argv[1]
    user_prompt = sys.argv[2]
    
    retrieve_context(target_dir, user_prompt)
