using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JsonSchemaGenerator.Infrastructure.Services;

public class SchemaGenerator(JSGeneratorSettings settings) : ISchemaGenerator
{
    private readonly JSGeneratorSettings _settings = settings;

    public string GenerateAndWrite(object? obj)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, object> GenerateSchema(object? obj)
    {
        if (obj is null)
        {
            return new Dictionary<string, object>
            {
                ["type"] = "null"
            };
        }

        return GenerateSchema(obj.GetType());
    }

    public Dictionary<string, object> GenerateSchema(Type type)
    {
        var schema = new Dictionary<string, object>
        {
            ["type"] = "object"
        };

        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (var prop in type.GetProperties())
        {
            var propSchema = new Dictionary<string, object>();
            var propType = prop.PropertyType;

            // Nullable types
            var underlyingType = Nullable.GetUnderlyingType(propType);
            var actualType = underlyingType ?? propType;

            if (actualType.IsEnum)
            {
                propSchema["type"] = "string";
                propSchema["enum"] = Enum.GetNames(actualType);
            }
            else if (IsPrimitiveJsonType(actualType))
            {
                propSchema["type"] = GetJsonType(actualType);
            }
            else if (actualType.IsClass && actualType != typeof(string))
            {
                propSchema = GenerateSchema(actualType);
            }

            properties[prop.Name] = propSchema;

            // Required logic
            if (IsRequired(prop))
            {
                required.Add(prop.Name);
            }
        }

        schema["properties"] = properties;

        if (required.Count > 0)
        {
            schema["required"] = required;
        }

        return schema;
    }

    private bool IsPrimitiveJsonType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    private string GetJsonType(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return "integer";
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
        if (type == typeof(DateTime)) return "string"; // with format, if needed
        return "string"; // fallback
    }

    static bool IsRequired(PropertyInfo prop)
    {
        var type = prop.PropertyType;

        // Nullable value type
        if (Nullable.GetUnderlyingType(type) != null)
            return false;

        // Check nullable reference type via NullableAttribute
        if (!type.IsValueType)
        {
            // Look for [Nullable] or [NullableContext] attributes
            var nullableAttr = prop.GetCustomAttribute<NullableAttribute>();
            if (nullableAttr != null && nullableAttr.NullableFlags.Length > 0)
            {
                return nullableAttr.NullableFlags[0] == 1; // 1 = NOT nullable, 2 = nullable
            }

            var contextAttr = prop.DeclaringType?.GetCustomAttribute<NullableContextAttribute>();
            if (contextAttr != null)
            {
                return contextAttr.Flag == 1; // 1 = not nullable
            }

            // Default to required if no metadata
            return true;
        }

        // Value types (non-nullable)
        return true;
    }
}