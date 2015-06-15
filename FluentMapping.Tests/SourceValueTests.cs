using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class SourceValueTests
    {
        [Test]
        public void SetsProperties()
        {
            var propInfo = typeof (SomeClass).GetProperties().First();

            var testee = new SourceValue<SomeClass>(propInfo);

            Assert.AreEqual("A", testee.PropertyName);
            Assert.AreEqual(typeof(string), testee.ValueType);
        }

        [Test]
        public void CreatesGetterExpression()
        {
            var propInfo = typeof(SomeClass).GetProperties().First();

            var testee = new SourceValue<SomeClass>(propInfo);

            var expr = testee.CreateGetter() as Expression<Func<SomeClass, string>>;

            var sourceInstance = new SomeClass {A = "test"};

            Assert.AreEqual("test", expr.Compile()(sourceInstance));
        }

        public class SomeClass
        {
            public string A { get; set; }
        }
    }
}