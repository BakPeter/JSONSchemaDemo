using JsonSchemaGenerator.Core.Interfaces;
using System.Reflection;
using System.Text.Json;

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
        var car = new Car
        {
            Make = "Ford", Model = "Focus", Year = 2020,
            Dictionary = new Dictionary<string, object> { { "name", "eli" }, { "age", 50 } },
            DictionaryNullable = new Dictionary<string, object>
                { { "name", "eli" }, { "age", 50 }, { "engine", new Engine() } }
        };
        //var car = new Car();
        //var schema = _schemaGenerator.Generate(car);
        //var schemaStr = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
        var (path1, schema1) = _schemaGenerator.GenerateAndWrite(car);
        _logger.LogInformation($"Path: {path1}\nSchema:\n{schema1}");

        var (path2, schema2) = _schemaGenerator.GenerateAndWrite(null);
        _logger.LogInformation($"Path: {path2}\nSchema:\n{schema2}");

        //Car myCar = new Car
        //{
        //    CarEngine = new Engine
        //    {
        //        Type = "V8",
        //        Horsepower = 450
        //    }
        //};

        //// Get the type of the Car instance
        //Type carType = myCar.GetType();

        //// Get the CarEngine property info
        //PropertyInfo carEngineProp = carType.GetProperty("CarEngine");

        //// Get the Engine instance from the Car object
        //object engineInstance = carEngineProp?.GetValue(myCar);

        //if (engineInstance != null)
        //{
        //    Type engineType = engineInstance.GetType();
        //    PropertyInfo[] engineProps = engineType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (var prop in engineProps)
        //    {
        //        object value = prop.GetValue(engineInstance);
        //        Console.WriteLine($"Engine Property: {prop.Name}, Value: {value}, Type: {prop.PropertyType.Name}");
        //    }
        //}

        return Task.CompletedTask;
    }
}

public class Car
{
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int? Year { get; set; }
    public CarColor Color { get; set; }
    public CarType? CarType { get; set; }
    public Engine CarEngine { get; set; }

    public Engine? CarEngineNullable { get; set; }

    public List<Engine> CarEnginesList { get; set; }
    public List<Engine>? CarEnginesListNullable { get; set; }
    public Engine[] CarEnginesArray { get; set; }
    public Engine[]? CarEnginesArrayNullable { get; set; }
    public double[] DoubleNumbersArray { get; set; }
    public double[]? DoubleNumbersArrayNullable { get; set; }
    public Guid CarUGuid { get; set; }
    public Guid? CarUGuidNullable { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime? DateTimeNullable { get; set; }
    public Uri Uri { get; set; }
    public Uri? UriNullable { get; set; }
    public DateOnly DateOnly { get; set; }
    public DateOnly? DateOnlyNullable { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public TimeOnly? TimeOnlyNullable { get; set; }
    public string PersonalEmail { get; set; }
    public string WorkEmail { get; set; }
    public string? EmailNullable { get; set; }
    public Dictionary<string, object> Dictionary { get; set; }
    public Dictionary<string, object>? DictionaryNullable { get; set; }
}

public enum CarColor
{
    White,
    Black,
    Red,
    Green
}

public enum CarType
{
    Sports,
    Suv,
    Family,
}

public class Engine
{
    public string? Type { get; set; } = null!;
    public int Horsepower { get; set; }
}