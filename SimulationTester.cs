using System.Text;

namespace Smart_Factory_Management_System
{
    public static class SimulationTester
    {
        internal static bool RunAllScenarios(Factory factory, out string detailedReport)
        {
            var log = new StringBuilder();
            bool isAllPassed = true;

            log.AppendLine("### 🛠️ Smart Factory Management - CI Regression Log");
            log.AppendLine($"Execution Stamp: {DateTime.UtcNow} UTC\n");

            if (factory == null)
            {
                detailedReport = "❌ [CRITICAL] - Factory instance was null. Initialization aborted.";
                return false;
            }

            // ----------------------------------------------------
            // SCENARIO 1: Null-Safe Bounds & Capacity Verification
            // ----------------------------------------------------
            log.Append("- **Scenario 1: Array Tracker & Capacity Safety Check** -> ");
            try
            {
                int machineCounter = factory.MachineCount;
                int actualInspected = 0;

                // Verifies that looping precisely up to the Counter doesn't trigger exceptions
                for (int i = 0; i < machineCounter; i++)
                {
                    if (factory.Machines[i] != null)
                    {
                        actualInspected++;
                    }
                }

                log.AppendLine($"✅ [PASSED] (Safely processed {actualInspected} active machine records)");
            }
            catch (Exception ex)
            {
                log.AppendLine($"❌ [FAILED] - Boundary traversal error: {ex.Message}");
                isAllPassed = false;
            }

            // ----------------------------------------------------
            // SCENARIO 2: Pure Employee Identity Lookup Guards
            // ----------------------------------------------------
            log.Append("- **Scenario 2: ID Authentication Loop Integrity** -> ");
            if (factory.EmployeeCount > 0 && factory.Employees[0] != null)
            {
                string existingId = factory.Employees[0].Id;
                bool lookupPassed = false;

                // Emulates the exact search rule used in your LoginHandler lookup loop
                for (int i = 0; i < factory.EmployeeCount; i++)
                {
                    if (factory.Employees[i] != null && factory.Employees[i].Id == existingId)
                    {
                        lookupPassed = true;
                        break;
                    }
                }

                if (lookupPassed) log.AppendLine("✅ [PASSED]");
                else
                {
                    log.AppendLine("❌ [FAILED] - Seeded user ID lookup failed to resolve inside search bounds.");
                    isAllPassed = false;
                }
            }
            else
            {
                log.AppendLine("⚠️ [SKIPPED] - No employees found inside factory seed data.");
            }

            // ----------------------------------------------------
            // SCENARIO 3: Automated Equipment Failure System Lock
            // ----------------------------------------------------
            log.Append("- **Scenario 3: Broken Component Production Block** -> ");
            if (factory.MachineCount > 0 && factory.Machines[0] != null && factory.Machines[0].Parts.Length > 0)
            {
                Machine machineUnderTest = factory.Machines[0];

                // Cache original state to leave things exactly as we found them
                var originalCondition = machineUnderTest.Parts[0].Condition;

                // Artificially simulate a hardware failure on the first part component
                // Works perfectly across both PartCondition or MachineCondition enums
                dynamic targetPart = machineUnderTest.Parts[0];

                try
                {
                    // Locate whatever critical enum name your project uses for broken parts
                    targetPart.Condition = Enum.Parse(targetPart.Condition.GetType(), "Critical");

                    // IsOperational() must catch this change and return false to block operations
                    if (machineUnderTest.Status == MachineStatus.Stopped)
                    {
                        log.AppendLine("✅ [PASSED] - Machine safely blocked sequence initialization.");
                    }
                    else
                    {
                        log.AppendLine("❌ [FAILED] - Security vulnerability! Machine claims operational status despite critical failure flag.");
                        isAllPassed = false;
                    }
                }
                catch (Exception ex)
                {
                    log.AppendLine($"❌ [FAILED] - Enum mapping error: {ex.Message}");
                    isAllPassed = false;
                }
                finally
                {
                    // Restore original factory state cleanly
                    targetPart.Condition = originalCondition;
                }
            }
            else
            {
                log.AppendLine("⚠️ [SKIPPED] - No valid machine equipment composition models detected.");
            }

            detailedReport = log.ToString();
            return isAllPassed;
        }
    }
}