using System;

namespace CompareThis.Utilities.ExampleClass
{
    public class BasicClass
    {
        public string StringProperty { get; set; }

        public int IntProperty { get; set; }

        public DateTime? DateTimeProperty { get; set; }

        public bool Filter(string filter)
        {
            return (filter != null)
            && ((StringProperty != null && StringProperty.Contains(filter))
            || (IntProperty.ToString().Contains(filter))
            || (DateTimeProperty.HasValue && DateTimeProperty.Value.ToString().Contains(filter))
            );
        }
    }
}
