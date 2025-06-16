using DDDSample1.Domain.Shared;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class Description : IValueObject
    {
        public string Value { get; private set; }

        private Description() { } // For ORM

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Description cannot be empty.");
            if (value.Length > 300)
                throw new BusinessRuleValidationException("Description cannot be longer than 100 characters.");
            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\s-]+$"))
                throw new BusinessRuleValidationException("Description can only contain letters, numbers, spaces, and hyphens.");

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
            var other = (Description)obj;
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

