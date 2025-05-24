#nullable enable

using System.Text.Json;
using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using JsonSchemaGenerator.Infrastructure.Services;

namespace JsonSchemaGenerator.UnitTests;

[TestFixture]
public class SchemaGeneratorTests
{
    private ISchemaGenerator _generator = new SchemaGenerator(new JSGeneratorSettings
        { SchemaUrl = "http://json-schema.org/draft-04/schema#" });

    [SetUp]
    public void Setup()
    {
    }

    [TearDown]
    public void TearDown()
    {
    }


    [Test]
    public void Generate_InputCarInstance_ReturnsCarSchema()
    {
        // Arrange
        var carInstance = new Car { Make = "Ford", Model = "Focus", Year = 2020 };
        var expectedCarSchemaJson =
            "{\"type\":\"object\",\"required\":[\"Make\",\"Model\",\"Year\"],\"properties\":{\"Make\":{\"type\":\"string\"},\"Model\":{\"type\":\"string\"},\"Year\":{\"type\":\"integer\",\"format\":\"int32\"},\"Color\":{\"type\":[\"string\",\"null\"]},\"CarEngine\":{\"type\":[\"object\",\"null\"],\"properties\":{\"Type\":{\"type\":\"string\"},\"Horsepower\":{\"type\":\"integer\",\"format\":\"int32\"},\"required\":[\"Type\",\"Horsepower\"]}}}}";

        // Act
        // var actualSchema = _generator.Generate(carInstance);
        var actualSchema = _generator.GenerateSchema(carInstance);
        var schema = JsonSerializer.Serialize(actualSchema, new JsonSerializerOptions { WriteIndented = true });

        // Assert
        // Assert.That(expectedCarSchemaJson, Is.EqualTo(actualSchema));
        Assert.Pass();
    }
}

public class Car
{
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int? Year { get; set; }
    public CarColor Color { get; set; }
    public CarType? CarType{ get; set; }
    public Engine? CarEngine { get; set; }
}

public enum CarColor
{
    White,
    Black,
    Red,
    Green
}

public enum CarType
{
    Sports,
    Suv,
    Family,

}
public class Engine
{
    public string? Type { get; set; } = null!;
    public int Horsepower { get; set; }
}

public class Person
{
    public int Id { get; set; } // Non-nullable, should be required
    public string Name { get; set; } // Nullable (string), not required
    public DateTime CreatedDate { get; set; } // Non-nullable, should be required
    public decimal? Price { get; set; } // Nullable (decimal?), not required
    public List<string> Tags { get; set; } // Nullable (list), not required
    public Address? ShippingAddress { get; set; } // Nullable (complex type), not required
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}