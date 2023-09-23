namespace Jeevan.ObjectDumper;

public sealed class StringDumpOptionsBuilder
{
    private readonly StringDumpOptions _options = new();

    public StringDumpOptionsBuilder IndentWith(int size = 2, char ch = ' ')
    {
        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size), size, "Indent size cannot be less than one.");
        if (char.IsControl(ch))
            throw new ArgumentOutOfRangeException(nameof(ch), ch, "Indent char is not printable.");

        _options.IndentSize = size;
        _options.IndentChar = ch;

        return this;
    }

    internal StringDumpOptions Build() => _options;
}

internal sealed class StringDumpOptions
{
    internal int IndentSize { get; set; } = 2;

    internal char IndentChar { get; set; } = ' ';
}
