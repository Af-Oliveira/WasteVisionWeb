using System.Collections.Generic;

namespace DDDSample1.Domain.Logging
{
public class LogMapper
    {
        public static LogDto toDto(LogEntry logEntry)
        {
            return new LogDto
            {
                Type = logEntry.Type.ToString(),
                Timestamp = logEntry.Timestamp,
                Description = logEntry.Description
            };
        }

        public static LogEntry toDomain(LogDto logDto)
        {
            return new LogEntry
            {
                Type = LogType.FromString(logDto.Type),
                Timestamp = logDto.Timestamp,
                Description = logDto.Description
            };
        }

        public static IEnumerable<LogDto> toDtoList(IEnumerable<LogEntry> logEntries)
        {
            List<LogDto> logDtos = new List<LogDto>();

            foreach (LogEntry logEntry in logEntries)
            {
                logDtos.Add(toDto(logEntry));
            }

            return logDtos;
        }

        public static IEnumerable<LogEntry> toDomainList(IEnumerable<LogDto> logDtos)
        {
            List<LogEntry> logEntries = new List<LogEntry>();

            foreach (LogDto logDto in logDtos)
            {
                logEntries.Add(toDomain(logDto));
            }

            return logEntries;
        }
    }
}