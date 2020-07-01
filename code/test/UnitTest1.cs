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

            Assert.Throws<Exception>(() => 
                Ensure.That(() => name == null)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => !(age > 50))
            );

            Assert.DoesNotThrow(() =>
                Ensure.That(() => list.All(o => o != null))
            );

            Assert.Throws<Exception>(() =>
                Ensure.That(() => list.All(o => o == null))
            );
            
            Assert.DoesNotThrow(() => 
                Ensure.That(() => !list.Any(o => o == null))
            );

            Assert.DoesNotThrow(() =>
                Ensure.That(() => Enumerable.All(list, o => o != null))
            );
            
            Assert.DoesNotThrow(() => 
                Ensure.That(() => !Enumerable.Any(list, o => o == null))
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => list[0].Length > 1)
            );
            
            Assert.Throws<Exception>(() =>
                Ensure.That(() => String.IsNullOrWhiteSpace(name))
            ); 
            
            Assert.DoesNotThrow(() => 
                Ensure.That(() => !String.IsNullOrWhiteSpace(name))
            );

            Assert.DoesNotThrow(() =>
                Ensure.That(() => data[0].Length > 1)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => name != null && age > 19 && age < 120)
            );

            Assert.Throws<Exception>(() => 
                Ensure.That(() => toggle && !toggle)
            );

            Assert.Throws<Exception>(() => 
                Ensure.That(() => age >= 21)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => null != name)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => name != null)
            );

            Assert.Throws<Exception>(() => 
                Ensure.That(() => name == null && (age > 20 || age < 20))
            );

            Assert.Throws<Exception>(() =>
                Ensure.That(() => (age > 20 || age < 20) && name == null)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => inner.Age > 1)
            );

            Assert.DoesNotThrow(() => 
                Ensure.That(() => inner[1] != "Something")
            );

            Assert.DoesNotThrow(() =>
                Ensure.That(() => !string.IsNullOrWhiteSpace(inner.Name))
            );
        }

        private static void Arguments(string name, int age, string[] data, List<string> list, Inner inner, int defaultValue = 21)
        {
            Ensure.That(() => list.All(o => o != null));

            Ensure.That(() => list.All(o => o == null));

            Ensure.That(() => Enumerable.All(list, o => o != null));

            Ensure.That(() => !Enumerable.Any(list, o => o == null));

            Ensure.That(() => list[0].Length > 1);

            Ensure.That(() => !String.IsNullOrEmpty(name));

            Ensure.That(() => !String.IsNullOrWhiteSpace(name));

            Ensure.That(() => data[0].Length > 1);

            Ensure.That(() => name != null && age > 19 && age < 120);

            Ensure.That(() => age >= defaultValue);

            Ensure.That(() => !(age > 20));

            Ensure.That(() => null != name);

            Ensure.That(() => name != null);

            Ensure.That(() => name == null && (age > 20 || age < 20));

            Ensure.That(() => inner.Age > 1);

            Ensure.That(() => inner[1] != "Something");

            Ensure.That(() => !string.IsNullOrWhiteSpace(inner.Name));
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
