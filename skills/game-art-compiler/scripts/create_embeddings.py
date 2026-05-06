#!/usr/bin/env python3
import sys
import os
import json
import numpy as np
from pathlib import Path
from typing import Dict, List

try:
    from sentence_transformers import SentenceTransformer
except ImportError:
    print("Error: sentence-transformers is not installed. Please run: pip install sentence-transformers")
    sys.exit(1)

def generate_embeddings_and_archetypes(captions_data: List[Dict], output_path: str):
    """
    captions_data format: [{"filename": "image.png", "caption": "A space helmet..."}]
    """
    if not captions_data:
        print("Error: No captions provided.")
        sys.exit(1)

    print("Loading local Model (all-MiniLM-L6-v2)...")
    # Using a small, fast model for embeddings
    model = SentenceTransformer('all-MiniLM-L6-v2')

    texts = [item['caption'] for item in captions_data]
    print(f"Generating vectors for {len(texts)} images...")
    embeddings = model.encode(texts)

    # 1. Calculate the centroid of all vectors
    centroid = np.mean(embeddings, axis=0)

    # 2. Find distances to the centroid
    distances = np.linalg.norm(embeddings - centroid, axis=1)

    # 3. Find the lowest distances (the most "average/representative" stylistic archetypes)
    # We want max 2 archetypes
    num_archetypes = min(len(distances), 2)
    archetype_indices = np.argsort(distances)[:num_archetypes]

    # 4. Construct final output
    index_db = {}
    for i, item in enumerate(captions_data):
        filename = item['filename']
        is_archetype = bool(i in archetype_indices)
        index_db[filename] = {
            "caption": item['caption'],
            "vector": embeddings[i].tolist(),
            "is_archetype": is_archetype
        }

    # 5. Save the output
    try:
        with open(output_path, 'w', encoding='utf-8') as f:
            json.dump(index_db, f, ensure_ascii=False, indent=2)
        print(f"Successfully wrote style_index.json to {output_path}")
        
        archetype_names = [captions_data[idx]['filename'] for idx in archetype_indices]
        print(f"Designated Archetypes: {', '.join(archetype_names)}")
    except Exception as e:
        print(f"Error saving file: {str(e)}")
        sys.exit(1)

if __name__ == "__main__":
    # Expects argument: path to a temp JSON containing the captions, and the target output directory
    if len(sys.argv) < 3:
        print("Usage: python create_embeddings.py <path_to_input_captions.json> <target_style_directory_path>")
        sys.exit(1)

    input_json_path = sys.argv[1]
    target_dir = sys.argv[2]

    if not os.path.exists(input_json_path):
        print(f"Error: {input_json_path} not found.")
        sys.exit(1)

    with open(input_json_path, 'r', encoding='utf-8') as f:
        try:
            data = json.load(f)
        except json.JSONDecodeError:
            print("Error: Input file must be valid JSON.")
            sys.exit(1)

    output_file = os.path.join(target_dir, "style_index.json")
    generate_embeddings_and_archetypes(data, output_file)
