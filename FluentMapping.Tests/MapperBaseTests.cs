using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class MapperBaseTests
    {
        [Test]
        public void SimpleMapWithContext()
        {
            var mapper = new TestMapper();

            var result = mapper.Map(new Source());

            Assert.That(result.Foo, Is.EqualTo("Foo value"));
            Assert.That(result.Bar, Is.EqualTo(11));
        }

        public sealed class Target
        {
            public string Foo { get; set; }

            public int Bar { get; set; }
        }

        public sealed class Source
        {
            public string Foo { get { return "Foo value"; }}
        }

        private sealed class TestMapper : MapperBase<Target, Source>
        {
            protected override TypeMappingSpec<Target, Source> Configure(TypeMappingSpec<Target, Source> mappingSpec)
            {
                return mappingSpec
                    .ThatSets(tgt => tgt.Bar).From(src => 11);
            }
        }
    }
}