using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace WebApiMqtt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttController : ControllerBase
    {
        private readonly ILogger<MqttController> _logger;

        private readonly IMqttClientPublish _mqttSerive;

        private readonly IInfluxDBService _influxDBService;

        public MqttController(ILogger<MqttController> logger, IMqttClientPublish service, IInfluxDBService influxDbService)
        {
            _logger = logger;  
            _mqttSerive = service;
            _influxDBService = influxDbService;
        }
        [HttpPost, Route("send/{number}")]
        public void SendCommand(string number)
        {
            _mqttSerive.Publish_Application_Message(number);
        }
        [HttpGet, Route("/telemetry")]
        public async Task<List<Telemetry>> GetAllTelemetry()
        {
            return await _influxDBService.QuereDB();
        }


    }
}
