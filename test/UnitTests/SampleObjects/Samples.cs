namespace Jeevan.ObjectDumper.UnitTests.SampleObjects;

public static class Samples
{
    public static Person John() => new()
    {
        Id = Guid.NewGuid(),
        Name = "John",
        BirthDate = new DateTime(1980, 5, 15, 0, 0, 0, DateTimeKind.Local),
        Hobbies = new List<string> { "Reading", "Traveling" },
        Address = null, // Simulate a null property
        IsActive = true,
        Salary = 50000.50m,
        Breakup = new SalaryBreakup(40000m, 10000m),
        DistanceToOffice = "3km",
        Scores = new Dictionary<string, int> { { "Math", 95 }, { "Science", 88 } },
        Spouse = Jane(),
    };

    public static Person Jane() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Jane",
        BirthDate = new DateTime(1982, 8, 20, 0, 0, 0, DateTimeKind.Local),
        Hobbies = null, // Simulate a null property
        Address = new Address
        {
            Street = "123 Main St",
            City = "Cityville",
            ZipCode = "12345",
        },
        IsActive = false,
        Salary = 60000.75m,
        Breakup = new SalaryBreakup(45000m, 15000m),
        DistanceToOffice = new Distance(4.8d, DistanceUnits.Kilometer),
        Scores = null, // Simulate a null property
    };

    public static IDictionary<Distance, string> PlaceDistances() => new Dictionary<Distance, string>
    {
        ["60km"] = "Nandi Hills",
        ["150km"] = "Mysuru",
        ["341 m"] = "Kottayam",
    };

    public static IDictionary<Distance, Distance> DistanceConversions() => new Dictionary<Distance, Distance>
    {
        ["60km"] = new(37.2822715d, DistanceUnits.Mile),
        ["150km"] = new(93.2056788d, DistanceUnits.Mile),
        ["550km"] = new(341.754156d, DistanceUnits.Mile),
    };
}
