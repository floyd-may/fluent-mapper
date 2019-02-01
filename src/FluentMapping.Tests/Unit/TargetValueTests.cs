using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace FluentMapping.Tests.Unit
{
    [TestFixture]
    public sealed class TargetValueTests
    {
        [Test]
        public void SetsProperties()
        {
            var propInfo = typeof (SomeClass).GetProperties().First();

            var testee = new TargetValue<SomeClass>(propInfo);

            Assert.AreEqual("A", testee.PropertyName);
            Assert.AreEqual(typeof(string), testee.ValueType);
        }

        [Test]
        public void CreatesSetterExpression()
        {
            var propInfo = typeof(SomeClass).GetProperties().First();

            var testee = new TargetValue<SomeClass>(propInfo);

            Expression<Func<SomeSourceClass, string>> getter = x => x.X;

            var expr = testee.CreateSetter(getter);

            var sourceInstance = new SomeSourceClass();
            var targetInstance = new SomeClass();

            (expr as Expression<Func<SomeClass, SomeSourceClass, SomeClass>>).Compile()(targetInstance, sourceInstance);

            Assert.AreEqual("a value", targetInstance.A);
        }

        public class SomeClass
        {
            public string A { get; set; }
        }

        public class SomeSourceClass
        {
            public string X { get { return "a value"; }}
        }
    }
}