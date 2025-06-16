using System.Text.RegularExpressions;

namespace DDDSample1.Domain.Shared
{
    public class Email : IValueObject
    {
        public string Value { get; private set; }

        public static readonly string PLACE_HOLDER = "placeholder@email.local";

        private Email() { } // For ORM

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Email cannot be empty.");
            if (!IsValidEmail(value))
                throw new BusinessRuleValidationException("Invalid email format.");
            this.Value = value;
        }

        private bool IsValidEmail(string email)
        {
            // More permissive regex for email validation, allowing local domains
            string pattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+$";
            return Regex.IsMatch(email, pattern);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Email)obj;
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

        public override string ToString()
        {
            return Value;
        }
    }
}
