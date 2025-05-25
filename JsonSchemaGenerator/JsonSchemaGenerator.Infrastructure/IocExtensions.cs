using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Core.Interfaces;
using JsonSchemaGenerator.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JsonSchemaGenerator.Infrastructure;

public static class IocExtensions
{
    public static void AddJsonSchemaGeneratorServices(this IServiceCollection services,
        JSGeneratorSettings settings)
    {
        services.AddSingleton(settings);
        services.AddSingleton<ISchemaGenerator, SchemaGenerator>();
    }
}