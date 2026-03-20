namespace Mosico.Services;

internal class CarlaTelemetryMapper : TelemetryMapper
{
    public CarlaTelemetryMapper(UdpTelemetryService telemetryService) : base(telemetryService)
    {
        telemetryService.TelemetryReceived += OnTelemetryReceived;
    }

    // Internal

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue(_settings.CarlaSizeBindingField, out var telemetryValueForSize))
        {
            UpdateSize(telemetryValueForSize, _settings.CarlaSizeBindingScale);
        }

        if (e.TryGetValue(_settings.CarlaOffsetBindingField, out var telemetryValueForOffset))
        {
            UpdateOffsetX(telemetryValueForOffset, _settings.CarlaOffsetBindingScale);
        }

        FireDataUpdate();
    }
}
