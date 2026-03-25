using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Mosico.Services;

internal abstract class UdpTelemetryService : IDisposable
{
    public bool IsPaused { get; set; } = false;
    public abstract int Port { get; }
    public abstract DataSource Source { get; }

    public event EventHandler<Dictionary<string, float>>? TelemetryReceived;

    public UdpTelemetryService()
    {
        var listenEndPoint = new IPEndPoint(IPAddress.Loopback, Port);

        _udpClient.ExclusiveAddressUse = false;
        _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udpClient.Client.Bind(listenEndPoint);

        Task.Run(() => ReceiveLoopAsync(GetFieldNames(), _cts.Token));
    }

    public void Dispose()
    {
        _cts.Cancel();

        _udpClient.Close();
        _udpClient.Dispose();

        GC.SuppressFinalize(this);
    }

    // Internal

    readonly UdpClient _udpClient = new();
    readonly CancellationTokenSource _cts = new();

    protected abstract string[] GetFieldNames();

    private async Task ReceiveLoopAsync(string[] fields, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync(token);
                if (IsPaused)
                {
                    await Task.Delay(100, token);
                    continue;
                }

                float[] values = MemoryMarshal.Cast<byte, float>(result.Buffer).ToArray();

                if (values.Length >= fields.Length)
                {
                    var telemetryData = new Dictionary<string, float>(fields.Length);
                    for (int i = 0; i < fields.Length; i++)
                    {
                        telemetryData.Add(fields[i], values[i]);
                    }

                    TelemetryReceived?.Invoke(null, telemetryData);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Invalid package");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error on receiving data: {ex.Message}");
        }
    }
}
