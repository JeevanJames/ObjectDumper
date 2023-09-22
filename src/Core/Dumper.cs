namespace Jeevan.ObjectDumper;

public static class Dumper
{
    public static ContainerDump Dump(object instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        ContainerDump rootNode = instance.GetType().IsEnumerable(out Type elementType)
            ? new CollectionDump(elementType, instance.GetType()) : new ObjectDump();

        DumpBuilder builder = new(instance, rootNode);
        builder.Build();

        return rootNode;
    }
}
