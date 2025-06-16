using DDDSample1.Domain.Shared;
using System;
using System.Text.Json.Serialization;

namespace DDDSample1.Domain.Users
{
    public class UserId : EntityId
    {
        [JsonConstructor]
        public UserId(Guid value) : base(value)
        {
        }

        public UserId(string value) : base(value)
        {
        }

        protected override Object createFromString(string text)
        {
            return new Guid(text);
        }

        public override String AsString()
        {
            Guid obj = (Guid)base.ObjValue;
            return obj.ToString();
        }

        public Guid AsGuid()
        {
            return (Guid)base.ObjValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (UserId)obj;
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
