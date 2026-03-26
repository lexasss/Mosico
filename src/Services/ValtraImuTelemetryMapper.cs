namespace Mosico.Services;

internal class ValtraImuTelemetryMapper : TelemetryMapper
{
    public bool IsFilterEnabled { get; set; } = true;

    public ValtraImuTelemetryMapper(UdpTelemetryService telemetryService) : base(telemetryService)
    {
        telemetryService.TelemetryReceived += OnTelemetryReceived;

        _settings.Updated += Settings_Updated;

        _sizeFilter = new(_settings.ValtraImuFilteringSizeStrength, TELEMETRY_SAMPLE_INTERVAL);
        _offsetFilter = new(_settings.ValtraImuFilteringOffsetStrength, TELEMETRY_SAMPLE_INTERVAL);
    }

    // Internal

    const double TELEMETRY_SAMPLE_INTERVAL = 4; // ms

    LowPassFilter _sizeFilter;
    LowPassFilter _offsetFilter;

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> data)
    {
        if (data.TryGetValue(_settings.ValtraImuSizeBindingField, out var size))
        {
            if (_settings.ValtraImuFilteringSizeEnabled)
                size = _sizeFilter.Filter(size);

            UpdateSize(size, _settings.ValtraImuSizeBindingScale);
        }

        if (data.TryGetValue(_settings.ValtraImuOffsetXBindingField, out var offsetX) &&
            data.TryGetValue(_settings.ValtraImuOffsetYBindingField, out var offsetY))
        {
            if (_settings.ValtraImuFilteringOffsetEnabled)
                (offsetX, offsetY) = _offsetFilter.Filter(offsetX, offsetY);

            UpdateOffsetX(offsetX, _settings.ValtraImuOffsetBindingScale);
            UpdateOffsetY(offsetY, _settings.ValtraImuOffsetBindingScale);
        }

        FireDataUpdate();
    }

    private void Settings_Updated(object? sender, string e)
    {
        if (e == nameof(_settings.ValtraImuFilteringSizeStrength))
        {
            _sizeFilter = new(_settings.ValtraImuFilteringSizeStrength, TELEMETRY_SAMPLE_INTERVAL);
        }
        else if (e == nameof(_settings.ValtraImuFilteringOffsetStrength))
        {
            _offsetFilter = new(_settings.ValtraImuFilteringOffsetStrength, TELEMETRY_SAMPLE_INTERVAL);
        }
    }
}
