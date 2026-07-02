import os
import re
import html

# Directories to exclude from the visual tree map and architectural scan
EXCLUDED_DIRS = {'.git', '.github', 'bin', 'obj', '.vs', 'packages', 'TestResults'}
EXCLUDED_FILES = {'.DS_Store', 'Smart-Factory-Management-System.sln.DotSettings.user'}

def build_project_tree(root_dir):
    """Generates a nested dictionary representation of the repository folder structure."""
    tree = {}
    for root, dirs, files in os.walk(root_dir):
        # Filter directories in-place to avoid traversing deep into build folders
        dirs[:] = [d for d in dirs if d not in EXCLUDED_DIRS]
        
        # Calculate relative path components
        rel_path = os.path.relpath(root, root_dir)
        path_parts = [] if rel_path == '.' else rel_path.split(os.sep)
        
        current_node = tree
        for part in path_parts:
            current_node = current_node.setdefault(part, {})
            
        for file in files:
            if file not in EXCLUDED_FILES:
                current_node[file] = None
    return tree

def generate_tree_html(node, name="", depth=0):
    """Recursively converts the project tree dictionary into an interactive HTML nested list."""
    indent = "    " * depth
    html_out = ""
    
    if isinstance(node, dict):
        if name:
            html_out += f'{indent}<li class="folder-node"><span class="folder-toggle">📁 {html.escape(name)}/</span>\n{indent}  <ul>\n'
        # Sort folders first, then files
        sorted_keys = sorted(node.keys(), key=lambda k: (node[k] is None, k.lower()))
        for key in sorted_keys:
            html_out += generate_tree_html(node[key], key, depth + 1)
        if name:
            html_out += f'{indent}  </ul>\n{indent}</li>\n'
    else:
        # It's a file element; link it via hash anchor to its detail card below
        anchor = html.escape(name.replace('.', '_'))
        ext_class = "file-cs" if name.endswith('.cs') else "file-generic"
        html_out += f'{indent}<li class="file-node"><a href="#{anchor}" class="{ext_class}">📄 {html.escape(name)}</a></li>\n'
        
    return html_out

def parse_cs_file_details(file_path):
    """Parses a C# source file to extract high-level metadata (Namespace, Class/Enum, Methods, Properties)."""
    details = {
        'type': 'Unknown Component',
        'namespace': 'Global Namespace',
        'name': os.path.basename(file_path),
        'elements': []
    }
    
    if not os.path.exists(file_path):
        return details

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        # Extract namespace context boundary
        ns_match = re.search(r'namespace\s+([\w\.]+)', content)
        if ns_match:
            details['namespace'] = ns_match.group(1)
            
        # Determine component blueprint nature
        if 'abstract class' in content:
            details['type'] = 'Abstract Base Class 🏛️'
        elif 'interface ' in content:
            details['type'] = 'System Interface 🔌'
        elif 'enum ' in content:
            details['type'] = 'Enumeration Context 🔢'
        elif 'class ' in content:
            details['type'] = 'Concrete Logic Class ⚙️'
            
        # Extract properties and methods signatures
        lines = content.split('\n')
        for line in lines:
            line = line.strip()
            # Catch class/struct names to validate architecture
            if re.match(r'^(public|internal|private|protected)\s+(abstract\s+|static\s+)?(class|enum|struct|interface)\s+(\w+)', line):
                continue
            
            # Identify high-level properties or backing tracking vectors
            if re.match(r'^(public|internal)\s+[\w\[\]<>?]+\s+\w+\s*\{\s*get;', line):
                details['elements'].append({'kind': 'Property', 'signature': line})
            # Identify structural method boundaries
            elif re.match(r'^(public|internal|protected\s+override|public\s+override)\s+[\w\[\]<>?]+\s+\w+\s*\(.*\)', line):
                # Clean tailing operational syntax modifications
                clean_sig = line.split('{')[0].strip().replace(';', '')
                details['elements'].append({'kind': 'Method', 'signature': clean_sig})
    except Exception as e:
        details['elements'].append({'kind': 'Error', 'signature': f"Could not extract metadata: {str(e)}"})
        
    return details

def build_documentation_dashboard(root_dir, output_html_file):
    """Orchestrates filesystem mappings and parses files to compile a unified interactive HTML file."""
    project_tree = build_project_tree(root_dir)
    tree_interface_html = generate_tree_html(project_tree)
    
    file_details_html = ""
    
    # Track all parsed source components systematically
    for root, dirs, files in os.walk(root_dir):
        dirs[:] = [d for d in dirs if d not in EXCLUDED_DIRS]
        for file in sorted(files):
            if file in EXCLUDED_FILES:
                continue
            
            file_path = os.path.join(root, file)
            rel_file_path = os.path.relpath(file_path, root_dir)
            anchor_id = file.replace('.', '_')
            
            file_details_html += f'<div class="card" id="{anchor_id}">\n'
            file_details_html += f'  <div class="card-header"><h3>{html.escape(file)}</h3><span class="path-badge">{html.escape(rel_file_path)}</span></div>\n'
            
            if file.endswith('.cs'):
                meta = parse_cs_file_details(file_path)
                file_details_html += f'  <div class="meta-row"><strong>Type:</strong> <span class="type-highlight">{html.escape(meta["type"])}</span> | <strong>Namespace:</strong> <code>{html.escape(meta["namespace"])}</code></div>\n'
                
                if meta['elements']:
                    file_details_html += '  <h4>Architectural Structural Elements:</h4>\n  <ul class="element-list">\n'
                    for elem in meta['elements']:
                        badge_color = "badge-prop" if elem['kind'] == 'Property' else "badge-method"
                        file_details_html += f'    <li><span class="badge {badge_color}">{elem["kind"]}</span> <code>{html.escape(elem["signature"])}</code></li>\n'
                    file_details_html += '  </ul>\n'
                else:
                    file_details_html += '  <p class="no-elements">No public OOP properties or method matrices exposed in this file layer.</p>\n'
            elif file.endswith('.md'):
                file_details_html += '  <div class="meta-row"><strong>Type:</strong> Documentation Base Resource File 📄</div>\n'
                file_details_html += '  <p class="no-elements">Markdown technical documentation file containing structural context guidelines.</p>\n'
            else:
                file_details_html += '  <div class="meta-row"><strong>Type:</strong> Repository Infrastructure File 🛠️</div>\n'
                file_details_html += '  <p class="no-elements">Configuration file or configuration framework block.</p>\n'
                
            file_details_html += '</div>\n\n'

    # Master elegant dark HTML template execution block
    html_template = f"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Smart Factory Management System - Repository Map</title>
    <style>
        :root {{
            --bg-main: #0d1117;
            --bg-surface: #161b22;
            --bg-card: #21262d;
            --border-color: #30363d;
            --text-main: #c9d1d9;
            --text-muted: #8b949e;
            --accent-blue: #58a6ff;
            --accent-green: #7ee787;
            --accent-orange: #ffa657;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif;
            background-color: var(--bg-main);
            color: var(--text-main);
            margin: 0;
            padding: 0;
            line-height: 1.6;
        }}
        header {{
            background-color: var(--bg-surface);
            border-bottom: 1px solid var(--border-color);
            padding: 20px 40px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}
        header h1 {{ margin: 0; font-size: 24px; color: #f0f6fc; }}
        header p {{ margin: 5px 0 0 0; color: var(--text-muted); font-size: 14px; }}
        .badge-repo {{
            background-color: rgba(88, 166, 255, 0.1);
            color: var(--accent-blue);
            border: 1px solid rgba(88, 166, 255, 0.2);
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 13px;
            font-family: monospace;
        }}
        .main-container {{
            display: flex;
            padding: 30px;
            gap: 30px;
            max-width: 1600px;
            margin: 0 auto;
        }}
        .sidebar {{
            flex: 1;
            min-width: 320px;
            max-width: 450px;
            background-color: var(--bg-surface);
            border: 1px solid var(--border-color);
            border-radius: 6px;
            padding: 20px;
            position: sticky;
            top: 30px;
            max-height: 85vh;
            overflow-y: auto;
        }}
        .content-area {{
            flex: 2;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }}
        .sidebar h2 {{ font-size: 18px; margin-top: 0; border-bottom: 1px solid var(--border-color); padding-bottom: 10px; color: #f0f6fc; }}
        
        /* Interactive Tree CSS Design */
        ul {{ list-style-type: none; padding-left: 20px; margin: 5px 0; }}
        li {{ margin: 6px 0; font-size: 14px; }}
        .folder-toggle {{ cursor: pointer; font-weight: 600; color: #e1b12c; user-select: none; }}
        .folder-toggle:hover {{ text-decoration: underline; }}
        .file-node a {{ text-decoration: none; color: var(--text-main); transition: color 0.2s; }}
        .file-node a:hover {{ color: var(--accent-blue); text-decoration: underline; }}
        .file-cs {{ color: #4cd137 !important; font-weight: 500; }}
        
        /* Content Card Custom Modules */
        .card {{
            background-color: var(--bg-surface);
            border: 1px solid var(--border-color);
            border-radius: 6px;
            padding: 20px;
            scroll-margin-top: 30px;
            transition: border-color 0.3s, box-shadow 0.3s;
        }}
        .card:target {{
            border-color: var(--accent-blue);
            box-shadow: 0 0 0 3px rgba(88, 166, 255, 0.15);
        }}
        .card-header {{
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            border-bottom: 1px solid var(--border-color);
            padding-bottom: 10px;
            margin-bottom: 15px;
        }}
        .card-header h3 {{ margin: 0; font-size: 20px; color: #f0f6fc; }}
        .path-badge {{ font-family: monospace; font-size: 12px; color: var(--text-muted); background: var(--bg-main); padding: 3px 8px; border-radius: 4px; border: 1px solid var(--border-color); }}
        .meta-row {{ font-size: 14px; color: var(--text-muted); margin-bottom: 15px; }}
        .type-highlight {{ color: #f0f6fc; font-weight: bold; }}
        .card h4 {{ font-size: 15px; margin: 15px 0 10px 0; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }}
        
        /* OO Elements Badges Rules */
        .element-list {{ padding-left: 0; }}
        .element-list li {{ display: flex; align-items: center; gap: 10px; background: var(--bg-main); padding: 8px 12px; margin: 6px 0; border-radius: 4px; border: 1px solid var(--border-color); font-family: monospace; font-size: 13px; overflow-x: auto; }}
        .badge {{ display: inline-block; padding: 2px 6px; font-size: 11px; font-weight: bold; border-radius: 3px; text-transform: uppercase; font-family: sans-serif; flex-shrink: 0; }}
        .badge-prop {{ background-color: rgba(255, 166, 87, 0.1); color: var(--accent-orange); border: 1px solid rgba(255, 166, 87, 0.2); }}
        .badge-method {{ background-color: rgba(126, 231, 135, 0.1); color: var(--accent-green); border: 1px solid rgba(126, 231, 135, 0.2); }}
        code {{ font-family: ui-monospace, SFMono-Regular, SF Mono, Menlo, Consolas, Liberation Mono, monospace; font-size: 13px; color: #ff79c6; }}
        .element-list code {{ color: #e6edf3; }}
        .no-elements {{ font-size: 14px; color: var(--text-muted); font-style: italic; margin: 10px 0 0 0; }}
    </style>
</head>
<body>

<header>
    <div>
        <h1>Smart Factory Management System</h1>
        <p>Autogenerated Architectural Blueprints & Code Maps</p>
    </div>
    <div class="badge-repo">CasperGamingOne/Smart-Factory-Management-System</div>
</header>

<div class="main-container">
    <div class="sidebar">
        <h2>📁 Repository Map Tree</h2>
        <ul>
            {tree_interface_html}
        </ul>
    </div>
    
    <div class="content-area">
        <h2>⚙️ Source Component Specifications</h2>
        {file_details_html}
    </div>
</div>

<script>
    // Expand/Collapse script handling logic for interactive folders
    document.querySelectorAll('.folder-toggle').forEach(folder => {{
        folder.addEventListener('click', () => {{
            const childList = folder.nextElementSibling;
            if (childList.style.display === 'none') {{
                childList.style.display = 'block';
                folder.textContent = folder.textContent.replace('📁', '📂');
            }} else {{
                childList.style.display = 'none';
                folder.textContent = folder.textContent.replace('📂', '📁');
            }}
        }});
    }});
</script>

</body>
</html>
"""
    
    with open(output_html_file, 'w', encoding='utf-8') as f:
        f.write(html_template)

if __name__ == '__main__':
    # Determine local execution pathways safely across platform environments
    root_workspace = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
    output_target = os.path.join(root_workspace, 'index.html')
    print(f"Initializing documentation generation mapping out root: {{root_workspace}}")
    build_documentation_dashboard(root_workspace, output_target)
    print(f"Dashboard mapping built successfully -> Target output: {{output_target}}")
