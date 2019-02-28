using System;
using System.Diagnostics;

namespace CompareThis.Helpers
{
    class DebugHelper
    {

        [Conditional("DEBUG")]
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        [Conditional("DEBUG")]
        public void WriteLine(int value)
        {
            Console.WriteLine(value);
        }

        [Conditional("DEBUG")]
        public void WriteLine(bool value)
        {
            Console.WriteLine(value);
        }
    }
}
