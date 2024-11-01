using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace WebApiMqtt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttController : ControllerBase
    {
        private readonly ILogger<MqttController> _logger;

        private readonly IMqttClientPublish _mqttSerive;

        public MqttController(ILogger<MqttController> logger, IMqttClientPublish service)
        {
            _logger = logger;  
            _mqttSerive = service;
        }
        [HttpPost, Route("send/{number}")]
        public void SendCommand(string number)
        {
            _mqttSerive.Publish_Application_Message(number);
        }


    }
}
