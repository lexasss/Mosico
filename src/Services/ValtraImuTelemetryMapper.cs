namespace Mosico.Services;

internal class ValtraImuTelemetryMapper : TelemetryMapper
{
    public ValtraImuTelemetryMapper(UdpTelemetryService telemetryService) : base(telemetryService)
    {
        telemetryService.TelemetryReceived += OnTelemetryReceived;
    }

    // Internal

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue(_settings.ValtraImuSizeBindingField, out var telemetryValueForSize))
        {
            UpdateSize(telemetryValueForSize, _settings.ValtraImuSizeBindingScale);
        }

        if (e.TryGetValue(_settings.ValtraImuOffsetXBindingField, out var telemetryValueForOffsetX))
        {
            UpdateOffsetX(telemetryValueForOffsetX, _settings.ValtraImuOffsetBindingScale);
        }

        if (e.TryGetValue(_settings.ValtraImuOffsetYBindingField, out var telemetryValueForOffsetY))
        {
            UpdateOffsetY(telemetryValueForOffsetY, _settings.ValtraImuOffsetBindingScale);
        }

        FireDataUpdate();
    }
}
