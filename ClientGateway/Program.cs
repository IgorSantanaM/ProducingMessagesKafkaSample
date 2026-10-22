using ClientGateway;
using ClientGateway.Controllers;
using ClientGateway.Models;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("KafkaConfig"));
builder.Services.Configure<SchemaRegistryConfig>(builder.Configuration.GetSection("SchemaConfig"));

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp
        .GetRequiredService<IOptions<SchemaRegistryConfig>>()
        .Value;

    return new CachedSchemaRegistryClient(config);
});

builder.Services.AddSingleton<IProducer<string, Biometrics>>(sp =>
{
    var producerConfig = sp.GetRequiredService<IOptions<ProducerConfig>>().Value;
    var schemaConfig = sp.GetRequiredService<ISchemaRegistryClient>();

    var serializerConfig = new JsonSerializerConfig
    {
        AutoRegisterSchemas = true
    };

    return new ProducerBuilder<string, Biometrics>(producerConfig)
        .SetValueSerializer(new JsonSerializer<Biometrics>
            (schemaConfig, serializerConfig)
        )
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();