import os
import re

# Curata tipurile C# pentru ca Mermaid JS sa nu dea crash (ex: Product[] -> ProductArray)
def sanitize_mermaid(text):
    return text.replace("[]", "Array").replace("<", "_").replace(">", "_").replace(" ", "")

def parse_cs_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Extrage numele clasei, interfetei sau enum-ului
    class_match = re.search(r'(?:class|interface|enum|struct)\s+(\w+)', content)
    if not class_match:
        return None
    class_name = class_match.group(1)

    # Extrage Variabile / Proprietati (ignora get/set)
    prop_pattern = re.compile(r'(public|private|protected|internal)\s+(?:static\s+|readonly\s+|const\s+)?([\w\[\]<>,]+)\s+(\w+)\s*(?:\{|;|=)')
    props = prop_pattern.findall(content)

    # Extrage Metode si Constructori
    method_pattern = re.compile(r'(public|private|protected|internal)\s+(?:static\s+|virtual\s+|override\s+|async\s+|abstract\s+)?([\w\[\]<>,]+)\s+(\w+)\s*\(')
    methods = method_pattern.findall(content)

    return {
        "name": class_name,
        "props": props,
        "methods": methods
    }

repo_data = {}

# 1. Scaneaza toate fisierele si asociaza-le cu proiectul corect (Core vs UI)
for root, dirs, files in os.walk("."):
    if "bin" in root or "obj" in root or ".git" in root or "scripts" in root:
        continue
    
    for file in files:
        if file.endswith(".cs"):
            filepath = os.path.join(root, file)
            
            # Detecteaza automat din ce proiect face parte verificand folderele parinte
            project_name = "Root"
            current_dir = root
            while current_dir != "." and current_dir != "/":
                if any(f.endswith('.csproj') for f in os.listdir(current_dir)):
                    project_name = os.path.basename(current_dir)
                    break
                current_dir = os.path.dirname(current_dir)
            
            if project_name not in repo_data:
                repo_data[project_name] = []
            
            parsed = parse_cs_file(filepath)
            if parsed:
                repo_data[project_name].append(parsed)

# 2. Construieste fisierul Markdown
md_lines = ["# 🏭 Smart Factory Management System - Architecture Map\n"]

# SECTIUNEA A: Generarea Diagramei UML (Structurata orizontal)
md_lines.append("## 🗺️ System UML Diagram\n")
md_lines.append("*(Poți face screenshot pe sub-secțiuni pentru prezentare)*\n")
md_lines.append("```mermaid")
md_lines.append("classDiagram")

for proj, classes in repo_data.items():
    if not classes: continue
    # Creeaza subgrafice decuplate (Core si UI separate vizual)
    md_lines.append(f"    namespace {proj} {{")
    for cls in classes:
        c_name = cls['name']
        md_lines.append(f"        class {c_name} {{")
        
        for acc, typ, name in cls['props']:
            s_typ = sanitize_mermaid(typ)
            # Folosim + pentru public, - pentru private
            visibility = "+" if "public" in acc else "-"
            md_lines.append(f"            {visibility}{s_typ} {name}")
            
        for acc, typ, name in cls['methods']:
            s_typ = sanitize_mermaid(typ)
            visibility = "+" if "public" in acc else "-"
            md_lines.append(f"            {visibility}{s_typ} {name}()")
            
        md_lines.append("        }")
    md_lines.append("    }")
md_lines.append("```\n")

# SECTIUNEA B: Documentatie Detaliata 1 la 1
md_lines.append("## 📖 Detailed Class Map\n")
md_lines.append("Această secțiune este auto-generată și reprezintă situația exactă a codului din acest moment.\n")

for proj, classes in repo_data.items():
    if not classes: continue
    md_lines.append(f"### 📂 Project: {proj}\n")
    for cls in classes:
        md_lines.append(f"#### 🔹 `{cls['name']}`")
        if cls['props']:
            md_lines.append("**Variables & Properties:**")
            for acc, typ, name in cls['props']:
                md_lines.append(f"- `{acc} {typ} {name}`")
            md_lines.append("")
        
        if cls['methods']:
            md_lines.append("**Methods:**")
            for acc, typ, name in cls['methods']:
                # Daca metoda are acelasi nume cu clasa, e constructor
                if name == cls['name']:
                    md_lines.append(f"- `{acc} Constructor: {name}()`")
                else:
                    md_lines.append(f"- `{acc} {typ} {name}()`")
            md_lines.append("")
        md_lines.append("---\n")

# Salveaza fisierul in folderul docs
os.makedirs("docs", exist_ok=True)
with open("docs/index.md", "w", encoding="utf-8") as f:
    f.write("\n".join(md_lines))

print("Gata! Harta repo-ului si diagrama UML au fost generate in docs/index.md")
