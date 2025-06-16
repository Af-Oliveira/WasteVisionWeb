using System;
using System.Collections.Generic;
using System.Linq;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Logging
{
    public class LogType : IEquatable<LogType>, IValueObject
    {

    public static LogType Auth => new LogType(0);
    public static LogType Error => new LogType(1);
    public static LogType Info => new LogType(2);
    public static LogType Debug => new LogType(3);
    public static LogType Category => new LogType(4);
    public static LogType Detection => new LogType(5);
    public static LogType RoboflowModel => new LogType(6);
    public static LogType Role => new LogType(7);
    public static LogType User => new LogType(8);
    public static LogType Prediction => new LogType(9);
    public static LogType ObjectPrediction => new LogType(10);
    public static LogType PredictionResult => new LogType(11);

    private static readonly Dictionary<int, string> _logTypes = new Dictionary<int, string>
    {
        { 0, "Auth" },
        { 1, "Error" },
        { 2, "Info" },
        { 3, "Debug" },
        { 4, "Category" },
        { 5, "Detection" },
        { 6, "RoboflowModel" },
        { 7, "Role" },
        { 8, "User" },
        { 9, "Prediction" },
        { 10, "ObjectPrediction" },
        { 11, "PredictionResult" }
    };

        private readonly int _value;

        private LogType(int value)
        {
            if (!_logTypes.ContainsKey(value))
                throw new ArgumentException("Invalid log value", nameof(value));
            _value = value;
        }

        public static LogType FromString(string logString)
        {
            var pair = _logTypes.FirstOrDefault(x => x.Value.Equals(logString, StringComparison.OrdinalIgnoreCase));
            if (pair.Equals(default(KeyValuePair<int, string>)))
                throw new ArgumentException("Invalid log string", nameof(logString));
            return new LogType(pair.Key);
        }

        public string AsString()
        {
            return _logTypes[_value];
        }

        public override string ToString()
        {
            return AsString();
        }

        public bool Equals(LogType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogType)obj);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(LogType left, LogType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogType left, LogType right)
        {
            return !Equals(left, right);
        }
    }
}
