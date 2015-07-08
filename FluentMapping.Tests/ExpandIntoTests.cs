using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class ExpandIntoTests
    {
        [Test]
        public void SimpleExpand()
        {
            var mapper = FluentMapper.ThatMaps<Target>().From<Source>()
                .ThatExpandsInto(tgt => tgt.Nested).UsingDefaultMappings()
                .Create();

            var source = new Source();
            var result = mapper.Map(source);

            Assert.That(result.Thing, Is.EqualTo(source.Thing));
            Assert.That(result.Nested.OtherProp, Is.EqualTo(source.OtherProp));
        }

        [Test]
        public void ExpandWithConstructor()
        {
            var mapper = FluentMapper.ThatMaps<Target>().From<Source>()
                .ThatExpandsInto(tgt => tgt.Nested)
                    .WithConstructor(() => new Nested("hi"))
                    .UsingDefaultMappings()
                .Create();

            var source = new Source();
            var result = mapper.Map(source);

            Assert.That(result.Nested.CtorProp, Is.EqualTo("hi"));
            Assert.That(result.Thing, Is.EqualTo(source.Thing));
            Assert.That(result.Nested.OtherProp, Is.EqualTo(source.OtherProp));
        }

        [Test]
        public void ExpandWithCustomMapping()
        {
            var mapper = FluentMapper.ThatMaps<Target>().From<MismatchSource>()
                .ThatExpandsInto(tgt => tgt.Nested)
                    .UsingMapping(spec => spec
                        .ThatSets(tgt => tgt.OtherProp).From(src => src.Prop))
                .Create();

            var source = new MismatchSource();
            var result = mapper.Map(source);

            Assert.That(result.Thing, Is.EqualTo(source.Thing));
            Assert.That(result.Nested.OtherProp, Is.EqualTo(source.Prop));
        }

        [Test]
        public void ExpandViaBuilder()
        {
            var mapper = FluentMapper.ThatMaps<Target>().From<Source>()
                .ThatExpandsInto(tgt => tgt.Nested)
                    .Via(() => new NestedBuilder(), builder => builder.Build())
                    .UsingMapping(spec => spec.WithTargetAsBuilder())
                .Create();

            var source = new Source();
            var result = mapper.Map(source);

            Assert.That(result.Thing, Is.EqualTo(source.Thing));
            Assert.That(result.Nested.OtherProp, Is.EqualTo(source.OtherProp));
        }

        public sealed class Target
        {
            public Nested Nested { get; set; }

            public string Thing { get; set; }
        }

        public sealed class Nested
        {
            public Nested() { }

            public Nested(string ctorProp)
            {
                CtorProp = ctorProp;
            }

            public string CtorProp { get; private set; }

            public string OtherProp { get; set; }
        }

        public sealed class Source
        {
            public string Thing { get { return "Thing value"; } }

            public string OtherProp { get { return "OtherProp value"; } }
        }

        public sealed class MismatchSource
        {
            public string Thing { get { return "Thing value"; } }

            public string Prop { get { return "Prop value"; } }
        }

        public sealed class NestedBuilder
        {
            private string _otherProp;

            public NestedBuilder WithOtherProp(string prop)
            {
                this._otherProp = prop;

                return this;
            }

            public Nested Build()
            {
                return new Nested() { OtherProp = _otherProp };
            }
        }
    }
}