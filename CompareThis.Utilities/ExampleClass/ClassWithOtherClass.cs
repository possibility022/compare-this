namespace CompareThis.Utilities.ExampleClass
{
    public class ClassWithOtherClass
    {
        public BasicClass BaseClass { get; set; }

        public ManyProperties ManyPropClass { get; set; }

        public ClassWithCollection ClassWithCollection { get; set; }

        public ClassWithNullables ClassWithNullables { get; set; }

        public int SomeInt { get; set; }

        public string SomeString { get; set; }


        public bool Filter(string filter)
        {
            return (BaseClass.Filter(filter) || ManyPropClass.Filter(filter) || ClassWithNullables.Filter(filter)
                ||
                (string.IsNullOrEmpty(filter) == false && (SomeInt.ToString().Contains(filter) || SomeString.Contains(filter))));
        }
    }
}
