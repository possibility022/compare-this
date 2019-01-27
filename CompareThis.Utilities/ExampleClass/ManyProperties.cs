namespace CompareThis.Utilities.ExampleClass
{
    public class ManyProperties
    {
        public int Int1 { get; set; }
        public int Int2 { get; set; }
        public int Int3 { get; set; }
        
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }
        public string Str4 { get; set; }

        public bool Filter(string filter)
        {
            return ((filter != null)
                && (
                Int1.ToString().Contains(filter)
                || Int2.ToString().Contains(filter)
                || Int3.ToString().Contains(filter)
                || (Str1 != null) && (Str1.Contains(filter))
                || (Str2 != null) && (Str2.Contains(filter))
                || (Str3 != null) && (Str3.Contains(filter))
                || (Str4 != null) && (Str4.Contains(filter))
                )
                );
        }

    }
}
