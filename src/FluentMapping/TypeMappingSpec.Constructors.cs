using System;
using System.Linq;
using System.Linq.Expressions;
using FluentMapping.Internal;

namespace FluentMapping
{
    public partial class TypeMappingSpec<TTarget, TSource>
    {
        public TypeMappingSpec<TTarget, TSource> WithConstructor(
            Func<TTarget> constructorFunc)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                this.TargetValues.ToArray(),
                this.SourceValues.ToArray(),
                this.CustomMappings.ToArray(),
                dummy => constructorFunc(),
                this.Assembler
                );
        }

        public TypeMappingSpec<TTarget, TSource> WithConstructor(
           Func<TSource, TTarget> constructorFunc)
        {
            return new TypeMappingSpec<TTarget, TSource>(
                this.TargetValues.ToArray(),
                this.SourceValues.ToArray(),
                this.CustomMappings.ToArray(),
                constructorFunc,
                this.Assembler
                );
        }
    }
}