using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Guardian.Test.Unit
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string name = "kevin";
            int age = 20;
            dynamic expando = new ExpandoObject();
            expando.Job = null;
            string[] data = new string[] { "One", "Two" };
            List<string> list = new List<string>() { "Three", "Four" };
            bool toggle = false;
            
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

            Guard.Requires(() => !(age > 20));
          
            Guard.Requires(() => null != name);

            Guard.Requires(() => name != null);

            Guard.Requires(() => name == null && (age > 20 || age < 20));
            
        }
    }
}
