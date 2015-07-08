using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentMapping.Internal;

namespace FluentMapping
{
    public partial class TypeMappingSpec<TTarget, TSource>
    {
        public TypeMappingSpec<TTarget, TSource> 
            ThatExpandsInto<TProperty>(
                Expression<Func<TTarget, TProperty>> nestedTargetObjectExpression,
                Func<TProperty> constructor)
            where TProperty : class
        {
            var nestedMapperSpec = FluentMapper
                .ThatMaps<TProperty>().From<TSource>()
                ;

            var unmappedSourceNames = new HashSet<string>(
                nestedMapperSpec.GetUnmappedSourceValues()
                    .Select(x => x.PropertyName)
                );

            nestedMapperSpec = nestedMapperSpec
                .WithSourceValues(
                    nestedMapperSpec.SourceValues
                        .Where(x => !unmappedSourceNames.Contains(x.PropertyName)));

            if (constructor != null)
                nestedMapperSpec = nestedMapperSpec.WithConstructor(constructor);

            var nestedMapper = nestedMapperSpec.Create();

            var nestedSourceValueNames = new HashSet<string>(
                nestedMapperSpec.SourceValues.Select(x => x.PropertyName));

            return this
                .ThatSets(nestedTargetObjectExpression).From(src => nestedMapper.Map(src))
                .WithSourceValues(
                    this.SourceValues
                        .Where(x => !nestedSourceValueNames.Contains(x.PropertyName)));
        }

        public ExpandIntoNestedTargetSpec<TTarget, TSource, TProperty, TProperty>
            ThatExpandsInto<TProperty>(
                Expression<Func<TTarget, TProperty>> nestedTargetObjectExpression
            )
            where TProperty : class
        {
            return new ExpandIntoNestedTargetSpec<TTarget, TSource, TProperty, TProperty>(
                this, 
                nestedTargetObjectExpression);
        }
    }
}