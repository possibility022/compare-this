namespace CompareThis.Utilities.ExampleClass
{
    public class OnePropClass
    {
        public string Str { get; set; }

        public bool Filter(string filter)
            => (filter != null) && Str.Contains(filter);
    }
}
