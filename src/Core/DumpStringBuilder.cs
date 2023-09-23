using System.Text;

namespace Jeevan.ObjectDumper;

internal sealed class DumpStringBuilder
{
    private readonly ContainerDump _dump;
    private readonly StringDumpOptions _options;
    private readonly StringBuilder _sb = new();

    internal DumpStringBuilder(ContainerDump dump, StringDumpOptions options)
    {
        _dump = dump;
        _options = options;
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
                    Indent(level).Append(propertyName).Append(": ");
                    if (propertyDump is ValueDump vd)
                        _sb.AppendLine(GetValueString(vd));
                    else
                    {
                        _sb.AppendLine();
                        Build((ContainerDump)propertyDump, level + 1);
                    }
                }

                break;

            case CollectionDump cd:
                for (int i = 0; i < cd.Values.Count; i++)
                {
                    Indent(level).Append('[').Append(i).Append("]: ");
                    if (cd.Values[i] is ValueDump vd)
                        _sb.AppendLine(GetValueString(vd));
                    else
                    {
                        _sb.AppendLine();
                        Build((ContainerDump)cd.Values[i], level + 1);
                    }
                }

                break;

            case DictionaryDump dd:
                BuildDictionary(dd, level);
                break;

            default:
                throw new NotSupportedException($"Unsupported container dump type {dump.GetType()}.");
        }
    }

    private void BuildDictionary(DictionaryDump dd, int level)
    {
        foreach (KeyValuePair<Dump, Dump> kvp in dd.Values)
        {
            if (kvp.Key is ValueDump kvd)
            {
                Indent(level).Append('[').Append(GetValueString(kvd)).Append("]: ");
                if (kvp.Value is ValueDump vvd)
                    _sb.AppendLine(GetValueString(vvd));
                else
                {
                    _sb.AppendLine();
                    Build((ContainerDump)kvp.Value, level + 1);
                }
            }
            else
            {
                Indent(level).AppendLine("[Key]:");
                Build((ContainerDump)kvp.Key, level + 1);
                Indent(level).Append("[Value]: ");
                if (kvp.Value is ValueDump vvd)
                    _sb.AppendLine(GetValueString(vvd));
                else
                {
                    _sb.AppendLine();
                    Build((ContainerDump)kvp.Value, level + 1);
                }
            }
        }
    }

    private StringBuilder Indent(int level)
    {
        for (int i = 0; i < level * _options.IndentSize; i++)
            _sb.Append(_options.IndentChar);
        return _sb;
    }

    private static string GetValueString(ValueDump valueDump) =>
        valueDump.Value is null ? "[NULL]" : valueDump.Value.ToString();
}
