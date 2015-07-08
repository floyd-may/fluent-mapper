using System.Linq;
using NUnit.Framework;

namespace FluentMapping.Tests.Unit
{
    [TestFixture]
    public sealed class TypeMappingSpecTests
    {
        [Test]
        public void UnmappedTargets()
        {
            var spec = FluentMapper.ThatMaps<TargetPlus>().From<Source>();

            var unmappedTargets = spec.GetUnmappedTargetValues();

            Assert.That(unmappedTargets, Is.EqualTo(spec.TargetValues.Where(x => x.PropertyName == "C")));
        }

        [Test]
        public void UnmappedSources()
        {
            var spec = FluentMapper.ThatMaps<Target>().From<SourcePlus>();

            var unmappedSources = spec.GetUnmappedSourceValues();

            Assert.That(unmappedSources, Is.EqualTo(spec.SourceValues.Where(x => x.PropertyName == "C")));
        }

        public class Source
        {
            public string A { get { return "A"; } }
            public int B { get { return 11; } }
        }

        public class SourcePlus : Source
        {
            public string C { get { return "C value"; }}
        }

        public class Target
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        public class TargetPlus : Target
        {
            public string C { get; set; }
        }
    }
}