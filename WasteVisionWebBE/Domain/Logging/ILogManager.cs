using System.Collections.Generic;

namespace DDDSample1.Domain.Logging
{
    public interface ILogManager
    {
        void Write(LogType type, string description);
        List<LogDto> Read(int numberOfLines = 100, bool reverse = true);
        int GetLines();
    }
}
