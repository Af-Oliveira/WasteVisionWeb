using DDDSample1.Domain.Shared;
using System;

namespace DDDSample1.Domain.Shared
{
    public class   Url : IValueObject
    {
        public string Value { get; private set; }

        private   Url() { } // For ORM

        public   Url(string value)
        {
                if (string.IsNullOrWhiteSpace(value))
                    throw new BusinessRuleValidationException("Model  Url cannot be empty.");
                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                    throw new BusinessRuleValidationException("Invalid  Url format.");
            
            this.Value = value;
        }

        public override string ToString() => Value;
        public string AsString() => Value;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return Value == ((  Url)obj).Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}