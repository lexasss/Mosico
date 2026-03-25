namespace Mosico.Services;

internal class ValtraImuTelemetryMapper : TelemetryMapper
{
    public bool IsFilterEnabled { get; set; } = true;

    public ValtraImuTelemetryMapper(UdpTelemetryService telemetryService) : base(telemetryService)
    {
        telemetryService.TelemetryReceived += OnTelemetryReceived;

        _settings.Updated += Settings_Updated;

        _sizeFilter = new(_settings.ValtraImuFilteringSizeStrength, TelemetrySampleInterval);
        _offsetXFilter = new(_settings.ValtraImuFilteringOffsetStrength, TelemetrySampleInterval);
        _offsetYFilter = new(_settings.ValtraImuFilteringOffsetStrength, TelemetrySampleInterval);
    }

    // Internal

    const double TelemetrySampleInterval = 4; // ms

    LowPassFilter _sizeFilter;
    LowPassFilter _offsetXFilter;
    LowPassFilter _offsetYFilter;

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue(_settings.ValtraImuSizeBindingField, out var telemetryValueForSize))
        {
            if (_settings.ValtraImuFilteringSizeEnabled)
                telemetryValueForSize = _sizeFilter.Filter(telemetryValueForSize);
            UpdateSize(telemetryValueForSize, _settings.ValtraImuSizeBindingScale);
        }

        if (e.TryGetValue(_settings.ValtraImuOffsetXBindingField, out var telemetryValueForOffsetX))
        {
            if (_settings.ValtraImuFilteringOffsetEnabled)
                telemetryValueForOffsetX = _offsetXFilter.Filter(telemetryValueForOffsetX);
            UpdateOffsetX(telemetryValueForOffsetX, _settings.ValtraImuOffsetBindingScale);
        }

        if (e.TryGetValue(_settings.ValtraImuOffsetYBindingField, out var telemetryValueForOffsetY))
        {
            if (_settings.ValtraImuFilteringOffsetEnabled)
                telemetryValueForOffsetY = _offsetYFilter.Filter(telemetryValueForOffsetY);
            UpdateOffsetY(telemetryValueForOffsetY, _settings.ValtraImuOffsetBindingScale);
        }

        FireDataUpdate();
    }

    private void Settings_Updated(object? sender, string e)
    {
        if (e == nameof(_settings.ValtraImuFilteringSizeStrength))
        {
            _sizeFilter = new(_settings.ValtraImuFilteringSizeStrength, TelemetrySampleInterval);
        }
        else if (e == nameof(_settings.ValtraImuFilteringOffsetStrength))
        {
            _offsetXFilter = new(_settings.ValtraImuFilteringOffsetStrength, TelemetrySampleInterval);
            _offsetYFilter = new(_settings.ValtraImuFilteringOffsetStrength, TelemetrySampleInterval);
        }
    }
}
