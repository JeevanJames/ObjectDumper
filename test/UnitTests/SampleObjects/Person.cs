namespace Jeevan.ObjectDumper.UnitTests.SampleObjects;

public class Person
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime? BirthDate { get; set; }

    public List<string>? Hobbies { get; set; }

    public Address? Address { get; set; }

    public bool IsActive { get; set; }

    public decimal Salary { get; set; }

    public SalaryBreakup Breakup { get; set; }

    public Distance DistanceToOffice { get; set; }

    public Dictionary<string, int>? Scores { get; set; }

    public Person? Spouse { get; set; }
}

public class Address
{
    public string? Street { get; set; }

    public string? City { get; set; }

    public string? ZipCode { get; set; }
}

public struct SalaryBreakup
{
    public SalaryBreakup(decimal basic, decimal bonus)
    {
        Basic = basic;
        Bonus = bonus;
    }

    public decimal Basic { get; }

    public decimal Bonus { get; }
}
