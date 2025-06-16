using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.ObjectPredictions
{
    public class ObjectPredictionId : EntityId
    {
        [JsonConstructor]
        public ObjectPredictionId(Guid value) : base(value)
        {
        }

        public ObjectPredictionId(string value) : base(value)
        {
        }

        protected override Object createFromString(string text)
        {
            return new Guid(text);
        }

        public Guid AsGuid()
        {
            return (Guid)base.ObjValue;
        }

        public override string ToString()
        {
            return Value;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (ObjectPredictionId)obj;
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string AsString()
        {
            Guid obj = (Guid)base.ObjValue;
            return obj.ToString();
        }
    }
}