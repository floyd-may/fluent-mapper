using System;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class BuilderConventionTests
    {
        [Test]
        public void SimpleBuilderMap()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetBuilder>().From<SourceClass>()
                .WithTargetAsBuilder()
                .Create();

            var source = new SourceClass();

            var target = mapper.Map(source);

            Assert.That(target.Prop1, Is.EqualTo(22));
            Assert.That(target.Prop2, Is.EqualTo("the value"));
        }

        [Test]
        public void BuilderWithUnmatchedWritablePropertyThrows()
        {
            var mapperSpec = FluentMapper
                .ThatMaps<TargetBuilderWithExtraWritable>().From<SourceClass>()
                .WithTargetAsBuilder()
                ;

            Assert.That(() => mapperSpec.Create(), Throws.Exception);
        }

        [Test]
        public void BuilderViaInterface()
        {
            var mockTarget = new Mock<ITargetBuilder>(MockBehavior.Strict);
            mockTarget.Setup(x => x.WithProp1(22)).Returns(mockTarget.Object);
            mockTarget.Setup(x => x.WithProp2("the value")).Returns(mockTarget.Object);

            var mapper = FluentMapper
                .ThatMaps<ITargetBuilder>().From<SourceClass>()
                .WithConstructor(() => mockTarget.Object)
                .WithTargetAsBuilder()
                .Create();

            var source = new SourceClass();

            var target = mapper.Map(source);

            Assert.That(target, Is.SameAs(mockTarget.Object));
            mockTarget.VerifyAll();
        }

        [Test]
        public void BuilderWithCustomMap()
        {
            var mapperSpec = FluentMapper
                .ThatMaps<TargetBuilderWithMismatchTypes>().From<SourceClass>()
                .WithTargetAsBuilder()
                .WithMappingAction((tgt, src) => tgt.WithProp1(src.Prop1.ToString()));

            Assert.That(() => mapperSpec.Create(), Throws.Nothing);
        }

        public interface ITargetBuilder : IBuilder<ITargetBuilder>
        {
        }

        public interface IBuilder<out TBuilder>
        {
            TBuilder WithProp1(int val);
            TBuilder WithProp2(string val);
        }

        public class TargetBuilder
        {
            public int Prop1 { get; private set; }
            public string Prop2 { get; private set; }

            public TargetBuilder WithProp1(int val)
            {
                return new TargetBuilder
                {
                    Prop1 = val,
                    Prop2 = Prop2
                };
            }

            public TargetBuilder WithProp2(string val)
            {
                return new TargetBuilder
                {
                    Prop1 = Prop1,
                    Prop2 = val
                };
            }
        }

        public sealed class TargetBuilderWithExtraWritable
        {
            public int Prop1 { get; private set; }
            public string Prop2 { get; private set; }
            public double Writable { get; set; }

            public TargetBuilderWithExtraWritable WithProp1(int val)
            {
                Console.WriteLine("TargetBuilder.WithProp1({0})", val);
                return new TargetBuilderWithExtraWritable
                {
                    Prop1 = val,
                    Prop2 = Prop2,
                    Writable = Writable
                };
            }

            public TargetBuilderWithExtraWritable WithProp2(string val)
            {
                Console.WriteLine("TargetBuilder.WithProp2({0})", val);
                return new TargetBuilderWithExtraWritable
                {
                    Prop1 = Prop1,
                    Prop2 = val,
                    Writable = Writable
                };
            }
        }

        public class TargetBuilderWithMismatchTypes
        {
            public string Prop1 { get; private set; }
            public string Prop2 { get; private set; }

            public TargetBuilderWithMismatchTypes WithProp1(string val)
            {
                return new TargetBuilderWithMismatchTypes
                {
                    Prop1 = val,
                    Prop2 = Prop2
                };
            }

            public TargetBuilderWithMismatchTypes WithProp2(string val)
            {
                return new TargetBuilderWithMismatchTypes
                {
                    Prop1 = Prop1,
                    Prop2 = val
                };
            }
        }

        public sealed class SourceClass
        {
            public int Prop1 { get { return 22; } }
            public string Prop2 { get { return "the value"; } }
        }
    }
}