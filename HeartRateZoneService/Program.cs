using HeartRateZoneService;
using HeartRateZoneService.Workers;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Confluent.Kafka.SyncOverAsync;
using ClientGateway.Models;
using HeartRateZoneService.Domain;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<HeartRateZoneWorker>();

        services.Configure<ConsumerConfig>(
            hostContext.Configuration.GetSection("kafkaConfigConsumer")
        );

        services.Configure<SchemaRegistryConfig>(hostContext.Configuration.GetSection("SchemaConfig"));

        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>().Value;

            return new CachedSchemaRegistryClient(config);
        });


        services.AddSingleton<IProducer<string, HeartRateZoneReached>>(sp =>
        {
            var schema = sp.GetRequiredService<ISchemaRegistryClient>();

            var producerOptions = sp.GetRequiredService<IOptions<ProducerConfig>>().Value;

            var serializerConfig = new JsonSerializerConfig
            {
                AutoRegisterSchemas = true
            };

            return new ProducerBuilder<string, HeartRateZoneReached>(producerOptions)
            .SetValueSerializer(new JsonSerializer<HeartRateZoneReached>(schema, serializerConfig))
            .Build();
        });
        services.AddSingleton<IConsumer<string, Biometrics>>(sp =>
        {
            var consumerOptions = sp.GetRequiredService<IOptions<ConsumerConfig>>().Value;

            return new ConsumerBuilder<string, Biometrics>(consumerOptions)
            .SetValueDeserializer(new JsonDeserializer<Biometrics>().AsSyncOverAsync())
            .Build();
        });
    })
    .Build();

await host.RunAsync();

