namespace CompareThis.Utilities.ExampleClass
{
    public class ClassWithOtherClass
    {
        public BasicClass BaseClass { get; set; }

        public ManyProperties ManyPropClass { get; set; }

        public int someInt { get; set; }

        public string someString { get; set; }


        public bool Filter (string filter)
        {
            return ((BaseClass.Filter(filter) || ManyPropClass.Filter(filter))
                ||
                (string.IsNullOrEmpty(filter) == false && (someInt.ToString().Contains(filter) || someString.Contains(filter))));
        }

    }
}
