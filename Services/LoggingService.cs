using System;
using System.IO;
using System.Text;

namespace TransferFilesApp.Services
{
    public static class LoggingService
    {
        private static readonly string LogDirectory = @"C:\Transfer";
        private static readonly object LockObject = new object();

        public static void LogInfo(string message)
        {
            Log("INFO", message);
            Console.WriteLine($"[INFO] {message}");
        }

        public static void LogWarning(string message)
        {
            Log("WARNING", message);
            Console.WriteLine($"[WARNING] {message}");
        }

        public static void LogError(string message)
        {
            Log("ERROR", message);
            Console.WriteLine($"[ERROR] {message}");
        }

        private static void Log(string level, string message)
        {
            lock (LockObject)
            {
                try
                {
                    var logFileName = $"transfer_log_{DateTime.Now:yyyyMMdd}.log";
                    var logPath = Path.Combine(LogDirectory, logFileName);
                    var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {level} - {message}{Environment.NewLine}";

                    File.AppendAllText(logPath, logEntry, Encoding.UTF8);
                }
                catch
                {
                    // Ignorar erros de logging
                }
            }
        }
    }
}

