using System.Collections;
using System.Reflection;

namespace Jeevan.ObjectDumper;

/// <summary>
///     Main workhorse class that creates the full dump from the instance.
/// </summary>
internal sealed class DumpBuilder
{
    private readonly object _instance;
    private readonly ContainerDump _rootContainer;

    internal DumpBuilder(object instance)
    {
        _instance = instance;

        Type type = instance.GetType();
        if (type.IsDictionary(out Type? keyType, out Type? valueType))
            _rootContainer = new DictionaryDump(keyType, valueType, type);
        else if (type.IsCollection(out Type? elementType))
            _rootContainer = new CollectionDump(elementType, type);
        else
            _rootContainer = new ObjectDump(type);
    }

    internal ContainerDump Build()
    {
        Build(_instance, _rootContainer);
        return _rootContainer;
    }

    private void Build(object instance, ContainerDump dump)
    {
        switch (dump)
        {
            case ObjectDump od:
                BuildObjectDump(instance, od);
                break;

            case CollectionDump cd:
                BuildCollectionDump(instance, cd);
                break;

            case DictionaryDump dd:
                BuildDictionaryDump(instance, dd);
                break;

            default:
                throw new NotSupportedException($"Unsupported dump type {dump.GetType()}.");
        }
    }

    // For all the BuildXXXXDump methods, we typically check the property, collection element or dictionary
    // entry in the following order:
    // ✅ Check if the value is null. If it is, dump it; we cannot continue recursing.
    // ✅ Check if the value is a "value type", which means it cannot be recursed further. A value
    //     type could be:
    //     ☑ A built-in primitive type like bool, int, char, etc.
    //     ☑ An enum
    //     ☑ A string
    //     ☑ Any type that implements IFormattable, which is typically implemented by custom primitives
    //         like DateTime, Guid, Uri, etc.
    // ✅ Check if the value is a dictionary. If it is, we recursively iterate over each of its entries.
    // ✅ Check if the value is a collection. If it is, we recursively iterate over each element.
    // ✅ Finally, if all other conditions fail, then we assume it's an object and iterate recursively
    //     over each of its properties.

    private void BuildObjectDump(object instance, ObjectDump objectDump)
    {
        IEnumerable<PropertyInfo> properties = instance.GetType().GetProperties()
            .Where(pi => pi.CanRead && pi.GetIndexParameters().Length == 0);
        foreach (PropertyInfo property in properties)
        {
            object? propertyValue = property.GetValue(instance);
            Type propertyType = property.PropertyType.GetActualType();
            if (propertyValue is null)
                objectDump.Properties.Add(property.Name, new ValueDump(propertyType, null));
            else if (propertyType.IsValue())
                objectDump.Properties.Add(property.Name, new ValueDump(propertyType, propertyValue));
            else
            {
                ContainerDump childDump;
                if (propertyType.IsDictionary(out Type? keyType, out Type? valueType))
                    childDump = new DictionaryDump(keyType.GetActualType(), valueType.GetActualType(), propertyType);
                else if (propertyType.IsCollection(out Type? elementType))
                    childDump = new CollectionDump(elementType.GetActualType(), propertyType);
                else
                    childDump = new ObjectDump(propertyType);

                objectDump.Properties.Add(property.Name, childDump);
                Build(propertyValue, childDump);
            }
        }
    }

    private void BuildCollectionDump(object instance, CollectionDump collectionDump)
    {
        foreach (object? element in (IEnumerable)instance)
        {
            if (element is null)
                collectionDump.Values.Add(new ValueDump(collectionDump.ElementType, null));
            else
            {
                Type elementType = element.GetType().GetActualType();
                if (elementType.IsValue())
                    collectionDump.Values.Add(new ValueDump(elementType, element));
                else
                {
                    ContainerDump childDump;
                    if (elementType.IsDictionary(out Type? keyType, out Type? valueType))
                        childDump = new DictionaryDump(keyType, valueType, elementType);
                    else if (elementType.IsCollection(out Type? childElementType))
                        childDump = new CollectionDump(childElementType, elementType);
                    else
                        childDump = new ObjectDump(elementType);

                    collectionDump.Values.Add(childDump);
                    Build(element, childDump);
                }
            }
        }
    }

    private void BuildDictionaryDump(object instance, DictionaryDump dictionaryDump)
    {
        foreach (DictionaryEntry entry in (IDictionary)instance)
        {
            Dump keyDump = GetDumpFor(entry.Key, dictionaryDump.KeyType);
            Dump valueDump = GetDumpFor(entry.Value, dictionaryDump.ValueType);
            dictionaryDump.Values.Add(keyDump, valueDump);
        }
    }

    private Dump GetDumpFor(object? instance, Type instanceType)
    {
        if (instance is null)
            return new ValueDump(instanceType, null);

        if (instanceType.IsValue())
            return new ValueDump(instanceType, instance);

        ContainerDump instanceDump;
        if (instanceType.IsDictionary(out Type? keyType, out Type? valueType))
            instanceDump = new DictionaryDump(keyType, valueType, instanceType);
        else if (instanceType.IsCollection(out Type? elementType))
            instanceDump = new CollectionDump(elementType, instanceType);
        else
            instanceDump = new ObjectDump(instanceType);
        Build(instance, instanceDump);

        return instanceDump;
    }
}
