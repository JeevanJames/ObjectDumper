using Jeevan.ObjectDumper.UnitTests.SampleObjects;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace Jeevan.ObjectDumper.UnitTests;

public sealed class DumperTests
{
    public DumperTests(ITestOutputHelper output)
    {
        Output = output;
    }

    public ITestOutputHelper Output { get; }

    [Fact]
    public void PersonDumpTest()
    {
        Person john = Samples.John();
        Dump dump = Dumper.Dump(john);
        dump.ShouldNotBeNull();
    }

    [Fact]
    public void PersonStringDumpTest()
    {
        Person john = Samples.John();
        string dump = Dumper.DumpString(john);
        dump.ShouldNotBeNull();

        Output.WriteLine(dump);
    }
}
