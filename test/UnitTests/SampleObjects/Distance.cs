using System.Text.RegularExpressions;

namespace Jeevan.ObjectDumper.UnitTests.SampleObjects;

public readonly partial record struct Distance(double Quantity, DistanceUnits Units)
{
    public static implicit operator Distance(double quantity) => new(quantity, DistanceUnits.Kilometer);

    public static implicit operator double(Distance distance) => distance.Quantity;

    public static implicit operator Distance(string value)
    {
        Match match = GetDistancePattern().Match(value);
        if (match.Success)
        {
            DistanceUnits units = match.Groups[2].Value == "km" ? DistanceUnits.Kilometer : DistanceUnits.Mile;
            return new Distance(double.Parse(match.Groups[1].Value), units);
        }

        return new Distance(-1, DistanceUnits.Kilometer);
    }

    [GeneratedRegex(@"^(\d+)\s?(k?m)$")]
    private static partial Regex GetDistancePattern();

    public override string ToString() =>
        $"{Quantity} {(Units == DistanceUnits.Kilometer ? "KMS" : "Miles")}";
}

public enum DistanceUnits
{
    Kilometer,
    Mile,
}
