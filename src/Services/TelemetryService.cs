using System.Net;
using System.Net.Sockets;

namespace Mosico.Services;

internal class TelemetryService : IDisposable
{
    public static string[] FieldNames => [
        "total_time",
        "lap_time",
        "lap_distance",
        "total_distance",
        "position_y",
        "position_z",
        "position_x",
        "speed",
        "velocity_x",
        "velocity_y",
        "velocity_z",
        "left_dir_x",
        "left_dir_y",
        "left_dir_z",
        "forward_dir_x",
        "forward_dir_y",
        "forward_dir_z",

        // unreported fields
        "suspension_position_bl",
        "suspension_position_br",
        "suspension_position_fl",
        "suspension_position_fr",
        "suspension_velocity_bl",
        "suspension_velocity_br",
        "suspension_velocity_fl",
        "suspension_velocity_fr",
        "wheel_patch_speed_bl",
        "wheel_patch_speed_br",
        "wheel_patch_speed_fl",
        "wheel_patch_speed_fr",
        "throttle_input",
        "steering_input",
        "brake_input",
        "clutch_input",
        "gear",

        "gforce_lateral",
        "gforce_longitudinal",
        "lap",
        "engine_rate",
        "native_sli_support",
        "race_position",
        "kers_level",
        "kers_level_max",
        "drs",
        "traction_control",
        "abs",
        "fuel_in_tank",
        "fuel_capacity",
        "in_pits",
        "race_sector",
        "sector_time_1",
        "sector_time_2",
        "brake_temp_bl",
        "brake_temp_br",
        "brake_temp_fl",
        "brake_temp_fr",
        "tyre_pressure_bl",
        "tyre_pressure_br",
        "tyre_pressure_fl",
        "tyre_pressure_fr",
        "laps_completed",
        "total_laps",
        "track_length",
        "last_lap_time",
        "max_rpm",
        "idle_rpm",
        "max_gears"
    ];

    public event EventHandler<Dictionary<string, float>>? TelemetryReceived;

    public TelemetryService()
    {
        var listenEndPoint = new IPEndPoint(IPAddress.Any, PORT);

        _udpClient.ExclusiveAddressUse = false;
        _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udpClient.Client.Bind(listenEndPoint);
        _udpClient.BeginReceive(DataReceived, _udpClient);
    }

    public void Dispose()
    {
        _udpClient.Close();
        _udpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    // Internal

    const int PORT = 20779; // 20778 - regular frequency, 20779 - reduced frequency

    const int VALUE_SIZE = 4;           // bytes, size of float
    readonly static int VALUE_COUNT = FieldNames.Length;
    readonly static int DATAGRAM_SIZE = VALUE_COUNT * VALUE_SIZE; // 264 bytes

    readonly UdpClient _udpClient = new();

    private void DataReceived(IAsyncResult ar)
    {
        var c = (UdpClient?)ar.AsyncState;
        var receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            byte[] bytes = c?.EndReceive(ar, ref receivedIpEndPoint) ?? [];
            System.Diagnostics.Debug.Write($"""[{receivedIpEndPoint}] {DateTime.Now:HH:mm:ss.ff tt}: Received {bytes.Length} bytes --- """);

            if (bytes.Length >= DATAGRAM_SIZE)
            {
                var telemetryData = new Dictionary<string, float>(VALUE_COUNT);
                for (int i = 0; i < VALUE_COUNT; i++)
                {
                    int offset = i * VALUE_SIZE;
                    telemetryData.Add(FieldNames[i], BitConverter.ToSingle(bytes, offset));
                }
                System.Diagnostics.Debug.WriteLine($"time = {telemetryData["total_time"]}");
                TelemetryReceived?.Invoke(null, telemetryData);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("TOO LITTLE");
            }

            c?.BeginReceive(DataReceived, ar.AsyncState);
        }
        catch (ObjectDisposedException)
        {
            // The UdpClient has been disposed, exit the method
            return;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error receiving data: {ex.Message}");
            return;
        }   
    }
}
