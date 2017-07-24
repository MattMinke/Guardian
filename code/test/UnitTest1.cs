using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;

namespace test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string name = null;
            int age = 20;
            dynamic expando = new ExpandoObject();
            expando.Job = null;

            System.Guard.Requires(() => !String.IsNullOrEmpty(name));

            System.Guard.Requires(() => !String.IsNullOrWhiteSpace(name));

            System.Guard.Requires(() => name != null && age > 20 && age < 120 );

            System.Guard.Requires(() => null != name);

            System.Guard.Requires(() => name != null);

            
        }
    }
}
