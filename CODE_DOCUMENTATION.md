# Smart Factory Management System - Code Documentation

This document provides a highly detailed description of each class and method present in the Smart Factory Management System project. The documentation is grouped by architectural layers to ensure a clear understanding of the codebase structure and responsibilities.

## Architectural Layers
- [Core (Domain/Entities)](#core)
- [Services (Infrastructure/Logic)](#services)
- [UI (User Interface)](#ui)
- [Text (Static Text/Constants)](#text)
- [Code Patterns & Design Decisions](#design)

---

## [Core](#core)

*Contains domain models and entities.*

### Employee.cs
This file defines the domain model for employees in the system. It implements a class hierarchy using the abstract `Employee` class as a base, with derived classes for specific roles (`Director`, `Technician`, `SalesAgent`, `Accountant`). It uses JSON polymorphism for serialization.

**Classes and Methods:**

*   **`Employee` (abstract)**: Base class for all employees.
    *   **Properties**:
        *   `Id` (int): Unique identifier for the employee, generated via a static counter.
        *   `Name` (string): The employee's full name.
        *   `Username` (string): The employee's login username.
        *   `PasswordHash` (string): The hashed password for authentication.
        *   `IsFirstTimeLogin` (bool): Flag indicating if the user must change their password on first login.
        *   `Role` (string): The role name (e.g., "Director", "Technician").
        *   `QuickActionName` (abstract string): Property to define the name for quick actions menu.
    *   **Methods**:
        *   `Employee(string name, string username, string passwordHash, bool isFirstTimeLogin = true)`: Constructor. Increments `_idCounter` and sets initial values.
        *   `UpdateName(string newName)`: Updates the employee's name, throws an exception if the string is empty or null.
        *   `UpdateUsername(string newUsername)`: Updates the employee's username, throws an exception if empty or null.
        *   `ChangePassword(string newPasswordHash)`: Updates the password hash and sets `IsFirstTimeLogin` to false.
        *   `InitializeIdCounter(int maxId)`: Static method to set the `_idCounter` when loading data from storage to avoid ID collisions.
        *   `ShowActivity()` (abstract): Returns a description of the employee's typical activity.
        *   `GetAvailableMenuOptions()` (abstract): Returns a list of menu options available to the user based on their role.

*   **`Director`**: Inherits from `Employee`. Represents a director role.
    *   **Methods**:
        *   Constructor sets the `Role` to "Director".
        *   `QuickActionName` returns "Quick Actions".
        *   `ShowActivity()` returns a specific string detailing the director's responsibilities.
        *   `GetAvailableMenuOptions()` returns menu options including "Employee Management", "View Operation History", and "Reports".

*   **`Technician`**: Inherits from `Employee`. Represents a technician role.
    *   **Methods**:
        *   Constructor sets the `Role` to "Technician".
        *   `QuickActionName` returns "Quick Actions".
        *   `ShowActivity()` returns a specific string detailing the technician's responsibilities.
        *   `GetAvailableMenuOptions()` returns menu options including "Machine Management".

*   **`SalesAgent`**: Inherits from `Employee`. Represents a sales agent role.
    *   **Methods**:
        *   Constructor sets the `Role` to "Sales Agent".
        *   `QuickActionName` returns "Quick Actions".
        *   `ShowActivity()` returns a specific string detailing the sales agent's responsibilities.
        *   `GetAvailableMenuOptions()` returns menu options including "Product Management".

*   **`Accountant`**: Inherits from `Employee`. Represents an accountant role.
    *   **Methods**:
        *   Constructor sets the `Role` to "Accountant".
        *   `QuickActionName` returns "Quick Actions".
        *   `ShowActivity()` returns a specific string detailing the accountant's responsibilities.
        *   `GetAvailableMenuOptions()` returns menu options including "Accounting" and "Reports".

### Factory.cs
This class acts as the central domain aggregate root for the system, managing collections of employees, machines, inventory, production batches, pending orders, and report requests.

**Classes and Methods:**

*   **`Factory`**: The main factory entity holding the state of the manufacturing system.
    *   **Properties**:
        *   `PendingOrders` (IEnumerable<ProductionOrder>): Read-only view of pending orders.
        *   `OrderCount` (int): Number of pending orders.
        *   `PendingReportRequests` (IEnumerable<ReportRequest>): Read-only view of pending report requests.
        *   `ReportRequestCount` (int): Number of pending report requests.
        *   `Batches` (IReadOnlyList<ProductionBatch>): Read-only list of production batches.
        *   `BatchCount` (int): Number of batches.
        *   `Employees` (IReadOnlyList<Employee>): Read-only list of employees.
        *   `Machines` (IReadOnlyList<Machine>): Read-only list of machines.
        *   `Inventory` (IReadOnlyList<Product>): Read-only list of inventory items.
        *   `MaxBatches` (int): Maximum allowed batches.
        *   `MaxCapacity` (int): Maximum production capacity.
        *   `MinStockThreshold` (int): Minimum threshold before stock is considered low.
    *   **Methods**:
        *   `GetTotalUnsoldUnits()`: Returns the sum of quantities from all batches that are not yet marked as sold.
        *   `LoadFromRepository(...)`: Hydrates the factory state from stored data. Importantly, it resets the static ID counters for `Employee` and `Machine` before materializing the lists to prevent ID inflation during deserialization, reconstructs `ProductionBatch` objects from loaded products, links products back to their batches, and finally updates the static ID counters to the maximum loaded ID.
        *   `AddEmployee(Employee employee)`: Adds an employee to the factory.
        *   `AddProduct(Product product, string? batchId)`: Adds a product to the inventory. If the product already exists with the same batch ID, name, and sold status, it increments the quantity. Otherwise, it adds a new product and links its index to the corresponding batch.
        *   `AddOrder(ProductionOrder order)`: Enqueues a new production order and re-sorts the queue based on the order's priority score.
        *   `AddReportRequest(ReportRequest request)`: Enqueues a new report request.
        *   `AddBatch(ProductionBatch batch)`: Adds a production batch.
        *   `ShowInventoryAlerts(IEnumerable<Product> inventory)`: Static method using `Spectre.Console` to display a warning table for products that are below the minimum stock threshold.
        *   `RemoveProduct(Product product)`: Removes a product from inventory.
        *   `RemoveBatch(ProductionBatch batch)`: Removes a batch.
        *   `RemoveEmployee(Employee employee)`: Removes an employee.

### Machine.cs
This file models the machines used in the factory's production line. It utilizes JSON polymorphism for serialization.

**Enums:**
*   `MachineStatus`: Represents if the machine is `Running` or `Stopped`.
*   `MachineCondition`: Represents the condition of the machine (`Excellent`, `Good`, `Critical`).

**Classes and Methods:**

*   **`Machine` (abstract)**: Base class for all production machines.
    *   **Properties**:
        *   `Id` (int): Unique identifier, generated via a static counter.
        *   `Name` (string?): Name of the machine.
        *   `Manufacturer` (string?): Manufacturer name.
        *   `SerialNumber` (string?): Serial number of the machine.
        *   `InstallationDate` (DateTime): Date the machine was installed.
        *   `Parts` (List<MachinePart>?): List of component parts belonging to the machine.
        *   `Status` (MachineStatus): Current status (running or stopped).
        *   `Condition` (MachineCondition): Current condition based on its parts.
        *   `SupportedProductType` (Type): Type of product this machine can process.
        *   `ActiveOrder` (ProductionOrder?): The order currently being processed.
        *   `TotalProcessedCount` (int): Tracks the number of successful operations.
        *   `TotalFailuresCount` (int): Tracks the number of failed operations.
        *   `SilentMode` (static bool): Flag to suppress console output during automated operations.
    *   **Methods**:
        *   `Machine(...)`: Constructor, increments the ID counter and initializes properties.
        *   `InitializeIdCounter(int maxId)`: Static method to set the `_idCounter` during deserialization.
        *   `IncrementSuccess()` / `IncrementFailure()`: Methods to update operation statistics.
        *   `GetMachineAge()` / `GetMachineAgeInYears()`: Calculates the age of the machine.
        *   `StartOrder(ProductionOrder order)`: Assigns an order and sets status to Running.
        *   `StartMachine()`: Simulates a boot sequence with console animations. Checks all parts; if any are critical, fails to boot. Otherwise, sets status to Running.
        *   `ApplyProductionWearAndTear()`: Simulates wear and tear during production. Has a 15% chance to degrade a random part by one step. Updates machine status/condition if a part becomes critical.
        *   `CalculateEfficiency()`: Calculates operational efficiency based on total processed vs failures.
        *   `GetEstimatedDaysUntilMaintenance()`: Calculates estimated days until maintenance based on part conditions.
        *   `PrintMachineInfo()`: Displays a comprehensive, formatted dashboard (using Spectre.Console) showing machine profile, components, predictive maintenance estimate, and efficiency.
        *   `NeedsRepair()`: Checks if the machine or any of its parts need repair.
        *   `RepairMachine()`: Simulates a repair process with console animations, setting all non-excellent parts to Excellent and resetting machine status.
        *   `GetMachineCondition()`: Determines the overall condition of the machine based on its most degraded part.
        *   `Produce(Product product)` (abstract): Processes a product, specific to machine types.

*   **`LitographyMachine`**: Inherits from `Machine`. Specializes in producing `Microprocessor`.
    *   **Methods**:
        *   Constructor sets `SupportedProductType` to `Microprocessor`.
        *   `Produce(Product blueprint)`: If valid, simulates production with console animations and applies wear and tear.

*   **`SmtMachine`**: Inherits from `Machine` (Solder Paste Printer). Specializes in producing `Motherboard`.
    *   **Methods**:
        *   Constructor sets `SupportedProductType` to `Motherboard`.
        *   `Produce(Product product)`: Verifies product type and state (`BlankBoard`). Transitions state to `SolderPrinted` and applies wear and tear.

*   **`PaPMachine`**: Inherits from `Machine` (Pick and Place).
    *   **Methods**:
        *   `Produce(Product product)`: Verifies product type and state (`SolderPrinted`). Transitions state to `ComponentsPlaced` and applies wear and tear.

*   **`ReflowOven`**: Inherits from `Machine`.
    *   **Methods**:
        *   `Produce(Product product)`: Verifies product type and state (`ComponentsPlaced`). Transitions state to `BakedAndSoldered` and applies wear and tear.

### MachinePart.cs
This file defines the parts that make up a machine, their condition, and specific types of parts. Uses JSON polymorphism.

**Enums:**
*   `PartCondition`: Represents the condition of a part (`Excellent`, `Good`, `Critical`).

**Classes and Methods:**

*   **`MachinePart` (abstract)**: Base class for machine components.
    *   **Properties**:
        *   `Name` (string?): Name of the part.
        *   `Condition` (PartCondition?): Current condition of the part.
    *   **Methods**:
        *   `BreakDown()`: Sets condition to Critical.
        *   `Repair(PartCondition restoredCondition)`: Sets the condition to the specified value.
        *   `DegradeStep()`: Degrades the part's condition by one level (Excellent -> Good -> Critical).
        *   `PrintPartInfo()` (abstract): Returns a formatted string with specific part information.

*   **`PowerSupply`**: Inherits from `MachinePart`.
    *   **Properties**: `Voltage` (int).
    *   **Methods**: `PrintPartInfo()` returns the voltage output.

*   **`CoolingSystem`**: Inherits from `MachinePart`.
    *   **Properties**: `Type` (string).
    *   **Methods**: `PrintPartInfo()` returns the cooling system type.

*   **`ControlUnit`**: Inherits from `MachinePart`.
    *   **Properties**: `Processor` (string).
    *   **Methods**: `PrintPartInfo()` returns the processor type.

*   **`AoiSystem`**: Inherits from `MachinePart`.
    *   **Properties**: `SystemType` (string).
    *   **Methods**: `PrintPartInfo()` returns the AOI system type.

### Product.cs
This file models the products manufactured by the factory. Uses JSON polymorphism.

**Enums:**
*   `BoardState`: Specific to motherboards (`BlankBoard`, `SolderPrinted`, `ComponentsPlaced`, `BakedAndSoldered`).

**Classes and Methods:**

*   **`Product` (abstract)**: Base class for products.
    *   **Properties**:
        *   `Name` (string?): Name of the product.
        *   `BatchId` (string?): Identifier for the batch this product belongs to.
        *   `IsSold` (bool): Flag indicating if the product has been sold.
        *   `ProductionCost` (double): Cost to produce the product.
        *   `SellingPrice` (double): Price at which the product is sold.
        *   `Quantity` (int): Number of items of this product type.
        *   `MinStockThreshold` (int): Minimum stock level before an alert is triggered.
    *   **Methods**:
        *   `IsLowStock()`: Returns true if quantity is at or below the minimum threshold.
        *   `MarkAsSold()`: Sets `IsSold` to true.
        *   `UpdateSellingPrice(double price)`: Updates the selling price.
        *   `AddQuantity(int amount)`: Increases quantity.
        *   `DeductQuantity(int amount)`: Decreases quantity, throws exception if insufficient stock.
        *   `GetTechnicalSpecifications()` (abstract): Returns a formatted string of specs.

*   **`Microprocessor`**: Inherits from `Product`.
    *   **Properties**: `Cores` (int?), `ClockSpeed` (double).
    *   **Methods**: `GetTechnicalSpecifications()` returns formatted string with cores and clock speed.

*   **`Motherboard`**: Inherits from `Product`.
    *   **Properties**: `CurrentState` (BoardState), `SocketStandard` (string?), `PhysicalForm` (string?).
    *   **Methods**:
        *   `TransitionTo(BoardState nextState)`: Updates the `CurrentState` (called by machines during production).
        *   `GetTechnicalSpecifications()` returns formatted string with socket and form factor.

### ProductionBatch.cs
This class groups identical products created in a single run. Used for inventory and accounting management.

**Classes and Methods:**

*   **`ProductionBatch`**: Represents a batch of produced items.
    *   **Properties**:
        *   `BatchId` (string): Unique identifier, defaults to a short GUID.
        *   `ProductName` (string): Name of the products in the batch.
        *   `Quantity` (int): Number of items in the batch.
        *   `UnitProductionCost` (double): Cost to produce a single item in this batch.
        *   `UnitSellPrice` (double?): Price at which a single item in this batch is sold.
        *   `IsPriced` (bool): True if `UnitSellPrice` is set (greater than 0).
        *   `IsSold` (bool): True if the entire batch has been marked as sold.
        *   `InventoryIndexes` (IReadOnlyList<int>): List of indices linking back to the `Factory.Inventory` list.
        *   `TotalCost` (double): Calculated total production cost (`UnitProductionCost` * `Quantity`).
    *   **Methods**:
        *   `SetUnitSellPrice(double? price)`: Sets the selling price; throws exception if negative.
        *   `MarkAsSold()`: Sets `IsSold` to true.
        *   `MarkAsUnsold()`: Sets `IsSold` to false (used for undo operations).
        *   `AddInventoryIndex(int index)`: Links an inventory item to this batch.

### ProductionOrder.cs
This class models a request to produce a specific quantity of a product.

**Classes and Methods:**

*   **`ProductionOrder`**: Represents a production task.
    *   **Properties**:
        *   `OrderId` (string): Unique identifier (short GUID).
        *   `ProductName` (string): General type name of the product.
        *   `Quantity` (int): Total number of items requested.
        *   `CompletedCount` (int): Number of items produced so far.
        *   `AssignedTechnicianId` (int): ID of the technician responsible for the order.
        *   `IsComplete` (bool): True if `CompletedCount` >= `Quantity`.
        *   `CustomProductName` (string): Specific name for the produced items.
        *   `Cores` (int?), `ClockSpeed` (double?): Specifics for Microprocessors.
        *   `SocketStandard` (string?), `PhysicalForm` (string?): Specifics for Motherboards.
        *   `PlacedBy` (string): Name of the user who placed the order.
        *   `IsNotifiedComplete` (bool): Flag for UI notifications.
        *   `BatchId` (string?): ID of the batch generated upon completion.
        *   `CreatedAt` (DateTime): Timestamp of order creation.
    *   **Methods**:
        *   `GetPriorityScore()`: Calculates a priority score based on time elapsed and quantity requested. Older and smaller orders get higher priority.
        *   `IncrementCompletedCount()`: Increments the completion counter; throws exception if already complete.

### ReportRequest.cs
This class models a request made by a Director for a specific report to be generated by an Accountant.

**Enums:**
*   `ReportStatus`: Status of the request (`Pending`, `Fulfilled`).

**Classes and Methods:**

*   **`ReportRequest`**: Represents a request for a report.
    *   **Properties**:
        *   `RequestId` (string): Unique identifier (short GUID).
        *   `ReportType` (string): Type of report requested (e.g., "Financial", "Inventory").
        *   `RequestedByDirectorName` (string): Name of the director who made the request.
        *   `IsNotifiedComplete` (bool): Flag for UI notifications.
        *   `ExportDestination` (string): Target location for the report (default "ReportsFolder").
        *   `Status` (ReportStatus): Current status.
    *   **Methods**:
        *   `Fulfill()`: Changes status to Fulfilled.
        *   `ToString()`: Returns a formatted string representing the request.

---

## [Services](#services)
*Contains infrastructure, data access, and business logic services.*

### ServicesInterfaces.cs
This file defines the interfaces that decouple the UI and other components from specific service implementations.

**Interfaces:**
*   **`IJsonRepository<T>`**: Defines a generic contract for loading and saving data.
    *   `Load()`: Returns a list of entities of type `T`.
    *   `Save(IEnumerable<T> data)`: Persists a collection of entities.
*   **`INotificationService`**: Contract for checking and displaying alerts/messages to users.
    *   `CheckAndShowNotifications(Employee user, Factory factory)`: Called upon login to show relevant alerts based on user role and system state.
*   **`ILoggerService`**: Contract for system activity logging.
    *   `LogInfo(...)`, `LogWarning(...)`, `LogError(...)`: Methods to record events with varying severity levels.
    *   `ShowOperationHistory()`: Renders the system log to the console.
*   **`IAccountService`**: Contract for managing user account details.
    *   `UpdateFullName(Employee user, string newName)`: Updates the user's name.
    *   `UpdateUsername(Employee user, string newUsername)`: Updates the user's login username.
    *   `UpdatePassword(Employee user, string newPassword)`: Updates the user's password.

### AccountService.cs
Implements `IAccountService`. Handles logic for updating employee credentials and synchronizing them with the underlying repository.

**Classes and Methods:**

*   **`AccountService`**:
    *   **Constructor**: Takes an `IJsonRepository<Employee>` for data access.
    *   **Methods**:
        *   `UpdateFullName(...)`: Validates the new name, updates the `Employee` object, and calls `PersistChanges`.
        *   `UpdateUsername(...)`: Validates the new username, checks the repository to ensure uniqueness, updates the object, and calls `PersistChanges`.
        *   `UpdatePassword(...)`: Validates the new password, hashes it using `SecurityHelper`, updates the object, and calls `PersistChanges`.
        *   `PersistChanges(Employee user)`: Private helper method. Loads all users, finds the matching record by ID, synchronizes the properties, and saves the collection back via the repository. Catches `IOException` and returns false if saving fails.

### Authentication.cs
Provides a static utility for authenticating users.

**Classes and Methods:**

*   **`Authentication` (static)**:
    *   **Methods**:
        *   `Authenticate(IJsonRepository<Employee> repository, string username, string password)`: Loads the list of users, searches for an exact (case-insensitive) username match, and verifies the provided password against the stored hash using `SecurityHelper`. Returns the `Employee` object if successful, or `null` otherwise.

### DataSeeder.cs
Responsible for populating the application with initial data if the data files do not exist.

**Classes and Methods:**

*   **`DataSeeder`**:
    *   **Constructor**: Takes repositories for employees, machines, products, and the file system service.
    *   **Methods**:
        *   `Seed()`: Checks for the existence of `employees.json`, `machines.json`, and `products.json` in the configured base directory. If they are missing, it initializes them with default data (a set of employees with different roles, four different types of machines with their respective parts, and a mix of sold/unsold inventory items to demonstrate the system's capabilities).

### FileSystemService.cs
Implements `IFileSystemService` to provide an abstraction over basic file operations.

**Classes and Methods:**

*   **`FileSystemService`**:
    *   **Constructor**: Takes an optional folder name (default "SmartFactoryData"). Resolves the path to the user's `MyDocuments` folder, appends the folder name, and creates the directory if it does not exist. Sets the `BaseDirectory` property.
    *   **Properties**:
        *   `BaseDirectory` (string): The path to the data folder.
    *   **Methods**:
        *   `WriteToFile(string fileName, string content)`: Overwrites a file with the given content.
        *   `ReadFromFile(string fileName)`: Reads the content of a file; returns an empty string if the file doesn't exist.
        *   `AppendToFile(string fileName, string content)`: Appends content (plus a newline) to a file.

### JsonRepository.cs
A generic implementation of `IJsonRepository<T>` that uses the file system service to serialize and deserialize data to/from JSON format.

**Classes and Methods:**

*   **`JsonRepository<T>`**:
    *   **Constructor**: Takes an `IFileSystemService` and a file name.
    *   **Fields**: `_serializerOptions` (JsonSerializerOptions) configured to write indented JSON.
    *   **Methods**:
        *   `Load()`: Reads the JSON string from the file system. If empty, returns an empty list; otherwise, deserializes into a `List<T>`.
        *   `Save(IEnumerable<T> items)`: Serializes the provided items to JSON and writes them to the file system.

### LoggerService.cs
Implements `ILoggerService` to record events to a flat file and provide a console-based view of the system history.

**Enums:**
*   `LogOrigin`: Identifies the source of the log event (e.g., System, Account, Production).
*   `LogEvent`: Defines the specific action that occurred (e.g., LoginSuccess, OrderPlaced).

**Classes and Methods:**

*   **`LoggerService`**:
    *   **Constructor**: Takes an `IFileSystemService` to write logs to `system_log.txt`.
    *   **Methods**:
        *   `LogInfo(LogOrigin origin, LogEvent eventType, string context)`: Maps an event type to a user-friendly string message and appends it to the log file as an INFO level entry.
        *   `LogWarning(LogOrigin origin, LogEvent eventType, string context)`: Maps an event type to a string message and appends it to the log file as a WARN level entry.
        *   `LogError(string message)`: Appends an ERROR level entry to the log.
        *   `ShowOperationHistory()`: Reads the log file, parses the bracket-enclosed format `[Timestamp] [Level] [Origin] Message`, and displays the history in a Spectre.Console `Table`.
        *   `WriteToFile(string level, string origin, string message)`: Private helper that formats and writes the actual string to the file using `IFileSystemService`.

### NotificationService.cs
Implements `INotificationService` to handle alert logic based on a user's role upon login.

**Classes and Methods:**

*   **`NotificationService`**:
    *   **Methods**:
        *   `CheckAndShowNotifications(Employee user, Factory factory)`: Examines the user's role and displays a blocking alert panel using Spectre.Console if actionable items exist:
            *   **Technician**: Automatically assigns unassigned production orders to them and notifies them.
            *   **Accountant**: Notifies them of any pending report requests.
            *   **SalesAgent**: Notifies them of completed orders they placed that haven't been acknowledged yet.
            *   **Director**: Notifies them of fulfilled report requests they initiated that haven't been acknowledged yet.

### SecurityHelper.cs
A static utility class for cryptographic operations.

**Classes and Methods:**

*   **`SecurityHelper` (static)**:
    *   **Methods**:
        *   `HashPassword(string password)`: Computes a SHA256 hash of the input password string and returns it as a Base64 encoded string.
        *   `VerifyPassword(string password, string hashedPassword)`: Hashes the input password and compares it to the stored hash, returning true if they match.

### UndoService.cs
Implements the Command pattern to provide undo functionality for certain actions in the system.

**Interfaces:**
*   **`ICommand`**: Represents an action that can be undone.
    *   `Description`: A string detailing the action.
    *   `ExecutedBy`: Username of the person who executed the action.
    *   `ExecutedAt`: Timestamp of the action.
    *   `Undo()`: Logic to reverse the action.
*   **`IUndoService`**: Contract for the undo manager.
    *   `RegisterCommand(ICommand command)`: Adds a command to the history.
    *   `GetUndoableCommands(Employee user)`: Retrieves commands the user is allowed to undo.
    *   `UndoCommand(ICommand command)`: Executes the `Undo` method on a command and removes it from history.

**Classes and Methods:**

*   **`UndoService` (Singleton)**:
    *   **Properties**: `Instance` (static singleton instance).
    *   **Methods**:
        *   `RegisterCommand(ICommand command)`: Adds to the static `Commands` list. Limits the history to the last 10 commands.
        *   `GetUndoableCommands(Employee user)`: Returns all commands if the user is a `Director`, otherwise only returns commands executed by that specific user.
        *   `UndoCommand(ICommand command)`: Calls `command.Undo()` and removes it from the list.

*   **Command Implementations**: Each class takes necessary dependencies (services, repositories, entities) in its constructor to perform the undo logic.
    *   **`ChangeFullNameCommand`**: Reverts a name change using `IAccountService`.
    *   **`ChangeUsernameCommand`**: Reverts a username change using `IAccountService`.
    *   **`SellFromBatchCommand`**: Reverts the sale of an entire batch by unmarking it as sold, resetting the price, and saving the inventory.
    *   **`SellFromInventoryCommand`**: Reverts a partial sale from inventory by adding the quantity back to the original product, removing the separated sold product/batch, and saving the inventory.
    *   **`RemoveEmployeeCommand`**: Reverts employee deletion by adding the employee back to the `Factory` and the `IJsonRepository<Employee>`.

## [UI](#ui)
*Contains handlers for the Text User Interface (TUI).*

### Program.cs
The main entry point for the console application. Sets up dependencies and manages the primary event loop.

**Classes and Methods:**

*   **`Program` (static)**:
    *   **Methods**:
        *   `Main()`: Initializes the application state, injects dependencies (repositories, services), calls the `DataSeeder`, and loads the `Factory` state. It contains an outer `while (true)` loop for the login screen (`LoginMenuHandler.ShowLoginScreen`). If successful, it checks for first-time login (`PasswordChangeHandler`), displays notifications (`NotificationService`), and enters the inner `sessionActive` loop. The inner loop renders the header (`TuiHelper`), dynamically builds a menu based on the user's role (`loggedInUser.GetAvailableMenuOptions()`), and dispatches to the corresponding `*MenuHandler`.
        *   `ExecuteQuickAction(...)`: Dispatches the "Quick Actions" menu option to the appropriate handler based on the concrete type of the `Employee`.

### TUIHelpers.cs
Contains shared UI rendering methods.

**Classes and Methods:**

*   **`TuiHelper` (static)**:
    *   **Methods**:
        *   `RenderSessionHeader(Employee user)`: Clears the console and draws a stylized header (using Spectre.Console `Rule`, `Grid`, and `Panel`) showing the current user's ID, Name, Role, and their role-specific Activity string.

### LoginMenuHandler.cs
Manages the terminal interface for user authentication.

**Classes and Methods:**

*   **`LoginMenuHandler` (static)**:
    *   **Methods**:
        *   `ShowLoginScreen(IJsonRepository<Employee> repository, ILoggerService loggerService)`: Displays the login panel. Prompts for username and password. If the user types "exit" as the username, returns `null` to shut down the app. Calls `Authentication.Authenticate()`. If successful, logs the event and returns the `Employee`. If it fails, logs the failure, shows an error message, and loops back.

### PasswordChangeHandler.cs
Manages the terminal interface forcing users to change their password on first login.

**Classes and Methods:**

*   **`PasswordChangeHandler` (static)**:
    *   **Methods**:
        *   `Run(Employee user, IJsonRepository<Employee> repository, ILoggerService loggerService)`: Displays a warning panel that this action cannot be undone. Prompts the user for a new password and a confirmation. Hashes the new password using `SecurityHelper`, updates the `Employee` object, finds the corresponding record in the repository, and saves the updated list. Logs the event.

### EmployeeMenuHandler.cs
Manages the terminal interface for viewing and editing employees.

**Classes and Methods:**

*   **`EmployeeMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(Factory factory, Employee loggedInUser, ILoggerService loggerService, IJsonRepository<Employee> repository)`: Displays the Employee Management menu loop. Options include viewing staff, adding a new employee, and removing an employee. Verifies that only `Director` roles can add or remove employees.
        *   `DisplayStaffTable(Factory factory)`: Internal helper that builds and renders a Spectre.Console `Table` of all current employees.
        *   `AddNewEmployeeFlow(...)`: Prompts the Director for a new employee's name, username, password, and role. Hashes the password, instantiates the corresponding subclass (`Technician`, `SalesAgent`, or `Accountant`), adds it to the `Factory`, saves it via the repository, and logs the event.
        *   `RemoveEmployeeFlow(...)`: Displays a list of employees (excluding the current user) for deletion. Asks for confirmation before removing the selected employee from the `Factory` and repository. Registers a `RemoveEmployeeCommand` with the `UndoService` and logs the event.

### MachineMenuHandler.cs
Manages the terminal interface for viewing and operating machines.

**Classes and Methods:**

*   **`MachineMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the Machine Management menu. Options include viewing the fleet status, running deep component inspections (Technician only), and fulfilling pending orders.
        *   `DisplayFleetOverview(Factory factory)`: Renders a table showing all machines, their status (`Running` vs `Stopped`), and their condition (`Excellent`, `Good`, `Critical`).
        *   `RunInspection(...)`: Prompts the user to select a machine. Calls `InspectMachine()` on the selected machine to display its detailed dashboard. If it needs repair (`NeedsRepair()`), prompts the user for confirmation and calls `RepairMachine()`. Saves the updated machine state to the repository and logs the maintenance event.

### ProductMenuHandler.cs
Manages the terminal interface for viewing inventory and analytics.

**Classes and Methods:**

*   **`ProductMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the Product Management menu and a summary panel detailing storage utilization. If total unsold units are below `factory.MinStockThreshold`, it displays a prominent flashing red alert. Options include viewing finished goods stock, viewing analytics, and accessing sales/orders.
        *   `DisplayInventoryTable(Factory factory)`: Renders a detailed table of all unsold products in the inventory, utilizing `product.GetTechnicalSpecifications()` to display formatted polymorphic data.
        *   `DisplayInventoryAnalytics(Factory factory)`: Calculates and displays cumulative metrics, grouping items by their specific subclass (`Microprocessor`, `Motherboard`) to show total volume, financial valuation, and storage occupancy percentage.

### ProductionMenuHandler.cs
Manages the terminal interface for the actual production process, primarily used by Technicians.

**Classes and Methods:**

*   **`ProductionMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the list of pending orders. Prompts the user to select an incomplete order to fulfill. Calls `StartProductionWorkflow`.
        *   `StartProductionWorkflow(...)`: Handles the core production logic. Verifies the user is the assigned technician. If it's a motherboard, calls `RunMotherboardWorkflow`. Otherwise, prompts the user to select a machine (if not auto-matched) or boots the matched machine if it's not running. Uses Spectre.Console's `Live` grid to display a real-time progress bar. Inside a loop, it instantiates `Product` items and calls `Machine.Produce()`. If the machine trips (fails wear and tear), it stops the loop. Finally, it prompts for the actual cost, finalizes the batch, saves state, and logs the outcome.
        *   `RunMotherboardWorkflow(...)`: A specialized, multi-stage workflow specifically for Motherboards requiring three separate machines (SMT Printer, Pick and Place, Reflow Oven) in sequence, displaying live progress for each stage.

### AccountingMenuHandler.cs
Manages the terminal interface for accounting functions.

**Classes and Methods:**

*   **`AccountingMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the Accounting menu (accessible only to Accountants). Options include viewing batches and processing report requests.
        *   `ShowBatches(Factory factory)`: Renders a table of all production batches.
        *   `ProcessReportRequests(...)`: Prompts the accountant to fulfill a pending report request made by a Director. Marks it fulfilled, saves the state, and calls `ExportReportToCsv`.
        *   `ExportReportToCsv(...)`: Generates a `.csv` file based on the requested report type (`Production Summary`, `Employee Report`, or `Order Backlog Summary`) and saves it to the specified location (Desktop or Data folder).
        *   `RunQuickActions(...)`: Provides a streamlined menu for the Accountant's quick actions.

### SalesMenuHandler.cs
Manages the terminal interface for the sales department, primarily used by Sales Agents.

**Classes and Methods:**

*   **`SalesMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the Sales menu loop. Verifies the user has access. Options include placing production orders, viewing pending orders, and recording sales.
        *   `PlaceOrder(...)`: Prompts for product type, custom name, and quantity (validating it's between 1 and 100). If Microprocessor, prompts for cores and clock speed; if Motherboard, prompts for socket and form factor. Creates a `ProductionOrder`, adds it to the `Factory`, saves it, and logs the event.
        *   `ShowPendingOrders(Factory factory)`: Displays a table of all current pending production orders.
        *   `RecordSale(...)`: Prompts the user to sell either a full un-sold `ProductionBatch` or individual items from `Inventory`.
            *   **Sell from Batch**: Updates the batch and linked inventory items to `IsSold=true` and sets the `UnitSellPrice`. Registers a `SellFromBatchCommand` for undo.
            *   **Sell from Inventory**: Deducts the specified quantity from the existing product. Creates a duplicate `Product` and `ProductionBatch` to represent the sold items and adds them to the factory history. Registers a `SellFromInventoryCommand` for undo.

### ReportMenuHandler.cs
Manages the terminal interface for generating reports.

**Classes and Methods:**

*   **`ReportMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the Reports menu. If the user is a `Director`, adds an option to request printable reports.
        *   `ShowEmployeeReport(Factory factory)`: Displays the staff table by reusing `EmployeeMenuHandler.DisplayStaffTable`.
        *   `RequestPrintableReport(...)`: Allows a Director to select a report type and a destination (Desktop or internally managed folder), creates a `ReportRequest`, adds it to the factory, and saves it.
        *   `ShowProductionSummary(...)`: Calculates and displays a comprehensive on-screen dashboard including overall inventory value, a table of all production batches with costs and sales, and a financial summary showing net profit/loss.
        *   `ShowOrderBacklogSummary(Factory factory)`: Displays a table of all pending and completed orders currently in the system backlog.
        *   `GetTotalInventoryUnits(Factory factory)`: Helper to calculate the sum of quantities of unsold inventory items.

### UndoMenuHandler.cs
Manages the terminal interface for undoing specific actions.

**Classes and Methods:**

*   **`UndoMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(Employee user, ILoggerService loggerService)`: Retrieves the list of undoable commands for the current user via `UndoService`. Displays the details of the most recent action and prompts for confirmation. If confirmed, calls `UndoService.Instance.UndoCommand()` and logs the undo event.

### AccountSettingsMenuHandler.cs
Manages the terminal interface for editing personal account details.

**Classes and Methods:**

*   **`AccountSettingsMenuHandler` (static)**:
    *   **Methods**:
        *   `Run(...)`: Displays the current user's profile information and a menu to change their Full Name, Username, or Password.
        *   `ChangeFullName(...)`: Prompts for a new name and requires the user to re-enter their password for verification (`SecurityHelper.VerifyPassword`). Updates the name via `IAccountService`, registers a `ChangeFullNameCommand` with the `UndoService`, and logs the event.
        *   `ChangeUsername(...)`: Similar to changing the full name. Prompts for a new username, verifies the password, updates via `IAccountService` (which ensures uniqueness), registers a `ChangeUsernameCommand`, and logs the event.
        *   `ChangePassword(...)`: Displays a warning panel that password changes cannot be undone. Prompts for a new password twice for confirmation. Updates the password via `IAccountService` and logs the event.

## [Text](#text)
*Contains static text constants and format strings for the console UI, acting as a localization/text-resource layer.*

### Common.cs
*   **`Common`**: Defines globally used string constants such as standard prompts ("Press any key to continue..."), error prefixes, success prefixes, access denied messages, and simple Yes/No strings.

### Login.cs
*   **`Login`**: Defines string constants specifically used by the authentication and first-time password setup processes, including prompts for credentials, success/failure messages, and gateway titles.

### MenuOptions.cs
*   **`MenuOptions`**: Contains static string arrays used to build the interactive menus in the UI (e.g., `MainMenu`, `ProductMenu`, `EmployeeRoles`, `FormFactors`).

### Employees.cs
*   **`Employees`**: Defines text constants for the Employee Management module, such as table headers, prompts for adding/removing employees, and success/error messages.

### Machines.cs
*   **`Machines`**: Defines text constants for the Machine Management module. Includes extensive formatted strings for the boot sequence, shutdown sequence, diagnostic dashboard, repair animations, and production process updates.

### Products.cs
*   **`Products`**: Defines string constants used in the Product Management and Inventory Analytics dashboards, including headers, warnings for low stock, and column names.

### Production.cs
*   **`Production`**: Defines string constants for the Production execution module, including progress bar updates, workflow steps (SMT, PaP, Reflow), and notification strings.

### Sales.cs
*   **`Sales`**: Defines string constants for the Sales module, including prompts for ordering custom components (cores, clock speed, socket, form factor) and recording batch/inventory sales.

### History.cs
*   **`History`**: Defines strings for the system log viewer, including table headers.

### AccountSettings.cs
*   **`AccountSettings`**: Defines strings for the profile settings UI, including table structure and success messages.

### Accounting.cs
*   **`Accounting`**: Defines strings for the Accounting module, including pending report notifications and batch pricing.

### Reports.cs
*   **`Reports`**: Defines strings for generating and viewing reports, including table columns for financial data, order backlogs, and CSV export destination strings.

### Undo.cs
*   **`UndoText`**: Defines strings for the Undo module, including warnings about passwords not being undoable and confirmation dialogs.

## [Code Patterns & Design Decisions](#design)

### Why JSON Polymorphism?
The project utilizes `[JsonDerivedType(typeof(DerivedClass), "typeDiscriminator")]` on base classes like `Employee`, `Machine`, `MachinePart`, and `Product`. This is a deliberate design decision for the `Core` entities to allow standard generic serialization via `System.Text.Json` (in `JsonRepository<T>`) while perfectly preserving the specific subclass properties (like `Microprocessor.Cores` or `LitographyMachine` vs `ReflowOven` behavior). This avoids writing custom JSON converters for complex inheritance structures and keeps the storage flat and simple.

### Strict Segregation of Concerns (Architecture)
The repository is split into distinct parts:
1.  **Core**: Contains NO UI code and NO saving/loading logic. It holds the pure business state (`Factory.cs`) and entity behaviors (e.g., `Machine.ApplyProductionWearAndTear()`).
2.  **Services**: Contains logic for data persistence (`JsonRepository`), authentication, and the Command Pattern for undo functionality (`UndoService`). The UI uses these via Interfaces, meaning the UI never directly reads/writes to files.
3.  **UI**: Depends on `Spectre.Console` for drawing, but cannot change the database directly. It must call `IJsonRepository.Save()` or a `Service` class.

### The Command Pattern for "Undo"
The `UndoService` implements the Command Pattern. Instead of just trying to "reverse" an action, operations that need to be undo-able (like changing a username, selling a product, or removing an employee) are encapsulated into classes implementing `ICommand` (e.g., `SellFromInventoryCommand`). When an action happens, the UI registers this command. If the user wants to undo it, the service calls `.Undo()` on that specific command object, which holds all the original context (like the old name, or the exact product that was sold) necessary to perfectly revert the state.

### Defensive Instantiation (`InitializeIdCounter`)
When loading entities like `Employee` or `Machine` from JSON, `System.Text.Json` invokes their constructors. Because the constructors contain an auto-incrementing static ID logic (`_idCounter++`), merely loading a list of 10 employees would inflate the counter by 10 unnecessarily. To fix this, `Factory.LoadFromRepository()` temporarily sets the static counters to 0 before materializing the lists, and then sets them to the maximum found ID afterwards, ensuring the next newly created entity gets the correct ID.

### Selective Serialization (`[JsonInclude]` and `[JsonIgnore]`)
The project utilizes `System.Text.Json.Serialization` attributes to precisely control what data is saved to disk and what is derived at runtime, prioritizing security and data integrity.

*   **`[JsonInclude]` on `init` properties**: In `ProductionOrder.cs`, properties like `OrderId`, `ProductName`, and `CreatedAt` use the `[JsonInclude]` attribute in combination with the `init` accessor. This is an exotic but powerful pattern. It means the property can only be set during object initialization (making the object immutable after creation from a coding perspective), but `System.Text.Json` is explicitly instructed that it is allowed to bypass this restriction during deserialization to populate the object from the JSON file. This guarantees that historical order data cannot be accidentally mutated by business logic later.

*   **`[JsonIgnore]` on runtime dependencies**: In `Machine.cs`, the `SupportedProductType` property (which holds a `System.Type` object) is decorated with `[JsonIgnore]`. The `Type` object is a runtime construct used to dynamically check if a machine can produce a certain product (e.g., `if (blueprint.GetType() != SupportedProductType)`). It makes no sense to serialize a C# `Type` object to a flat JSON file. By ignoring it, we keep the JSON payload small and clean, and the `Type` is simply re-assigned in the constructor of the concrete machine class (like `LitographyMachine`) when the object is instantiated during deserialization via the `JsonDerivedType` discriminator.


### LINQ Usage Snippets
The project heavily utilizes Language Integrated Query (LINQ) to efficiently filter, group, and analyze data in memory without complex `foreach` loops.

**1. Finding an item based on a condition (`FirstOrDefault`)**
Used to safely try to find an item, returning `null` if it doesn't exist, preventing index out of bounds exceptions.
```csharp
// Authentication.cs - Finding a user by case-insensitive username
var employee = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

// UI/ProductionMenuHandler.cs - Finding a specific batch
var existing = factory.Batches.FirstOrDefault(b => b.BatchId == order.BatchId);
```

**2. Filtering a collection (`Where`)**
Used to return a subset of a collection based on a predicate.
```csharp
// Services/NotificationService.cs - Finding unassigned orders
var unassigned = factory.PendingOrders.Where(o => o.AssignedTechnicianId == -1).ToList();

// UI/ProductMenuHandler.cs - Getting only unsold products for the inventory view
var unsoldProducts = factory.Inventory.Where(p => !p.IsSold).ToList();
```

**3. Checking for existence (`Any`)**
Used for fast boolean checks to see if at least one element matches a condition, without iterating the whole list if a match is found early.
```csharp
// Services/AccountService.cs - Ensuring a username isn't already taken (excluding the current user)
if (users.Any(u => u.Id != user.Id && u.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))

// UI/AccountingMenuHandler.cs - Checking if there are any pending report requests
if (!pendingRequests.Any())
```

**4. Aggregating values (`Sum`, `Max`)**
Used for financial calculations and finding highest values.
```csharp
// UI/ReportMenuHandler.cs - Calculating the total monetary value of unsold inventory
var inventoryValue = factory.Inventory.Where(p => !p.IsSold).Sum(p => p.SellingPrice * p.Quantity);

// Core/Factory.cs - Finding the highest ID to reset the auto-increment counter
var maxEmployeeId = employeeList.Max(e => e.Id);
```

**5. Transforming data (`Select`)**
Often used to project data into a new format, especially for building UI menus.
```csharp
// UI/EmployeeMenuHandler.cs - Transforming raw string options into numbered menu choices for Spectre.Console
var menuOptions = MenuOptions.EmployeeManagementMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
```

**6. Grouping related items (`GroupBy`)**
Used when loading flat JSON data and needing to reconstruct hierarchical relationships (like Products belonging to a Batch).
```csharp
// Core/Factory.cs - Grouping products by their BatchId to reconstruct ProductionBatch objects
var productsByBatch = productList.Where(p => !string.IsNullOrEmpty(p.BatchId)).GroupBy(p => p.BatchId);
```
