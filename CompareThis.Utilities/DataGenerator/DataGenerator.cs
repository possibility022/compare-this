using CompareThis.Utilities.ExampleClass;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static ClassWithOtherClass GetFilledUpClassWithOtherClasses()
        {
            return new ClassWithOtherClass()
            {
                BaseClass = new BasicClass()
                {
                    DateTimeProperty = new DateTime(2019, 1, 1),
                    IntProperty = 321,
                    StringProperty = "321"
                },
                ManyPropClass = new ManyProperties()
                {
                    Int1 = 321,
                    Int2 = 352565321,
                    Int3 = 4151213,
                    Str1 = "Some String number one ;)",
                    Str2 = "Some string number two :D",
                    Str3 = "mambo number five",
                    Str4 = "uga buga buga u FILTER!"
                },

                ClassWithCollection = GetClassWithCollection(),

                ClassWithNullables = new ClassWithNullables()
                {
                    NullableBool = false,
                    NullableByte = 0x11,
                    NullableDateTime = new DateTime(2000,10,10),
                    NullableInt = 321321321
                },

                SomeInt = 321,
                SomeString = "some string : |"
            };
        }

        public static ClassWithCollection GetClassWithCollection(int collectionSize = 100)
        {
            return new ClassWithCollection()
            {
                SomeCollection = GetListOfRandomStrings(collectionSize),
                ArrayOfInt = GetArrayOfInt(collectionSize)
            };
        }

        public static List<string> GetListOfRandomStrings(int length = 100, int lengthOfString = 10)
        {
            var list = new List<string>();

            for (int i = 0; i <= length; i++)
            {
                list.Add(RandomString(lengthOfString));
            }

            return list;
        }

        public static int[] GetArrayOfInt(int length = 100)
        {
            int[] arr = new int[length];
            for (int i = 0; i <= arr.Length - 1; i++)
            {
                arr[i] = i;
            }

            return arr;
        }
    }
}
