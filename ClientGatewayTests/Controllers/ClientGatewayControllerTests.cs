namespace ClientGatewayTests.Controllers;

using Moq;
using Confluent.Kafka;
using ClientGateway.Controllers;
using Microsoft.Extensions.Logging;
using ClientGateway.Models;

public class ClientGatewayControllerTests
{
    private Logger<ClientGatewayController> _logger;
    private Mock<IProducer<string, Biometrics>> _mockProducer;
    private ClientGatewayController _controller;

    [SetUp]
    public void Setup()
    {
        _logger = new Logger<ClientGatewayController>(new LoggerFactory());
        _mockProducer = new Mock<IProducer<string, Biometrics>>();
        _controller = new ClientGatewayController(_logger, _mockProducer.Object);
    }

    [Test]
    public async Task RecordMeasurements_ShouldProduceTheExpectedMessage()
    {
        var expectedTopic = "BiometricsImported";
        var expectedMessage = TestHelpers.createMessage();

        _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, Biometrics>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new DeliveryResult<string, Biometrics>
            {
                Message = expectedMessage
            }));

        var result = await _controller.RecordMeasurements(expectedMessage.Value);

        _mockProducer.Verify(producer => producer.ProduceAsync(
            expectedTopic,
            It.Is<Message<string, Biometrics>>(msg => msg.Key == expectedMessage.Key && msg.Value == expectedMessage.Value),
            It.IsAny<CancellationToken>()));

        _mockProducer.Verify(producer => producer.Flush(It.IsAny<CancellationToken>()), Times.Once());

        Assert.That(result.Value, Is.EqualTo(expectedMessage.Value));
    }

    [Test]
    public void RecordMeasurements_ShouldReturnAFailure_IfTheMessageProducerFails()
    {
        var expectedTopic = "BiometricsImported";
        var expectedMessage = TestHelpers.createMessage();

        _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, Biometrics>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromException<DeliveryResult<string, Biometrics>>(new Exception("Boom")));

        Assert.ThrowsAsync<Exception>(() => _controller.RecordMeasurements(expectedMessage.Value));

        _mockProducer.Verify(producer => producer.ProduceAsync(
            expectedTopic,
            It.Is<Message<string, Biometrics>>(msg => msg.Key == expectedMessage.Key && msg.Value == expectedMessage.Value),
            It.IsAny<CancellationToken>()));
    }
}

