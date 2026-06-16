// /src/LifeAdvisor.Domain/ValueObjects/Address.cs
using System;

namespace LifeAdvisor.Domain.ValueObjects;

public record Address
{
    public string City { get; init; }
    public string Country { get; init; }

    private Address()
    {
        City = string.Empty;
        Country = string.Empty;
    }

    public Address(string city, string country)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        City = city.Trim();
        Country = country.Trim();
    }

    // Optional: A helper to format it back to the "City, Country" string for the frontend
    public override string ToString() => $"{City}, {Country}";
}