#nullable enable

using System.Text.Json;
using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using JsonSchemaGenerator.Infrastructure.Services;

namespace JsonSchemaGenerator.UnitTests;

[TestFixture]
public class SchemaGeneratorTests
{
    private ISchemaGenerator _generator;
    private readonly List<ValidTestCase> _validTestCases;

    public SchemaGeneratorTests()
    {
        _validTestCases = new List<ValidTestCase>();
        _validTestCases.Add(new ValidTestCase
            { Description = "Schema for null object", Obj = null, JsonSchema = "{\"type\":\"null\"}" });
        //_validTestCases.Add(new ValidTestCase
        //    { Description = "Schema for null object", Obj = null, JsonSchema = "{ \"type\":\"string\"}" });
    }

    [SetUp]
    public void Setup()
    {
        _generator = new SchemaGenerator(new JSGeneratorSettings());
    }

    [TearDown]
    public void TearDown()
    {
    }


    [Test]
    public void Generate_InputObjectInstance_ReturnsObjectJsonSchema()
    {
        var failedTests = new List<string>();
        var passedTestsCounter = 0;
        for (var i = 0; i < _validTestCases.Count; i++)
        {
            // Arrange
            var testCase = _validTestCases[i];
            var instance = testCase.Obj;
            var expectedSchema = testCase.JsonSchema;

            // Act
            var schema = _generator.Generate(instance);
            var actualSchema = JsonSerializer.Serialize(schema);

            // Assert - Aggregate Failures
            if (actualSchema.Equals(expectedSchema))
                passedTestsCounter++;
            else
                failedTests.Add($"{i}: {testCase.Description}, expected: '{expectedSchema}', actual: '{actualSchema}'");
        }

        // Assert
        if (failedTests.Count == 0)
            Assert.Pass($"{passedTestsCounter}/{_validTestCases.Count} Tests Passed");
        else
        {
            var result =
                $"{passedTestsCounter}/{_validTestCases.Count} Tests Passed\nFailedTests:\n{string.Join("\n", failedTests)}";
            Assert.Fail(result);
        }
    }
}

internal class ValidTestCase
{
    public string Description { get; set; }
    public object? Obj { get; set; }
    public string JsonSchema { get; set; }
}