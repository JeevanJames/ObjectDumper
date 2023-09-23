using System.Text;

namespace Jeevan.ObjectDumper;

public static class Dumper
{
    public static Dump Dump(object? instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        Type type = instance.GetType();
        if (type.IsValue())
            return new ValueDump(type, instance);

        ContainerDump rootContainer = type.IsCollection(out Type elementType)
            ? new CollectionDump(elementType, type)
            : new ObjectDump();

        DumpBuilder builder = new(instance, rootContainer);
        builder.Build();

        return rootContainer;
    }

    public static string DumpString(object instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        Dump dump = Dump(instance);
        if (dump is ValueDump vd)
            return vd.Value is null ? "[NULL]" : vd.Value.ToString();

        DumpStringBuilder builder = new((ContainerDump)dump);
        return builder.Build();
    }
}

internal sealed class DumpStringBuilder
{
    private const int _indent = 2;
    private readonly ContainerDump _dump;
    private readonly StringBuilder _sb = new();

    internal DumpStringBuilder(ContainerDump dump)
    {
        _dump = dump;
    }

    internal string Build()
    {
        Build(_dump, 0);
        return _sb.ToString();
    }

    private void Build(ContainerDump dump, int level)
    {
        switch (dump)
        {
            case ObjectDump od:
                foreach ((string propertyName, Dump propertyDump) in od.Properties)
                {
                    _sb.Indent(level).Append(propertyName).Append(": ");
                    if (propertyDump is ValueDump vd)
                        _sb.AppendLine(GetValueString(vd));
                    else
                    {
                        _sb.AppendLine();
                        Build((ContainerDump)propertyDump, level + _indent);
                    }
                }

                break;

            case CollectionDump cd:
                for (int i = 0; i < cd.Values.Count; i++)
                {
                    _sb.Indent(level).Append('[').Append(i).Append("]: ");
                    if (cd.Values[i] is ValueDump vd)
                        _sb.AppendLine(GetValueString(vd));
                    else
                    {
                        _sb.AppendLine();
                        Build((ContainerDump)cd.Values[i], level + _indent);
                    }
                }

                break;

            default:
                throw new NotSupportedException($"Unsupported container dump type {dump.GetType()}.");
        }
    }

    private static string GetValueString(ValueDump valueDump) =>
        valueDump.Value is null ? "[NULL]" : valueDump.Value.ToString();
}

internal static class StringBuilderExtensions
{
    internal static StringBuilder Indent(this StringBuilder sb, int indent)
    {
        for (int i = 0; i < indent; i++)
            sb.Append('·');
        return sb;
    }
}
