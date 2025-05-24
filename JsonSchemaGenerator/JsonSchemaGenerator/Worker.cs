using JsonSchemaGenerator.Core.Interfaces;

namespace JsonSchemaGenerator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISchemaGenerator _schemaGenerator;

    public Worker(ILogger<Worker> logger, ISchemaGenerator schemaGenerator)
    {
        _logger = logger;
        _schemaGenerator = schemaGenerator;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var obj = new Dictionary<string, object>();
        var schema = _schemaGenerator.Generate(obj);
        return Task.CompletedTask;
    }
}