using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Logging;
using DDDSample1.Domain.Shared; 

public class LogEntry
    {
        public LogType Type { get; set; }
        public string Timestamp { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Type},{Timestamp},{Description}";
        }

        public static LogDto FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            return LogMapper.toDto(new LogEntry
            {
                Type = LogType.FromString(values[0]),
                Timestamp = values[1],
                Description = values[2]
            });
        }
    }