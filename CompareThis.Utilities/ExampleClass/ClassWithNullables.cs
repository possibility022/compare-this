using System;

namespace CompareThis.Utilities.ExampleClass
{
    public class ClassWithNullables
    {
        public int? NullableInt { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public bool? NullableBool { get; set; }
        public byte? NullableByte { get; set; }

        public bool Filter(string filter)
        {
            return (filter != null && (
                (NullableInt.HasValue && NullableInt.Value.ToString().Contains(filter)) ||
                (NullableDateTime.HasValue && NullableDateTime.Value.ToString().Contains(filter)) ||
                (NullableBool.HasValue && NullableBool.Value.ToString().Contains(filter)) ||
                (NullableByte.HasValue && NullableByte.Value.ToString().Contains(filter))
                ));
        }
    }
}
