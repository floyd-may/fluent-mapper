using System;

namespace FluentMapping
{
    public interface IValue<T>
    {
        string PropertyName { get; }
        Type ValueType { get; }
        string Description { get; }
    }
}