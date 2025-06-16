using System;

namespace DDDSample1.Domain.Shared
{
    public class NumberDouble : IValueObject
    {
        public double Value { get; private set; }

        private NumberDouble() { } // For ORM

        public NumberDouble(double value)
        {
            Value = value;
        }

        public NumberDouble(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            Value = ParseDouble(value);
        }

        public override string ToString()
        {
            return Value.ToString("F2"); // Format to 2 decimal places
        }

        public override bool Equals(object obj)
        {
            if (obj is not NumberDouble other)
                return false;

            return Math.Abs(Value - other.Value) < 0.0001; // Handle floating-point precision
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string AsString()
        {
            return Value.ToString();
        }

        private double ParseDouble(string value)
        {
            var normalizedDouble = value.Replace(',', '.');
            if (!double.TryParse(normalizedDouble, out double parsedValue))
                throw new ArgumentException("Value must be a valid double.", nameof(value));
            return parsedValue;
        }

    }
}