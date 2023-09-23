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

    [Theory]
    [MemberData(nameof(GetDumpTestObjects))]
    public void PersonDumpTest(object instance)
    {
        Dump dump = Dumper.Dump(instance);
        dump.ShouldNotBeNull();
    }

    [Theory]
    [MemberData(nameof(GetDumpTestObjects))]
    public void StringDumpTests(object instance)
    {
        string dump = Dumper.DumpString(instance, b => b.IndentWith(2, '·'));
        dump.ShouldNotBeNullOrWhiteSpace();

        Output.WriteLine(dump);
    }

    public static IEnumerable<object[]> GetDumpTestObjects()
    {
        yield return new object[] { Samples.John() };
        yield return new object[] { Samples.PlaceDistances() };
        yield return new object[] { Samples.DistanceConversions() };
    }
}
