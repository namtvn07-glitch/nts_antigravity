import json
import os
import networkx as nx

def load_graph(filepath):
    if os.path.exists(filepath):
        with open(filepath, 'r', encoding='utf-8') as f:
            data = json.load(f)
            return nx.node_link_graph(data)
    return nx.Graph()

def save_graph(graph, filepath):
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    data = nx.node_link_data(graph)
    with open(filepath, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=4)

def generate_interactive_html(graph, filepath):
    data = nx.node_link_data(graph)
    nodes_for_vis = []
    edges_for_vis = []
    
    for node in data.get('nodes', []):
        community = node.get('community', 0)
        color = f"hsl({(community * 137.5) % 360}, 70%, 50%)"
        nodes_for_vis.append({
            "id": node['id'],
            "label": str(node['id']),
            "title": f"Type: {node.get('type', 'Unknown')}<br>Source: {node.get('source_file', 'Unknown')}<br>Community: {community}",
            "color": color
        })
        
    for edge in data.get('links', []):
        edges_for_vis.append({
            "from": edge['source'],
            "to": edge['target'],
            "label": edge.get('relation', ''),
            "arrows": "to"
        })
        
    html_content = f"""<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Knowledge Graph Mindmap</title>
    <script type="text/javascript" src="https://unpkg.com/vis-network/standalone/umd/vis-network.min.js"></script>
    <style type="text/css">
        body {{ margin: 0; padding: 0; background-color: #1e1e1e; color: #fff; font-family: sans-serif; }}
        #mynetwork {{ width: 100vw; height: 100vh; border: none; }}
        #info {{ position: absolute; top: 10px; left: 10px; background: rgba(0,0,0,0.7); padding: 10px; border-radius: 5px; z-index: 10; font-size: 14px; }}
    </style>
</head>
<body>
    <div id="info">GraphRAG Visualizer - Scroll to zoom, Drag to pan</div>
    <div id="mynetwork"></div>

    <script type="text/javascript">
        var nodes = new vis.DataSet({json.dumps(nodes_for_vis)});
        var edges = new vis.DataSet({json.dumps(edges_for_vis)});

        var container = document.getElementById('mynetwork');
        var data = {{
            nodes: nodes,
            edges: edges
        }};
        var options = {{
            nodes: {{
                shape: 'dot',
                size: 20,
                font: {{ color: '#ffffff', size: 14 }}
            }},
            edges: {{
                font: {{ color: '#aaaaaa', size: 10, align: 'middle' }},
                color: '#666666',
                smooth: {{ type: 'continuous' }}
            }},
            physics: {{
                forceAtlas2Based: {{
                    gravitationalConstant: -50,
                    centralGravity: 0.01,
                    springLength: 100,
                    springConstant: 0.08
                }},
                maxVelocity: 50,
                solver: 'forceAtlas2Based',
                timestep: 0.35,
                stabilization: {{ iterations: 150 }}
            }}
        }};
        var network = new vis.Network(container, data, options);
    </script>
</body>
</html>
"""
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(html_content)
