using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class Phone : IValueObject
    {
        public string Value { get; private set; }

        private Phone() { } // For ORM

        public Phone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Phone number cannot be empty.");
            if (!Regex.IsMatch(value, @"^\+?[1-9]\d{1,14}$"))
                throw new BusinessRuleValidationException("Invalid phone number format. Use E.164 format.");

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
            var other = (Phone)obj;
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
