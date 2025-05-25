namespace JsonSchemaGenerator.Core.Interfaces;

public interface ISchemaGenerator
{
    (string, string) GenerateAndWrite(object? obj);
    Dictionary<string, object> Generate(object? obj);
}