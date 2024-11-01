using Services.DTO;

namespace Services.Interface;

public interface IInfluxDBService
{
    Task WriteTelemetry(Telemetry telemetry);

    Task<List<Telemetry>> QuereDB();
}
