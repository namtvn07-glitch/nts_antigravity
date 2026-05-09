import os
import re
import json
import argparse
import networkx as nx
from pathlib import Path
import graph_utils
import networkx.algorithms.community as nx_comm


def get_markdown_files(workspace_root):
    """Scan the rules, learned, and knowledge directories for markdown files."""
    agents_dir = Path(workspace_root) / ".agents"
    target_dirs = [
        agents_dir / "rules",
        agents_dir / "learned",
        agents_dir / "knowledge",
    ]

    md_files = []
    for d in target_dirs:
        if d.exists():
            for filepath in d.rglob("*.md"):
                md_files.append(filepath)
    return md_files


def extract_entities_and_relations(text, source_file=""):
    """
    Rule-based entity extraction from Markdown. Zero API cost.
    - H1/H2/H3 headings    → Category nodes
    - **[Tag] Label**       → Tagged nodes (Pattern, Gotcha, Rule, etc.)
    - `ClassName`           → Technical concept nodes
    Relations: headings contain sub-headings/rules; headings reference backtick terms.
    """
    nodes = []
    edges = []
    seen_ids = set()

    def add_node(node_id, node_type, label):
        if node_id not in seen_ids:
            seen_ids.add(node_id)
            nodes.append({"id": node_id, "type": node_type, "label": label})

    heading_stack = []  # tracks (level, node_id) for parent linking

    for raw_line in text.split("\n"):
        line = raw_line.strip()
        if not line:
            continue

        # --- Headings → Category nodes ---
        h_match = re.match(r"^(#{1,3})\s+(.+)", line)
        if h_match:
            level = len(h_match.group(1))
            title = re.sub(r"[*`#]", "", h_match.group(2)).strip()
            if not title:
                continue
            node_id = f"{source_file}::{title}"
            add_node(node_id, f"H{level}_Category", title)

            # Link to nearest ancestor heading of lower level
            while heading_stack and heading_stack[-1][0] >= level:
                heading_stack.pop()
            if heading_stack:
                parent_id = heading_stack[-1][1]
                edges.append({"source": parent_id, "target": node_id, "relation": "contains"})

            heading_stack.append((level, node_id))
            continue

        # --- Tagged bullet items: **[Pattern] Label** or **[Gotcha] Label** ---
        tag_match = re.match(r"^[-*]\s+\*\*\[(\w+)\]\s*(.+?)\*\*", line)
        if tag_match:
            tag_type = tag_match.group(1).capitalize()
            label = re.sub(r"[*`]", "", tag_match.group(2)).strip().rstrip(":")
            if not label:
                continue
            node_id = f"{source_file}::{label}"
            add_node(node_id, tag_type, label)
            if heading_stack:
                edges.append({"source": heading_stack[-1][1], "target": node_id, "relation": "defines"})
            continue

        # --- Backtick technical terms (CamelCase or dotted, ignore short/lowercase) ---
        for tech in re.findall(r"`([A-Z][A-Za-z0-9_.]{2,})`", line):
            tech_id = f"tech::{tech}"
            add_node(tech_id, "Technical", tech)
            if heading_stack:
                edges.append({"source": heading_stack[-1][1], "target": tech_id, "relation": "references"})

    return {"nodes": nodes, "edges": edges}


def summarize_community(community_nodes, graph):
    """
    Extractive community summary — no LLM required.
    Concatenates node labels grouped by type.
    """
    by_type = {}
    for node_id in community_nodes:
        data = graph.nodes.get(node_id, {})
        ntype = data.get("type", "Unknown")
        label = data.get("label", node_id.split("::")[-1])
        by_type.setdefault(ntype, []).append(label)

    parts = []
    for ntype, labels in sorted(by_type.items()):
        parts.append(f"{ntype}: {', '.join(labels[:5])}")

    return (
        f"Community of {len(community_nodes)} concepts. "
        + " | ".join(parts)
    )


def detect_and_summarize_communities(graph):
    if len(graph.nodes) == 0:
        return []

    communities = nx_comm.louvain_communities(graph, seed=42)
    summaries = []

    for i, community in enumerate(communities):
        summary_text = summarize_community(community, graph)
        summaries.append({
            "community_id": i,
            "nodes": list(community),
            "summary": summary_text,
        })
        for node in community:
            graph.nodes[node]["community"] = i

    return summaries


def build_graph(workspace_root):
    md_files = get_markdown_files(workspace_root)
    G = nx.Graph()

    for filepath in md_files:
        with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
            content = f.read()

        extracted_data = extract_entities_and_relations(content, source_file=filepath.name)

        for node in extracted_data.get("nodes", []):
            G.add_node(
                node["id"],
                type=node.get("type", "Unknown"),
                label=node.get("label", node["id"]),
                source_file=filepath.name,
            )

        for edge in extracted_data.get("edges", []):
            src, tgt = edge["source"], edge["target"]
            if G.has_node(src) and G.has_node(tgt):
                G.add_edge(src, tgt, relation=edge.get("relation", "related_to"))

    return G


def run_query(query, community_summaries, index_path, model):
    """Semantic search over FAISS index and print top results."""
    try:
        import faiss
        import numpy as np

        index = faiss.read_index(index_path)
        query_vec = model.encode([query]).astype("float32")
        distances, indices = index.search(query_vec, 5)

        print("\n=== QUERY RESULTS ===")
        for rank, idx in enumerate(indices[0]):
            if 0 <= idx < len(community_summaries):
                dist = distances[0][rank]
                print(f"[{rank + 1}] (dist={dist:.3f}) {community_summaries[idx]['summary']}")
        print("====================\n")
    except Exception as e:
        print(f"Query failed: {e}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Build GraphRAG knowledge index.")
    parser.add_argument(
        "--query", type=str, default=None,
        help="After building, run a semantic search query and print results."
    )
    args = parser.parse_args()

    workspace_root = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
    graph = build_graph(workspace_root)

    # Community Detection & Summarization
    community_summaries = detect_and_summarize_communities(graph)

    graph_data_dir = os.path.join(workspace_root, ".agents", "graph_data")
    os.makedirs(graph_data_dir, exist_ok=True)

    out_path = os.path.join(graph_data_dir, "knowledge_graph.json")
    graph_utils.save_graph(graph, out_path)

    html_path = os.path.join(graph_data_dir, "interactive_mindmap.html")
    graph_utils.generate_interactive_html(graph, html_path)

    summaries_path = os.path.join(graph_data_dir, "community_summaries.json")
    with open(summaries_path, "w", encoding="utf-8") as f:
        json.dump(community_summaries, f, indent=4, ensure_ascii=False)

    print(f"Graph built with {graph.number_of_nodes()} nodes and {graph.number_of_edges()} edges.")
    print(f"Detected {len(community_summaries)} communities.")
    print(f"Saved graph to {out_path}")
    print(f"Saved summaries to {summaries_path}")
    print(f"Saved mindmap to {html_path}")

    # Vector Indexing
    print("\nBuilding Vector Index...")
    model = None
    index_path = os.path.join(graph_data_dir, "vector_index.faiss")

    try:
        from sentence_transformers import SentenceTransformer
        import faiss
        import numpy as np

        model = SentenceTransformer("all-MiniLM-L6-v2")
        print("Using SentenceTransformer (all-MiniLM-L6-v2)")

    except ImportError:
        print("sentence-transformers or faiss not installed. Trying TF-IDF fallback...")
        try:
            from sklearn.feature_extraction.text import TfidfVectorizer
            import faiss
            import numpy as np

            class TFIDFModel:
                def __init__(self, texts):
                    self.vec = TfidfVectorizer(max_features=384)
                    self.vec.fit(texts)

                def encode(self, texts):
                    return self.vec.transform(texts).toarray().astype("float32")

            all_texts = [s["summary"] for s in community_summaries] or ["placeholder"]
            model = TFIDFModel(all_texts)
            print("Using TF-IDF fallback")
        except ImportError:
            print("No embedding library available. Skipping vector indexing.")

    if model and community_summaries:
        try:
            texts_to_embed = [s["summary"] for s in community_summaries]
            embeddings = model.encode(texts_to_embed)

            dimension = embeddings.shape[1]
            index = faiss.IndexFlatL2(dimension)
            index.add(embeddings)

            faiss.write_index(index, index_path)
            print(f"Saved FAISS index to {index_path} ({len(texts_to_embed)} vectors, dim={dimension})")
        except Exception as e:
            print(f"Vector indexing failed: {e}")
    elif not community_summaries:
        print("No communities to embed.")

    # Optional semantic query
    if args.query and model:
        run_query(args.query, community_summaries, index_path, model)
