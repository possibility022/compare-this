using CompareThis.Utilities.ExampleClass;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareThis.Utilities.DataGenerator
{
    public static class DataGenerator
    {

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static IList<BasicClass> GetBasicClass(int count)
        {
            return Builder<BasicClass>.CreateListOfSize(count)
                .All()
                    .With(c => c.StringProperty = null)
                .Random(count - (count * (25 / 100)))
                    .With(c => c.StringProperty = Faker.Lorem.Sentence(90))
                    .With(c => c.DateTimeProperty = null)
                .Build();
        }
    }
}
