using System.Collections;
using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System;

namespace JsonSchemaGenerator.Infrastructure.Services;

public class SchemaGenerator : ISchemaGenerator
{
    private readonly JSGeneratorSettings _settings;

    public SchemaGenerator(JSGeneratorSettings settings)
    {
        _settings = settings;
    }

    public (string, string) GenerateAndWrite(object? obj)
    {
        var schema = Generate(obj);

        var fileName = GetFileName(schema);
        var path = Path.Combine(_settings.TergetPath, fileName);

        var schemaStr = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, schemaStr);
        return (path, schemaStr);
    }

    private string GetFileName(Dictionary<string, object> schema)
    {
        schema.TryGetValue("title", out var title);
        return $"{title ?? "NullSchema"}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_jsonschema.json";
    }

    public Dictionary<string, object> Generate(object? obj)
    {
        if (obj is null)
        {
            return GetNullSchema();
        }

        return GenerateJsonSchema(obj.GetType(), obj, true);
    }


    private Dictionary<string, object> GenerateJsonSchema(Type type, object? instance, bool isRoot)
    {
        var schema = new Dictionary<string, object>();

        if (isRoot)
        {
            schema["$schema"] = "https://json-schema.org/draft/2020-12/schema";
            schema["title"] = type.Name;
        }

        schema["type"] = "object";

        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propType = prop.PropertyType;

            Dictionary<string, object> propSchema;

            if (IsDictionary(propType, out var keyType, out var valueType))
            {
                if (keyType == typeof(string) && valueType == typeof(object))
                {
                    var propValue = instance == null ? null : prop.GetValue(instance);

                    if (propValue is Dictionary<string, object?> dict && dict.Count > 0)
                    {
                        var dictProperties = new Dictionary<string, object>();
                        var dictRequired = new List<string>();
                        foreach (var kvp in dict)
                        {
                            var kvpValueType = kvp.Value?.GetType();
                            var valSchema = GenerateSchemaForType(kvpValueType);
                            dictProperties[kvp.Key] = valSchema;
                            dictRequired.Add(kvp.Key);
                        }

                        propSchema = new Dictionary<string, object>
                        {
                            ["type"] = "object",
                            ["properties"] = dictProperties,
                            ["required"] = dictRequired
                        };
                    }
                    else
                    {
                        // Empty or null dictionary => empty properties
                        propSchema = new Dictionary<string, object>
                        {
                            ["type"] = "object",
                            ["properties"] = new Dictionary<string, object>()
                        };
                    }
                }
                else
                {
                    propSchema = new Dictionary<string, object>
                    {
                        ["type"] = "object",
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["key"] = GenerateSchemaForType(keyType),
                            ["value"] = GenerateSchemaForType(valueType)
                        },
                        ["required"] = new[] { "key", "value" }
                    };
                }
            }
            else if (IsCollection(propType, out var elementType))
            {
                propSchema = new Dictionary<string, object>
                {
                    ["type"] = "array",
                    ["items"] = GenerateSchemaForType(elementType)
                };
            }
            else
            {
                propSchema = GenerateSchemaForType(propType);
            }

            properties[prop.Name] = propSchema;

            if (IsRequired(prop))
                required.Add(prop.Name);
        }

        schema["properties"] = properties;
        if (required.Count > 0)
            schema["required"] = required;

        return schema;
    }

    private Dictionary<string, object> GetNullSchema()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "null"
        };
    }


    private Dictionary<string, object> GenerateSchemaForObject(object? obj)
    {
        if (obj == null)
            return GetNullSchema();

        return GenerateJsonSchema(obj.GetType(), obj, false);
    }

    private bool IsDictionary(Type type, out Type? keyType, out Type? valueType)
    {
        keyType = null!;
        valueType = null!;

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            if (def == typeof(Dictionary<,>) || def == typeof(IDictionary<,>))
            {
                var args = type.GetGenericArguments();
                keyType = args[0];
                valueType = args[1];
                return true;
            }
        }

        return false;
    }

    private Dictionary<string, object> GenerateSchemaForType(Type? type)
    {
        if (type is null) return GetNullSchema();

        var schema = new Dictionary<string, object>();
        var nullableUnderlying = Nullable.GetUnderlyingType(type);
        var actualType = nullableUnderlying ?? type;

        if (actualType.IsEnum)
        {
            schema["type"] = "string";
            schema["enum"] = Enum.GetNames(actualType);
        }
        else if (IsJsonPrimitive(actualType))
        {
            schema["type"] = GetJsonType(actualType);

            // Add JSON format where appropriate
            if (actualType == typeof(DateTime)) schema["format"] = "date-time";
            else if (actualType == typeof(Guid)) schema["format"] = "uuid";
            else if (actualType == typeof(Uri)) schema["format"] = "uri";
            else if (actualType == typeof(DateOnly)) schema["format"] = "date";
            else if (actualType == typeof(TimeOnly)) schema["format"] = "time";
            else if (actualType == typeof(string) && actualType.Name.ToLower().Contains("email"))
                schema["format"] = "email";
        }
        else if (IsCollection(actualType, out var elementType))
        {
            schema["type"] = "array";
            schema["items"] = GenerateSchemaForType(elementType);
        }
        else if (actualType.IsClass)
        {
            schema = GenerateJsonSchema(actualType, null, false); // recursion (no loop protection)
        }

        return schema;
    }

    private bool IsJsonPrimitive(Type type)
    {
        return type.IsPrimitive || type == typeof(string)
                                || type == typeof(decimal) || type == typeof(DateTime)
                                || type == typeof(Guid) || type == typeof(Uri)
                                || type == typeof(DateOnly) || type == typeof(TimeOnly);
    }

    private string GetJsonType(Type type)
    {
        if (type == typeof(string) || type == typeof(Guid) || type == typeof(Uri)) return "string";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return "integer";
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
        if (type == typeof(DateTime) || type == typeof(DateOnly) || type == typeof(TimeOnly)) return "string";
        return "string";
    }

    private bool IsCollection(Type type, out Type? elementType)
    {
        elementType = null;

        if (type.IsArray)
        {
            elementType = type.GetElementType();
            return true;
        }

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            if (def == typeof(List<>) || def == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        return false;
    }

    private bool IsRequired(PropertyInfo prop)
    {
        var type = prop.PropertyType;
        // Nullable<T> (e.g., int?)
        if (Nullable.GetUnderlyingType(type) != null)
            return false;

        // Reference types (string, class, etc.)
        if (!type.IsValueType)
        {
            var nullableAttr = prop.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

            if (nullableAttr != null && nullableAttr.ConstructorArguments.Count > 0)
            {
                var arg = nullableAttr.ConstructorArguments[0];
                if (arg.ArgumentType == typeof(byte))
                    return (byte)arg.Value == 1;

                if (arg.ArgumentType == typeof(byte[]) &&
                    arg.Value is IReadOnlyCollection<CustomAttributeTypedArgument> values)
                    return (byte)values.First().Value! == 1;
            }

            var contextAttr = prop.DeclaringType?
                .CustomAttributes
                .FirstOrDefault(a =>
                    a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");

            if (contextAttr != null && contextAttr.ConstructorArguments.Count > 0)
                return (byte)contextAttr.ConstructorArguments[0].Value! == 1;

            return true; // assume required if no metadata
        }

        return true; // non-nullable value types
    }
}