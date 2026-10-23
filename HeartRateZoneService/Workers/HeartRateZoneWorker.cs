
using ClientGateway.Models;
using Confluent.Kafka;
using HeartRateZoneService.Domain;

namespace HeartRateZoneService.Workers;

public class HeartRateZoneWorker : BackgroundService
{
    private readonly ILogger<HeartRateZoneWorker> _logger;
    private readonly IConsumer<string, Biometrics> _consumer;
    private readonly IProducer<string, HeartRateZoneReached> _producer;

    private const string InputTopic = "Biometrics";
    private const string OutputTopic = "Reached01";

    public HeartRateZoneWorker(
        IConsumer<string, Biometrics> consumer,
        IProducer<string, HeartRateZoneReached> producer,
        ILogger<HeartRateZoneWorker> logger)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;

        _consumer.Subscribe(InputTopic);
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _producer.InitTransactions(TimeSpan.FromSeconds(30));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);

                await HandleMessage(result, stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            _consumer.Close();
        }
    }

    protected virtual async Task HandleMessage(
        ConsumeResult<string, Biometrics> result,
        CancellationToken stoppingToken)
    {
        _producer.BeginTransaction();

        try
        {
            var message = result.Message.Value;

            if (message?.HeartRate != null)
            {
                foreach (var heartRate in message.HeartRate)
                {
                    var reached = new HeartRateZoneReached(
                        message.DeviceId,
                        heartRate.GetHeartRateZone(heartRate.Value),
                        DateTime.UtcNow,
                        heartRate.Value,
                        message.MaxHeartRate
                    );

                    await _producer.ProduceAsync(
                        OutputTopic,
                        new Message<string, HeartRateZoneReached>
                        {
                            Key = message.DeviceId.ToString(),
                            Value = reached
                        },
                        stoppingToken
                    );
                }
            }

            var offsets = new[]
            {
                new TopicPartitionOffset(
                    result.TopicPartition,
                    result.Offset + 1
                )
            };

            _producer.SendOffsetsToTransaction(
                offsets,
                _consumer.ConsumerGroupMetadata,
                TimeSpan.FromSeconds(10)
            );

            _producer.CommitTransaction();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Transaction failed for offset {Offset}",
                result.Offset
            );

            _producer.AbortTransaction();
            throw;
        }
    }
}
