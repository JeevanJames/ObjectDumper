using System.Diagnostics;

namespace Jeevan.ObjectDumper;

#pragma warning disable S2094 // Classes should not be empty
public abstract class Dump
{
}
#pragma warning restore S2094 // Classes should not be empty

public abstract class ContainerDump : Dump
{
}

[DebuggerDisplay("Object - {Properties.Count} properties")]
public sealed class ObjectDump : ContainerDump
{
    public ObjectDump(Type type)
    {
        Type = type;
    }

    public Type Type { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public IDictionary<string, Dump> Properties { get; } = new Dictionary<string, Dump>(StringComparer.Ordinal);
}

[DebuggerDisplay("{CollectionType.Name,nq}<{ElementType.Name,nq}> - {Values.Count} elements")]
public sealed class CollectionDump : ContainerDump
{
    internal CollectionDump(Type elementType, Type collectionType)
    {
        ElementType = elementType;
        CollectionType = collectionType;
    }

    public Type ElementType { get; }

    public Type CollectionType { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public IList<Dump> Values { get; } = new List<Dump>();
}

[DebuggerDisplay("Dictionary<{KeyType.Name,nq}, {ValueType.Name,nq}> - {Values.Count} elements")]
public sealed class DictionaryDump : ContainerDump
{
    internal DictionaryDump(Type keyType, Type valueType, Type dictionaryType)
    {
        KeyType = keyType;
        ValueType = valueType;
        DictionaryType = dictionaryType;
    }

    public Type KeyType { get; }

    public Type ValueType { get; }

    public Type DictionaryType { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public IDictionary<Dump, Dump> Values { get; } = new Dictionary<Dump, Dump>();
}

/// <summary>
///     Represents a dump of a value or <c>null</c>.
/// </summary>
[DebuggerDisplay("Value: {Type.Name,nq} = {Value}")]
public sealed class ValueDump : Dump
{
    public ValueDump(Type type, object? value)
    {
        Type = type;
        Value = value;
    }

    public Type Type { get; }

    public object? Value { get; }
}
