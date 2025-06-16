using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DDDSample1.Domain.Logging;
using System;

namespace WasteVisionWebBE.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLogs()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "./mylogs.csv");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log file not found.");
            }

            var logs = new List<LogDto>();

            try
            {
                var csvLines = System.IO.File.ReadAllLines(filePath);

                foreach (var line in csvLines)
                {
                    var columns = line.Split(',');

                    if (columns.Length >= 3)
                    {
                        logs.Add(new LogDto
                        {
                            Type = columns[0].Trim(),
                            Timestamp = columns[1].Trim(),
                            Description = string.Join(",", columns.Skip(2)).Trim()
                        });
                    }
                }

                // Sort logs by timestamp (most recent first)
                var sortedLogs = logs.OrderByDescending(log =>
                {
                    if (DateTime.TryParseExact(log.Timestamp, "yyyy-MM-dd HH:mm:ss", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        return parsedDate;
                    }
                    // If parsing fails, try with different common formats
                    if (DateTime.TryParse(log.Timestamp, out DateTime fallbackDate))
                    {
                        return fallbackDate;
                    }
                    // If all parsing fails, return minimum date to put invalid entries at the end
                    return DateTime.MinValue;
                }).ToList();

                return Ok(sortedLogs);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the log file.");
            }
        }
    }
}
