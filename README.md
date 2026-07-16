# Smart Factory Management System

## Introduction
The **Smart Factory Management System** is a robust console-based application designed to manage the core operations of a modern manufacturing facility. Built using C# and the .NET framework, it provides a comprehensive text-based user interface (TUI) powered by `Spectre.Console`.

## Features
- **Role-Based Access Control:** Differentiated access and menus for Directors, Technicians, Sales Agents, and Accountants.
- **Production Management:** Initiate, track, and complete production orders (e.g., Motherboards, Microprocessors) utilizing a fleet of specialized machines.
- **Inventory & Batch Tracking:** Full visibility into finished goods, production batches, and stock capacity.
- **Machine Fleet Maintenance:** Track machine conditions, performance metrics, and perform essential maintenance.
- **Comprehensive Reporting:** Generate dynamic reports on staffing, revenue, machine fleet, and production summaries.
- **Auditing & Logging:** A centralized logging service tracks critical user actions and system events.

## Architecture
The system is cleanly decoupled into three main projects (namespaces):
- **Core:** Contains domain entities like `Factory`, `Employee`, `Machine`, and `Product`. Models the core business rules and state.
- **Services:** Handles infrastructure concerns such as JSON data persistence (`JsonRepository`), authentication, and logging (`LoggerService`).
- **UI:** A rich console application utilizing `Spectre.Console` for interactive menus, tables, and prompts, organized by domain handlers (`ProductMenuHandler`, `ProductionMenuHandler`, etc.).

## Installation / Setup Instructions
1. Navigate to the project's **Releases** page on GitHub.
2. Download the latest `.zip` release.
3. Extract the contents of the `.zip` file to a directory of your choice.
4. Run the executable (`SmartFactory_ManagementSystem.exe` or equivalent depending on your OS) directly from the extracted folder.
*(Note: Ensure your terminal supports ANSI escape sequences for the best TUI experience.)*

## Usage
Upon starting the application, you will be prompted to log in. Default seeded users are provided based on roles (e.g., `admin`/`admin` for Director, or specific usernames set up by `DataSeeder`).
Navigate through the intuitive TUI using the arrow keys and `Enter` to manage employees, execute production workflows, fulfill sales orders, and review factory reports.

## Learning Experience
Throughout the development of this project, several key software engineering concepts were applied and mastered:
- **Object-Oriented Programming (OOP):** Extensive use of inheritance and polymorphism to model Employees, Machines, and Products.
- **JSON Polymorphic Serialization:** Utilizing `[JsonDerivedType]` attributes to seamlessly serialize and deserialize complex object hierarchies into a single JSON file.
- **Terminal UI Design:** Leveraging `Spectre.Console` to build interactive, aesthetically pleasing, and user-friendly console interfaces beyond standard text output.
- **Layered Architecture:** Decoupling the user interface from business logic and data access, promoting cleaner code and easier maintenance.

## Project Structure & UML Diagram
Below is the comprehensive Mermaid class diagram illustrating the system's architecture, including classes, methods, fields, and their relationships, categorized by their respective projects.

![Architecture Diagram](Architecture_Diagram.svg)
