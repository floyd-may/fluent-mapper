using System;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests.Unit
{
    [TestFixture]
    public sealed class DefaultAssemblerTests
    {
        [Test]
        public void Assemble()
        {
            var source = "source";

            var ctor = new Mock<Func<string, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(source)).Returns("ctor result");

            var mapFunc = new Mock<Func<string, string, string>>(MockBehavior.Strict);
            mapFunc.Setup(x => x("ctor result", source)).Returns("map result");

            var testee = new DefaultAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(source);

            Assert.That(result, Is.EqualTo("map result"));
        }

        [Test]
        public void AssembleWithContext()
        {
            var source = "source";
            var context = DateTime.Now;

            var ctor = new Mock<Func<string, DateTime, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(source, context)).Returns("ctor result");

            var mapFunc = new Mock<Func<string, string, DateTime, string>>(MockBehavior.Strict);
            mapFunc.Setup(x => x("ctor result", source, context)).Returns("map result");

            var testee = new DefaultAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(source, context);

            Assert.That(result, Is.EqualTo("map result"));
        }
    }
}