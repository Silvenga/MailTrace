namespace MailTrace.Components.Models.Logs
{
    public class LineAttribute
    {
        public LineAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public LineAttribute()
        {
        }

        public string Name { get; set; }

        public string Value { get; set; }

        protected bool Equals(LineAttribute other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((LineAttribute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }
}