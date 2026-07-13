import os
import re

# Configuration
PROJECT_DIRS = ["SmartFactory.Core", "SmartFactory.UI"] # Adjust names to match your actual folder structure
OUTPUT_FILE = "docs/index.md"

def extract_class_info(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract Class Name
    class_match = re.search(r'(class|enum) (\w+)', content)
    if not class_match: return None
    
    class_name = class_match.group(2)
    
    # Extract Members (simplified regex for C# public members)
    # Catches: public Type Name { get; set; } or public void Method()
    members = re.findall(r'public\s+[\w<>\[\]]+\s+(\w+)', content)
    
    return {"name": class_name, "members": members}

def generate_mermaid(all_classes):
    mermaid = "graph TD\n"
    # Create subgraphs for horizontal cutting
    mermaid += "subgraph Core\n"
    mermaid += "  " + " ; ".join([c['name'] for c in all_classes if "Core" in c['name']]) + "\n"
    mermaid += "end\n"
    mermaid += "subgraph UI\n"
    mermaid += "  " + " ; ".join([c['name'] for c in all_classes if "UI" in c['name']]) + "\n"
    mermaid += "end\n"
    return mermaid.replace("[]", "") # CRITICAL: Remove brackets to stop Mermaid crash

def run():
    all_classes = []
    os.makedirs("docs", exist_ok=True)
    
    for project in PROJECT_DIRS:
        for root, _, files in os.walk(project):
            for file in files:
                if file.endswith(".cs"):
                    info = extract_class_info(os.path.join(root, file))
                    if info: all_classes.append(info)

    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write("# Smart Factory System Documentation\n\n")
        f.write("## UML Architecture Map\n```mermaid\n" + generate_mermaid(all_classes) + "\n```\n\n")
        f.write("## Class Specifications\n")
        for cls in all_classes:
            f.write(f"### {cls['name']}\n")
            for member in cls['members']:
                f.write(f"- {member}\n")

if __name__ == "__main__":
    run()
