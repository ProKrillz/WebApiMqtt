

namespace Services.Interface;

public interface IMqttClientPublish
{
    Task Publish_Application_Message(string number);
}
