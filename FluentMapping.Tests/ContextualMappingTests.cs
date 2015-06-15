using System.Globalization;
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
                .UsingContext<DoubleToStringContext>()
                .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
                .Create();

            var source = new SourceWithExtraDoubleC();
            var result = mapper.Map(source, new DoubleToStringContext());

            Assert.That(result.C, Is.EqualTo("4.2"));
        }
        
        [Test]
        public void ContextRetainedAfterIgnore()
        {
            var mapper = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>()
                .UsingContext<DoubleToStringContext>()
                .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
                .IgnoringSourceProperty(x => x.A)
                .IgnoringTargetProperty(x => x.A)
                .Create();

            var source = new SourceWithExtraDoubleC();
            var result = mapper.Map(source, new DoubleToStringContext());

            Assert.That(result.A, Is.Null);
            Assert.That(result.C, Is.EqualTo("4.2"));
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

        public sealed class TargetWithExtraStringC : SimpleTarget
        {
            public string C { get; set; }
        }

        public sealed class SourceWithExtraDoubleC : SimpleSource
        {
            public double C { get { return 4.2; } }
        }

        public sealed class DoubleToStringContext
        {
            public string ArbitraryMethod(double val)
            {
                return val.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}