using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace FluentMapping.Tests
{
    [TestFixture]
    public sealed class CustomMappingTests
    {
        [Test]
        public void CustomSetterMatchingType()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .ThatSets(tgt => tgt.B).From(src => src.A)
                .Create();

            var source = new SimpleSource();
            var result = mapper.Map(source);

            Assert.That(result.B, Is.EqualTo("A"));
        }

        [Test]
        public void UnmatchedCustomITargetValueExceptionText()
        {
            var targetValue = new Mock<ITargetValue<SimpleTarget>>(MockBehavior.Strict);
            targetValue.Setup(x => x.Description).Returns("DESC");
            targetValue.Setup(x => x.PropertyName).Returns("Derp");

            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .ThatSets(tgt => tgt.B).From(src => src.A)
                ;
            mapper = new TypeMappingSpec<SimpleTarget, SimpleSource>(
                mapper.TargetValues.Concat(new[]{ targetValue.Object }).ToArray(),
                mapper.SourceValues.ToArray(),
                mapper.CustomMappings.ToArray(),
                mapper.ConstructorFunc,
                mapper.Assembler
                );

            Assert.That(() => mapper.Create(), Throws.Exception);

            try
            {
                mapper.Create();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Target DESC is unmatched.", ex.Message);
            }
        }

        [Test]
        public void CustomSetterWithTwoSourceProperties()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SourceWithStringC>()
                .ThatSets(tgt => tgt.B).From(src => src.A + " " + src.B)
                .Create();

            var source = new SourceWithStringC();
            var result = mapper.Map(source);

            Assert.That(result.B, Is.EqualTo("A B"));
        }

        [Test]
        public void CustomSetterViaTargetInterface()
        {
            var mapper = CreateMapperForTarget<SimpleTarget>();

            var source = new SimpleSource();
            var result = mapper.Map(source);

            Assert.That(result.B, Is.EqualTo("A"));
        }

        [Test]
        public void CustomSetterViaSourceInterface()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<ISource>()
                .ThatSets(tgt => tgt.B).From(src => src.A)
                .Create();

            var source = new SimpleSource();
            var result = mapper.Map(source);

            Assert.That(result.B, Is.EqualTo("A"));
        }

        [Test]
        public void CustomMap()
        {
            var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .WithCustomMap((tgt, src) => tgt.SetB(src.A))
                .IgnoringTargetProperty(tgt => tgt.B)
                .Create();

            var source = new SimpleSource();
            var result = mapper.Map(source);

            Assert.That(result.B, Is.EqualTo("A"));
        }

        private IMapper<TTarget, SimpleSource> CreateMapperForTarget<TTarget>() where TTarget : class, ITarget
        {
            return FluentMapper
                .ThatMaps<TTarget>().From<SimpleSource>()
                .ThatSets(tgt => tgt.B).From(src => src.A)
                .Create();
        }

        public class SimpleSource : ISource
        {
            public string A { get { return "A"; } }
        }

        public class SourceWithStringC : SimpleSource
        {
            public string B { get { return "B"; } }
        }

        public class SimpleTarget : ITarget
        {
            public string B { get; set; }

            public void SetB(string val)
            {
                B = val;
            }
        }

        public interface ISource
        {
            string A { get; }
        }

        public interface ITarget
        {
            string B { get; set; }
        }
    }
}