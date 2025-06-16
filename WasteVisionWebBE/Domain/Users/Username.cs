using DDDSample1.Domain.Shared;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Users
{
    public class Username : IValueObject
    {
        public string Value { get; private set; }

        private Username() { } // For ORM

        public Username(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Username cannot be empty.");
            if (value.Length > 50)
                throw new BusinessRuleValidationException("Username cannot be longer than 50 characters.");
            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\s-_]+$"))
                throw new BusinessRuleValidationException("Username can only contain letters, numbers, spaces, hyphens, and underscores.");

            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Username)obj;
            return this.Value == other.Value;
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