namespace EagleBank.Domain;

public sealed class Address
{
    public string Line1 { get; private set; } = null!;

    public string? Line2 { get; private set; }

    public string? Line3 { get; private set; }

    public string Town { get; private set; } = null!;

    public string County { get; private set; } = null!;

    public string Postcode { get; private set; } = null!;

    private Address()
    {
    }

    public Address(string line1, string? line2, string? line3, string town, string county, string postcode)
    {
        Line1 = line1;
        Line2 = line2;
        Line3 = line3;
        Town = town;
        County = county;
        Postcode = postcode;
    }
}
