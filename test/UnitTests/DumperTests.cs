using Jeevan.ObjectDumper.UnitTests.SampleObjects;

using Shouldly;

using Xunit;

namespace Jeevan.ObjectDumper.UnitTests;

public sealed class DumperTests
{
    [Fact]
    public void PersonDumpTest()
    {
        Person john = Samples.John();
        ContainerDump node = Dumper.Dump(john);
        node.ShouldNotBeNull();
    }
}
