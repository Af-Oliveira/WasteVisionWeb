using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DDDSample1.Domain.Logging
{
    public class LogManager : ILogManager
    {
        private readonly string _logFilePath;
        private readonly object _lock = new object();

        public LogManager(string logFilePath = "logs.csv")
        {
            _logFilePath = logFilePath;

            // Create file if it doesn't exist
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Close();
            }
        }

        public void Write(LogType type, string description)
        {
            var logEntry = new LogEntry
            {
                Type = type,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Description = description
            };

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, $"{logEntry}\n");
            }
        }

        public List<LogDto> Read(int numberOfLines = 100, bool reverse = true)
        {
            lock (_lock)
            {
                var lines = File.ReadAllLines(_logFilePath);
                var entries = lines.Select(LogEntry.FromCsv).ToList();

                if (reverse)
                {
                    entries.Reverse();
                }

                return entries.Take(numberOfLines).ToList();
            }
        }

        public int GetLines()
        {
            lock (_lock)
            {
                return File.ReadAllLines(_logFilePath).Length;
            }
        }
    }
}
