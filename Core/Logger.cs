using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Factory_Management_System
{
    public static class Logger
    {
        private static readonly string LogFilePath = "operations.txt";

        public static void Log(string actionDescription, string username = "System")
        {
            // Formatul: Timestamp | Acțiune
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}| {username} | {actionDescription}";

            try
            {
                using (StreamWriter sw = new StreamWriter(LogFilePath, true))
                {
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la scrierea log-ului: {ex.Message}");
            }
        }
    }
}
