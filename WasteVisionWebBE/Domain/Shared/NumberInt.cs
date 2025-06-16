using System;

namespace DDDSample1.Domain.Shared
{
    public class NumberInt : IValueObject
    {
        public int Value { get; private set; }

        private NumberInt() { } // For ORM

        public NumberInt(int value)
        {
            Value = value;
        }

        public NumberInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            if (!int.TryParse(value, out int parsedValue))
                throw new ArgumentException("Value must be a valid integer.", nameof(value));
            Value = parsedValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is not NumberInt other)
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

               public string AsString()
        {
            return Value.ToString();
        }
    }
}