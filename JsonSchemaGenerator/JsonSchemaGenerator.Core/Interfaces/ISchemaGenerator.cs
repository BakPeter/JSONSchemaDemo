namespace JsonSchemaGenerator.Core.Interfaces;

public interface ISchemaGenerator
{
    string GenerateAndWrite(object? obj);
    Dictionary<string, object> GenerateSchema(object? obj);
}