using JsonSchemaGenerator;
using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Infrastructure;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services )=>
    {
        var settings = context.Configuration.GetSection("JSGeneratorSettings").Get<JSGeneratorSettings>();
        services.AddJsonSchemaGeneratorServices(settings!);

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
