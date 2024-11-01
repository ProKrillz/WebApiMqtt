using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Extensions.TopicTemplate;
using System.Text.Json;
using System.Text;
using Services.DTO;
using Services.Interface;
using Microsoft.Extensions.Logging;


namespace Services.MqttService;

public class MqttClientWorker : BackgroundService
{
    static readonly MqttTopicTemplate sampleTemplate = new("home/temp");

    private readonly IInfluxDBService _influxDBService;

    private readonly ILogger<MqttClientWorker> _logger;
 
    public MqttClientWorker(IInfluxDBService iInfluxDBService, ILogger<MqttClientWorker> logger)
    {
        _influxDBService = iInfluxDBService;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) { } //create more instance

    public async override Task StartAsync(CancellationToken stoppingToken) //creates one instance
    {
        var mqttFactory = new MqttFactory();

        IMqttClient mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
             .WithTcpServer("49987f455bc94d2183a5075a9fa78344.s1.eu.hivemq.cloud", 8883)
             .WithCredentials("MQTT.fx", "linkin")
             .WithProtocolVersion(MqttProtocolVersion.V311)
             .WithTlsOptions(x => x.UseTls())
             .Build();

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Telemetry? found = JsonSerializer.Deserialize<Telemetry>(Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment));
            _logger.LogInformation("random");
            _influxDBService.WriteTelemetry(found);
            return Task.CompletedTask;
        };
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicTemplate(sampleTemplate).Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }
}
