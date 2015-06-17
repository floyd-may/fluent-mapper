using System;
using System.Globalization;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class ContextualMappingTests
    {
        [Test]
        public void MapViaContext()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>()
                .UsingContext<StringifyContext>()
                .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
                .Create();

            var source = new SourceWithExtraDoubleC();
            var result = mapper.Map(source, new StringifyContext());

            Assert.That(result.C, Is.EqualTo("4.2"));
        }

        [Test]
        public void ContextualConstructor()
        {
            var ctor = new Mock<Func<StringifyContext, TargetWithExtraStringC>>(MockBehavior.Strict);
            ctor.Setup(x => x(It.IsAny<StringifyContext>())).Returns(() => new TargetWithExtraStringC());

                var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .UsingContext<StringifyContext>()
                .WithConstructor(ctx => ctor.Object(ctx))
                .Create();

            mapper.Map(new SimpleSource(), new StringifyContext());

            ctor.Verify(x => x(It.IsAny<StringifyContext>()));
        }

        [Test]
        public void ContextualConstructorWithNullSourceBehavior()
        {
            var ctor = new Mock<Func<StringifyContext, TargetWithExtraStringC>>(MockBehavior.Strict);
            ctor.Setup(x => x(It.IsAny<StringifyContext>())).Returns(() => new TargetWithExtraStringC());

            var mapper = FluentMapper
            .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>()
            .WithNullSource().ReturnNull()
            .UsingContext<StringifyContext>()
            .WithConstructor(ctx => ctor.Object(ctx))
            .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
            .Create();

            var result = mapper.Map(null, new StringifyContext());

            Assert.That(result, Is.Null);
            ctor.Verify(x => x(It.IsAny<StringifyContext>()), Times.Never());
        }
        
        [Test]
        public void ContextRetainedAfterIgnore()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>()
                .UsingContext<StringifyContext>()
                .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
                .IgnoringSourceProperty(x => x.A)
                .IgnoringTargetProperty(x => x.A)
                .Create();

            var source = new SourceWithExtraDoubleC();
            var result = mapper.Map(source, new StringifyContext());

            Assert.That(result.A, Is.Null);
            Assert.That(result.C, Is.EqualTo("4.2"));
        }

        [Test]
        public void CustomContextualMapWithBuilder()
        {
            var mapper = FluentMapper
                .ThatMaps<BuilderTarget>().From<SimpleSource>()
                .WithTargetAsBuilder()
                .UsingContext<StringifyContext>()
                .WithCustomMap((tgt, src, ctx) => tgt.WithB(ctx.ArbitraryMethod(src.B)))
                .WithCustomMap((tgt, src, ctx) => tgt.WithC("asdf"))
                .Create();

            var result = mapper.Map(new SimpleSource(), new StringifyContext());

            Assert.That(result.B, Is.EqualTo("11"));
            Assert.That(result.C, Is.EqualTo("asdf"));
        }

        public class SimpleSource
        {
            public string A { get { return "A"; } }
            public int B { get { return 11; } }
        }

        public class SimpleTarget
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        public class BuilderTarget
        {
            public string A { get; private set; }
            public string B { get; private set; }
            public string C { get; private set; }

            public BuilderTarget WithB(string val)
            {
                return new BuilderTarget
                {
                    B = val,
                    C = C,
                    A = A
                };
            }

            public BuilderTarget WithA(string val)
            {
                return new BuilderTarget
                {
                    B = B,
                    C = C,
                    A = val
                };
            }

            public BuilderTarget WithC(string val)
            {
                return new BuilderTarget
                {

                    A = A,
                    B = B,
                    C = val
                };
            }
        }

        public sealed class TargetWithExtraStringC : SimpleTarget
        {
            public string C { get; set; }
        }

        public sealed class SourceWithExtraDoubleC : SimpleSource
        {
            public double C { get { return 4.2; } }
        }

        public sealed class StringifyContext
        {
            public string ArbitraryMethod<T>(T val)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}", val);
            }
        }
    }
}