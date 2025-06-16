using DDDSample1.Domain.Shared;
using System.Text.RegularExpressions;

namespace DDDSample1.Domain.RoboflowModels
{
    public class ApiKey : IValueObject
    {
        public string Value { get; private set; }

        private ApiKey() { } // For ORM

        public ApiKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("API key cannot be empty.");
            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9]+$"))
                throw new BusinessRuleValidationException("API key can only contain letters and numbers.");

            this.Value = value;
        }

        public override string ToString() => Value;
        public string AsString() => Value;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return Value == ((ApiKey)obj).Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}