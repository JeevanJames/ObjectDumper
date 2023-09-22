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
        Scores = null, // Simulate a null property
    };
}
