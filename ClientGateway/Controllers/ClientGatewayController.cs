using System.Diagnostics.Metrics;
using System.Net;
using ClientGateway.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ClientGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientGatewayController : ControllerBase
{
    private readonly ILogger<ClientGatewayController> _logger;
    private readonly IProducer<string, Biometrics> _producer;

    public ClientGatewayController(ILogger<ClientGatewayController> logger, IProducer<string, Biometrics> producer)
    {
        _logger = logger;
        _producer = producer;
        logger.LogInformation("ClientGatewayController is Active.");
    }

    [HttpGet("Hello")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public string Hello()
    {
        _logger.LogInformation("Hello World");
        return "Hello World";
    }

    [HttpGet("RecordMeasurements")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<DeliveryResult<string, Biometrics>> RecordMeasurements(Biometrics biometrics)
    {
        var message = new Message<string, Biometrics>()
        {
            Key = biometrics.DeviceId.ToString(),
            Value = biometrics
        };
        var produced =  await _producer.ProduceAsync($"BiometricsImported", message);
        return produced;
    }
}



