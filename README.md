# fluent-mapper

FluentMapper is an object-to-object mapping library, similar in purpose to both AutoMapper and TinyMapper. However, FluentMapper is designed to offer two key benefits:

* FluentMapper is configured via a fluent API (hence the name)
* FluentMapper is opinionated [wiki](https://github.com/floyd-may/fluent-mapper/wiki)

A fluent object-to-object mapper for C#, inspired by Martin Fowler's [data mapper](http://martinfowler.com/eaaCatalog/dataMapper.html).

# Code Overview

Simplest mapping:

    FluentMapper.ThatMaps<TargetPlus>().From<Source>();

## Mapping Fields

* .ThatSets(tgt => tgt.City).From(src => src.City) - simple mapping
* .ThatSets(tgt => tgt.Country).From(src => src.Country.ToString()) - with some logic
* .ThatSets(tgt => tgt.StreetName).From(src => "Generated")  - static value

## Ignoring Fields

* .IgnoringTargetProperty(x => x.FieldTargetName)
* .IgnoringSourceProperty(x => x.FieldSourceName)

## using Context

* Simple implementation

      var mapper = FluentMapper
                .ThatMaps<TargetWithExtraStringC>().From<SourceWithExtraDoubleC>()
                .UsingContext<StringifyContext>()
                .ThatSets(x => x.C).From((src, ctx) => ctx.ArbitraryMethod(src.C))
                .Create();

* With constructor:

      var mapper = FluentMapper
                .ThatMaps<SimpleTarget>().From<SimpleSource>()
                .UsingContext<StringifyContext>()
                .WithConstructor(ctx => ctor.Object(ctx))
                .Create();

* Context with Builder

      var mapper = FluentMapper
                .ThatMaps<BuilderTarget>().From<SimpleSource>()
                .WithTargetAsBuilder()
                .UsingContext<StringifyContext>()
                .WithCustomMap((tgt, src, ctx) => tgt.WithB(ctx.ArbitraryMethod(src.B)))
                .WithCustomMap((tgt, src, ctx) => tgt.WithC("asdf"))
                .Create();

## Using expands into

* Simple implementation

       var mapper = FluentMapper.ThatMaps<Target>().From<Source>()
                .ThatExpandsInto(tgt => tgt.Nested).UsingDefaultMappings()
                .Create();

* Expand with constructor

      var mapper = FluentMapper.ThatMaps<Target>().From<Source>()
                .ThatExpandsInto(tgt => tgt.Nested)
                    .WithConstructor(() => new Nested("hi"))
                    .UsingDefaultMappings()
                .Create();


* Expand with custom mapping

      var mapper = FluentMapper.ThatMaps<Target>()
                .From<MismatchSource>()
                .ThatExpandsInto(tgt => tgt.Nested)
                    .UsingMapping(spec => spec
                        .ThatSets(tgt => tgt.OtherProp).From(src => src.Prop))
                .Create();

## Handling Null Input

* Default - will terminate with an argument exception
* .WithNullSource().ReturnNull()  - Returns null when null given
* .WithNullSource().ReturnEmptyObject() - Returns an empty object when null given.

# More information

For more information, please visit the [wiki](https://github.com/floyd-may/fluent-mapper/wiki) or take a look at the `.cs` files in `FluentMapping.Tests` to see how it works.
