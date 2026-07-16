# Repository Guidelines for LLMs

## Architecture & Modularity
- **Strict Layering**: Code is separated into `Core` (domain/entities), `Services` (infrastructure/logic), and `UI` (TUI/Spectre.Console).
- **Modularity**: UI MUST NOT directly access data stores or Core logic without using Services. Keep code decoupled and extensible.
- **Security**: Always validate inputs strictly. Prevent outside manipulation (e.g., path traversal, injection). Code must be secure by default.

## Development Rules
- **Testing**: Always run `dotnet build` and `dotnet test` to verify changes. Builds and tests must pass.
- **Mermaid Diagrams**: When generating Mermaid class diagrams, STRIP complex generic signatures, default parameter values, and trailing spaces in class members to prevent `mermaid-cli` parsing errors.

## PR Summary Generation
When asked to review a PR:
1. Diff changes (`git diff <target>..<feature>`).
2. Fix obvious minor bugs/performance issues autonomously (verify with `dotnet build`/`dotnet test`).
3. Output the following exact format in chat:

```markdown
## Summary by Jules
[1-2 sentences overall goal]

New Features:
- [Details]
Bug Fixes:
- [Details]
Enhancements:
- [Details]
Refactoring:
- [Details]
```
