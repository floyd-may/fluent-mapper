using System;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class FluentMapperBasicTests
    {
        [Test]
        public void SimpleMapping()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .Create();

            var result = mapper.Map(new SimpleSource());

            Assert.That(result.A, Is.EqualTo("A"));
            Assert.That(result.B, Is.EqualTo(11));
        }

        [Test]
        public void ThrowsForNull()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .Create();

            Assert.That(() => mapper.Map(null), Throws.Exception);
        }

        [Test]
        public void NullSourceBehavior()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .WithNullSource().ReturnNull()
                .Create();

            Assert.That(mapper.Map(null), Is.Null);
        }

        [Test]
        public void NullSourceEmptyObject()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .WithNullSource().ReturnEmptyObject()
                .Create();

            var result = mapper.Map(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.A, Is.Null);
            Assert.That(result.B, Is.EqualTo(0));
        }

        [Test]
        public void ThrowsForUnmatchedTarget()
        {
            var mapperSpec = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SimpleSource>();

            Assert.That(() => mapperSpec.Create(), Throws.Exception);

            try
            {
                mapperSpec.Create();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Target String TargetWithExtraStringC.C is unmatched.");
            }
        }

        [Test]
        public void ThrowsForUnmatchedSource()
        {
            var mapperSpec = FluentMapper
                .ThatMaps<SimpleTarget>().From<SourceWithExtraDoubleC>();

            Assert.That(() => mapperSpec.Create(), Throws.Exception);

            try
            {
                mapperSpec.Create();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Source Double SourceWithExtraDoubleC.C is unmatched.");
            }
        }

        [Test]
        public void ThrowsForMismatchedType()
        {
            var mapperSpec = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>();

            Assert.That(() => mapperSpec.Create(), Throws.Exception);

            try
            {
                mapperSpec.Create();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Cannot map [String TargetWithExtraStringC.C] from [Double SourceWithExtraDoubleC.C].");
            }
        }

        [Test]
        public void AutomaticallyIgnoresReadOnlyTargetProperties()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithReadonlyC>().From<SimpleSource>()
                .Create();

            var result = mapper.Map(new SimpleSource());

            Assert.That(result.A, Is.EqualTo("A"));
            Assert.That(result.B, Is.EqualTo(11));
        }

        [Test]
        public void IgnoreExtraTarget()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SimpleSource>()
                .IgnoringTargetProperty(x => x.C)
                .Create();

            var result = mapper.Map(new SimpleSource());

            Assert.That(result.A, Is.EqualTo("A"));
            Assert.That(result.B, Is.EqualTo(11));
        }

        [Test]
        public void IgnoreExtraSource()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SourceWithExtraDoubleC>()
                .IgnoringSourceProperty(x => x.C)
                .Create();

            var result = mapper.Map(new SourceWithExtraDoubleC());

            Assert.That(result.A, Is.EqualTo("A"));
            Assert.That(result.B, Is.EqualTo(11));
        }

        [Test]
        public void WithConstructor()
        {
            var mockCtor = new Mock<Func<SimpleTarget>>(MockBehavior.Strict);
            var target = new SimpleTarget();
            mockCtor.Setup(x => x()).Returns(target);

            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .WithConstructor(mockCtor.Object)
                .Create();

            var result = mapper.Map(new SimpleSource());

            mockCtor.Verify(x => x(), Times.Once());
        }
        
        [Test]
        public void SourceMappedViaInheritedInterface()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<ISimpleSource>()
                .Create();

            var result = mapper.Map(new SimpleSource());

            Assert.That(result.A, Is.EqualTo("A"));
            Assert.That(result.B, Is.EqualTo(11));
        }

        [Test]
        public void MapsIfAssignable()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithInterfaceProperty>()
                .From<SourceWithConcreteProperty>()
                .Create();

            var result = mapper.Map(new SourceWithConcreteProperty());
        }

        [Test]
        public void ThrowsIfNoConstructor()
        {
            var spec = FluentMapper.ThatMaps<ISimpleTarget>().From<SimpleSource>();

            Assert.That(() => spec.Create(), Throws.Exception);

            try
            {
                spec.Create();
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Unable to find constructor for type 'ISimpleTarget'."));
            }
        }

        public class SimpleSource : ISimpleSource
        {
            public string A { get { return "A"; } }
            public int B { get { return 11; } }
        }

        public interface ISimpleTarget
        {
            string A { get; set; }
            int B { get; set; }
        }

        public class SimpleTarget : ISimpleTarget
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        public class TargetWithInterfaceProperty
        {
            public ISimpleSource A { get; set; }
        }

        public class SourceWithConcreteProperty
        {
            public SimpleSource A {
                get { return new SimpleSource(); }
            }
        }

        public sealed class TargetWithExtraStringC : SimpleTarget
        {
            public string C { get; set; }
        }

        public sealed class TargetWithReadonlyC : SimpleTarget
        {
            public string C { get { return "hi"; } }
        }

        public sealed class SourceWithExtraDoubleC : SimpleSource
        {
            public double C { get { return 4.2; } }
        }

        public sealed class SourceWithExtraStringD : SimpleSource
        {
            public string D { get { return "a value"; } }
        }

        public interface ISimpleSource : ISource
        {
        }

        public interface ISource
        {
            string A { get; }
            int B { get; }
        }

        
    }
}