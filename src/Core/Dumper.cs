namespace Jeevan.ObjectDumper;

public static class Dumper
{
    public static Dump Dump(object? instance)
    {
        if (instance is null)
            return new ValueDump(typeof(object), null);

        Type type = instance.GetType();
        if (type.IsValue())
            return new ValueDump(type, instance);

        DumpBuilder builder = new(instance);
        return builder.Build();
    }

    public static string DumpString(object instance, Action<StringDumpOptionsBuilder>? optionsBuilder = null)
    {
        Dump dump = Dump(instance);

        if (dump is ValueDump vd)
            return vd.Value is null ? "[NULL]" : vd.Value.ToString();

        StringDumpOptionsBuilder ob = new();
        optionsBuilder?.Invoke(ob);
        StringDumpOptions options = ob.Build();

        DumpStringBuilder builder = new((ContainerDump)dump, options);
        return builder.Build();
    }
}
