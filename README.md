# Smart_Factory_Management_System

> [!IMPORTANT]
> Project early in development! The repository map and UML overview are generated from `scripts/generate_repo_map.py`.
> 
> GitHub Pages: https://caspergamingone.github.io/Smart-Factory-Management-System/
> 
> Generated overview: [docs/PROJECT_OVERVIEW.md](docs/PROJECT_OVERVIEW.md)

## Project Map

The codebase is organized into a few clear layers:

- Domain model: `Factory.cs`, `Employee.cs`, `Machine.cs`, `MachinePart.cs`, `Product.cs`, `ProductionOrder.cs`, `ProductionBatch.cs`
- Application flow: `Program.cs`, `LoginHandler.cs`, `UIHelpers.cs`, `MenuOptions.cs`
- Feature menus: `AccountingMenuHandler.cs`, `EmployeeMenuHandler.cs`, `MachineMenuHandler.cs`, `ProductMenuHandler.cs`, `ProductionMenuHandler.cs`, `ReportMenuHandler.cs`, `SalesMenuHandler.cs`
- Generated documentation: `index.html`, `docs/PROJECT_OVERVIEW.md`, `scripts/generate_repo_map.py`

The generated overview includes a detailed UML class table, relationship summary, Mermaid diagram, and a file-by-file inventory of the repository.

## Quick Status View

[![.NET Basic Autotest](https://github.com/CasperGamingOne/Smart-Factory-Management-System/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CasperGamingOne/Smart-Factory-Management-System/actions/workflows/dotnet.yml)

[![Dependabot Updates](https://github.com/CasperGamingOne/Smart-Factory-Management-System/actions/workflows/dependabot/dependabot-updates/badge.svg)](https://github.com/CasperGamingOne/Smart-Factory-Management-System/actions/workflows/dependabot/dependabot-updates)

## Tasks and person assigned

> [!NOTE]
> Format: Task | Assigned Person

- [x] Factory.cs ~ Final             | @CasperGamingOne
- [x] Machine.cs ~ Final             | @CasperGamingOne
- [x] MachinePart.cs ~ Final         | @CasperGamingOne
- [ ] Product.cs                     | @ganeaandreea701-crypto
- [x] Employee.cs (Basic)            | @ganeaandreea701-crypto
- [ ] Program.cs                     | @CasperGamingOne & @ganeaandreea701-crypto
- [x] LoginHandler.cs                | @CasperGamingOne
- [x] *MenuHandler.cs* not Final     | @CasperGamingOne & @ganeaandreea701-crypto

-[ ] to revise code                  | @CasperGamingOne
-[ ] to make dynamic menus           | @CasperGamingOne
-[ ] to add menu restrictions        | @CasperGamingOne
