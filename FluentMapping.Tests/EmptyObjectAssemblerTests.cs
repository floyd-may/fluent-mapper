using System;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class EmptyObjectAssemblerTests
    {
        [Test]
        public void AssembleNotNull()
        {
            var source = "source";

            var ctor = new Mock<Func<string, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(source)).Returns("ctor result");

            var mapFunc = new Mock<Func<string, string, string>>(MockBehavior.Strict);
            mapFunc.Setup(x => x("ctor result", source)).Returns("map result");

            var testee = new EmptyObjectAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(source);

            Assert.That(result, Is.EqualTo("map result"));
        }

        [Test]
        public void AssembleNullSource()
        {
            var ctor = new Mock<Func<string, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(null)).Returns("ctor result");

            // note the lack of setup here on the mapFunc - it should get skipped
            var mapFunc = new Mock<Func<string, string, string>>(MockBehavior.Strict);

            var testee = new EmptyObjectAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(null);

            Assert.That(result, Is.EqualTo("ctor result"));
        }

        [Test]
        public void AssembleContextNonNull()
        {
            var source = "source";
            var context = DateTime.Now;

            var ctor = new Mock<Func<string, DateTime, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(source, context)).Returns("ctor result");

            var mapFunc = new Mock<Func<string, string, DateTime, string>>(MockBehavior.Strict);
            mapFunc.Setup(x => x("ctor result", source, context)).Returns("map result");

            var testee = new EmptyObjectAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(source, context);

            Assert.That(result, Is.EqualTo("map result"));
        }

        [Test]
        public void AssembleContextWithNull()
        {
            var ctor = new Mock<Func<string, DateTime, string>>(MockBehavior.Strict);
            ctor.Setup(x => x(null, It.IsAny<DateTime>())).Returns("ctor result");

            // note no setups on the map function
            var mapFunc = new Mock<Func<string, string, DateTime, string>>(MockBehavior.Strict);

            var testee = new EmptyObjectAssembler();
            var mapper = testee.Assemble(ctor.Object, mapFunc.Object);

            var result = mapper.Map(null, DateTime.Now);

            Assert.That(result, Is.EqualTo("ctor result"));
        }
    }
}