using System;

namespace DDDSample1.Domain.Shared
{
    public class Date : IValueObject
    {
        public DateTime Value { get; private set; }

        private Date() { } // For ORM

        public Date(DateTime value)
        {
            if (value == default)
                throw new ArgumentException("Date cannot be the default value.", nameof(value));
            Value = value;
        }

        public Date(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Date cannot be null or empty.", nameof(value));
            if (!DateTime.TryParse(value, out DateTime parsedValue))
                throw new ArgumentException("Date must be a valid DateTime.", nameof(value));
            Value = parsedValue;
        }

        public override string ToString()
        {
            return Value.ToString("yyyy-MM-dd"); // Format as ISO 8601
        }

        public override bool Equals(object obj)
        {
            if (obj is not Date other)
                return false;

            return Value.Date == other.Value.Date; // Compare only the date part
        }

        public override int GetHashCode()
        {
            return Value.Date.GetHashCode();
        }

        public string AsString()
        {
            return Value.ToString("yyyy-MM-dd"); // Format as ISO 8601
        }

        public bool IsAfter(Date other)
        {
            return Value.Date > other.Value.Date;
        }

        public bool IsBefore(Date other)
        {
            return Value.Date < other.Value.Date;
        }

    }
}