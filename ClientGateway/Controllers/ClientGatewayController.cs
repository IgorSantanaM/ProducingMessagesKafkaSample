using System.Diagnostics.Metrics;
using System.Net;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ClientGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientGatewayController : ControllerBase
{
    private readonly ILogger<ClientGatewayController> _logger;
    private readonly IProducer<string, string> _producer;

    public ClientGatewayController(ILogger<ClientGatewayController> logger, IProducer<string, string> producer)
    {
        _logger = logger;
        _producer = producer;
        logger.LogInformation("ClientGatewayController is Active.");
    }

    [HttpGet("Hello")]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public string Hello()
    {
        _logger.LogInformation("Hello World");
        return "Hello World";
    }

    [HttpGet("RecordMeasurements")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<string> RecordMeasurements(Message<string, string> message)
    {
        var produced =  await _producer.ProduceAsync($"{message.Key}", message);
        return produced.Value;
    }
}



