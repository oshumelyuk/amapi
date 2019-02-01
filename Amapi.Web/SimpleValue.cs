using System;

namespace Amapi.Web
{
    public class SimpleValue
    {
        public SimpleValue()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public SimpleValue(string value) : this()
        {
            Value = value;
        }

        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
