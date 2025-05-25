using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JsonSchemaGenerator.Infrastructure.Services;

public class SchemaGenerator: ISchemaGenerator
{
    private readonly JSGeneratorSettings _settings;

    public SchemaGenerator(JSGeneratorSettings settings)
    {
        _settings = settings;
    }

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

    private bool IsRequired(PropertyInfo prop)
    {
        var type = prop.PropertyType;

        // Value types
        if (Nullable.GetUnderlyingType(type) != null)
            return false; // nullable value type (e.g., int?)

        if (type.IsValueType)
            return true; // non-nullable value type (e.g., int)

        // Reference types: check if marked nullable (e.g., string?)
        var nullable = prop.CustomAttributes
            .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

        if (nullable != null)
        {
            if (nullable.ConstructorArguments.Count == 1)
            {
                var arg = nullable.ConstructorArguments[0];
                if (arg.ArgumentType == typeof(byte))
                {
                    return (byte)arg.Value == 1; // 1 = not nullable, 2 = nullable
                }

                if (arg.ArgumentType == typeof(byte[]) && arg.Value is IReadOnlyCollection<CustomAttributeTypedArgument> values)
                {
                    var flag = (byte)values.First().Value!;
                    return flag == 1;
                }
            }
        }

        // Fall back to NullableContextAttribute on declaring type
        var nullableContext = prop.DeclaringType?
            .CustomAttributes
            .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");

        if (nullableContext != null && nullableContext.ConstructorArguments.Count == 1)
        {
            var contextFlag = (byte)nullableContext.ConstructorArguments[0].Value!;
            return contextFlag == 1; // 1 = not nullable, 2 = nullable
        }

        // Assume required if no nullable info found
        return true;
    }
}