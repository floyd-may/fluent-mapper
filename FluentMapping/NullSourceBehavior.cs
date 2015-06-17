using System;
using System.Linq;

namespace FluentMapping
{
    public sealed class NullSourceBehavior<TTarget, TSource>
        where TTarget : class
        where TSource : class
    {
        public TypeMappingSpec<TTarget, TSource> Spec { get; private set; }

        public NullSourceBehavior(TypeMappingSpec<TTarget, TSource> spec)
        {
            Spec = spec;
        }

        public TypeMappingSpec<TTarget, TSource> ReturnNull()
        {
            return new TypeMappingSpec<TTarget, TSource>(
                Spec.TargetValues.ToArray(),
                Spec.SourceValues.ToArray(),
                Spec.CustomMappings.ToArray(),
                Spec.ConstructorFunc,
                new NullSourceAssembler()
                );
        }
    }
}