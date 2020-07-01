using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;

namespace Guardian.Test.Unit
{
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            string name = "kevin";
            int age = 20;
            dynamic expando = new ExpandoObject();
            expando.Job = null;
            string[] data = new string[] { "One", "Two" };
            List<string> list = new List<string>() { "Three", "Four" };
            bool toggle = false;
            var inner = new Inner() { Name = name, Age = age };

            // Arguments(inner.Name, age, data, list, inner);
            Guard.Requires(() => name == null);

            Guard.Requires(() => !(age > 20));

            Guard.Requires(() => list.All(o => o != null));

            Guard.Requires(() => list.All(o => o == null));

            Guard.Requires(() => Enumerable.All(list, o => o != null));

            Guard.Requires(() => !Enumerable.Any(list, o => o == null));

            Guard.Requires(() => list[0].Length > 1);

            Guard.Requires(() => !String.IsNullOrEmpty(name));

            Guard.Requires(() => !String.IsNullOrWhiteSpace(name));

            Guard.Requires(() => data[0].Length > 1);

            Guard.Requires(() => name != null && age > 19 && age < 120);

            Guard.Requires(() => toggle && !toggle);

            Guard.Requires(() => age >= 21);

            Guard.Requires(() => null != name);

            Guard.Requires(() => name != null);

            Guard.Requires(() => name == null && (age > 20 || age < 20));

            Guard.Requires(() => inner.Age > 1);

            Guard.Requires(() => inner[1] != "Something");

            Guard.Requires(() => !string.IsNullOrWhiteSpace(inner.Name));

        }

        private static void Arguments(string name, int age, string[] data, List<string> list, Inner inner, int defaultValue = 21)
        {
            Guard.Requires(() => list.All(o => o != null));

            Guard.Requires(() => list.All(o => o == null));

            Guard.Requires(() => Enumerable.All(list, o => o != null));

            Guard.Requires(() => !Enumerable.Any(list, o => o == null));

            Guard.Requires(() => list[0].Length > 1);

            Guard.Requires(() => !String.IsNullOrEmpty(name));

            Guard.Requires(() => !String.IsNullOrWhiteSpace(name));

            Guard.Requires(() => data[0].Length > 1);

            Guard.Requires(() => name != null && age > 19 && age < 120);

            Guard.Requires(() => age >= defaultValue);

            Guard.Requires(() => !(age > 20));

            Guard.Requires(() => null != name);

            Guard.Requires(() => name != null);

            Guard.Requires(() => name == null && (age > 20 || age < 20));

            Guard.Requires(() => inner.Age > 1);

            Guard.Requires(() => inner[1] != "Something");

            Guard.Requires(() => !string.IsNullOrWhiteSpace(inner.Name));
        }

        private class Inner
        {
            private List<string> indexer = new List<string>()
            {
                "Index 1",
                "Index 2"
            };

            public string Name { get; set; }
            public int Age { get; set; }

            public string this[int index]
            {
                get { return indexer[index]; }
                set { indexer[index] = value; }
            }
        }
    }
}
