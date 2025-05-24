using JsonSchemaGenerator;
using JsonSchemaGenerator.Core.Configurations;
using JsonSchemaGenerator.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var settings = builder.Configuration.GetSection("JSGeneratorSettings").Get<JSGeneratorSettings>();
builder.Services.AddJsonSchemaGeneratorServices(settings!);

var host = builder.Build();
host.Run();
