using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Extensions.TopicTemplate;
using System.Text.Json;
using System.Text;
using Services.DTO;


namespace Services.MqttService;

public class MqttClientWorker : BackgroundService {

    static readonly MqttTopicTemplate sampleTemplate = new("home/temp");

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) { } //create more instance

    public async override Task StartAsync(CancellationToken stoppingToken) //creates one instance
    {
        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("49987f455bc94d2183a5075a9fa78344.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("MQTT.fx", "linkin")
                .WithProtocolVersion(MqttProtocolVersion.V310)
                .WithTlsOptions(x => x.UseTls())
                .Build();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application message.");

                Telemetry? found = JsonSerializer.Deserialize<Telemetry>(Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment));
                Console.WriteLine(found?.Humidity);
                Console.WriteLine(found?.Temperature);
                return Task.CompletedTask;
            };

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicTemplate(sampleTemplate).Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }

}
