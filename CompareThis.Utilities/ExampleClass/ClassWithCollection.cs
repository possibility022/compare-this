using System.Collections.Generic;

namespace CompareThis.Utilities.ExampleClass
{
    public class ClassWithCollection
    {
        public ICollection<string> SomeCollection { get; set; }

        public bool Filter(string filter)
        {
            foreach (var s in SomeCollection)
            {
                if (s.Contains(filter))
                    return true;
            }

            return false;
        }

    }
}
