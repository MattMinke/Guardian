using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Guardian;
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

            
            Guard.Requires(() => !String.IsNullOrEmpty(name));

            Guard.Requires(() => !String.IsNullOrWhiteSpace(name));

            Guard.Requires(() => name != null && age > 20 && age < 120 );

            Guard.Requires(() => null != name);

            Guard.Requires(() => name != null);

            Guard.Requires(() => name == null && (age > 20 || age < 20));
        }
    }
}
