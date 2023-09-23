using System.Collections;
using System.Reflection;

namespace Jeevan.ObjectDumper;

/// <summary>
///     Main workhorse class that creates the full dump from the instance.
/// </summary>
internal sealed class DumpBuilder
{
    private readonly object _instance;
    private readonly ContainerDump _rootNode;

    internal DumpBuilder(object instance, ContainerDump rootNode)
    {
        _instance = instance;
        _rootNode = rootNode;
    }

    internal void Build()
    {
        Build(_instance, _rootNode);
    }

    private void Build(object instance, ContainerDump node)
    {
        switch (node)
        {
            case ObjectDump on:
                BuildObjectNode(instance, on);
                break;

            case CollectionDump an:
                BuildArrayNode(instance, an);
                break;

            default:
                throw new NotSupportedException($"Unsupported node type {node.GetType()}.");
        }
    }

    private void BuildObjectNode(object instance, ObjectDump objectDump)
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
            else if (propertyType.IsCollection(out Type elementType))
            {
                CollectionDump collectionDump = new(elementType, propertyType);
                objectDump.Properties.Add(property.Name, collectionDump);
                Build(propertyValue, collectionDump);
            }
            else
            {
                ObjectDump childObjectDump = new();
                objectDump.Properties.Add(property.Name, childObjectDump);
                Build(propertyValue, childObjectDump);
            }
        }
    }

    private void BuildArrayNode(object instance, CollectionDump collectionDump)
    {
        foreach (object? element in (IEnumerable)instance)
        {
            if (element is null)
                collectionDump.Values.Add(new ValueDump(collectionDump.ElementType, null));
            else if (element.GetType().IsValue())
                collectionDump.Values.Add(new ValueDump(element.GetType(), element));
            else if (element.GetType().IsCollection(out Type childElementType))
            {
                CollectionDump childCollectionDump = new(childElementType, element.GetType());
                collectionDump.Values.Add(childCollectionDump);
                Build(element, childCollectionDump);
            }
            else
            {
                ObjectDump childObjectDump = new();
                collectionDump.Values.Add(childObjectDump);
                Build(element, childObjectDump);
            }
        }
    }
}
