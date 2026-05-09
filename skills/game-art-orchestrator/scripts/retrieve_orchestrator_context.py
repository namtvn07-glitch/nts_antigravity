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
    
    # 1. Load data
    graph_data_dir = os.path.join(base_dir, ".agents", "graph_data")
    summaries_path = os.path.join(graph_data_dir, "community_summaries.json")
    vector_index_path = os.path.join(graph_data_dir, "vector_index.faiss")
    style_index_path = os.path.join(base_dir, style_dir, "style_index.json")
    
    community_summaries = []
    if os.path.exists(summaries_path):
        with open(summaries_path, "r", encoding="utf-8") as f:
            community_summaries = json.load(f)
            
    style_data = {}
    if os.path.exists(style_index_path):
        with open(style_index_path, "r", encoding="utf-8") as f:
            style_data = json.load(f)

    # 2. Encode Prompt
    try:
        model = SentenceTransformer('all-MiniLM-L6-v2')
    except Exception as e:
        print(f"Error loading model: {e}", file=sys.stderr)
        sys.exit(1)
        
    prompt_vector = model.encode([prompt]).astype('float32')
    
    # 3. Retrieve Global Rules (GraphRAG Community Summaries via FAISS)
    top_global_rules = []
    if os.path.exists(vector_index_path) and community_summaries:
        try:
            import faiss
            index = faiss.read_index(vector_index_path)
            distances, indices = index.search(prompt_vector, 3)
            for idx in indices[0]:
                if 0 <= idx < len(community_summaries):
                    top_global_rules.append(community_summaries[idx]["summary"])
        except Exception as e:
            print(f"FAISS search error: {e}", file=sys.stderr)

    # Text-based fallback when FAISS unavailable or returns nothing
    if not top_global_rules and community_summaries:
        print("[INFO] Using text fallback: returning top community summaries.", file=sys.stderr)
        top_global_rules = [s["summary"] for s in community_summaries[:3]]

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
            score = cosine_similarity(prompt_vector[0], block[vec_key])
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
