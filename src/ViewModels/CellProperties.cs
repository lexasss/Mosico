using Mosico.Services;
using System.ComponentModel;
using System.Windows.Media;

namespace Mosico.ViewModels;

internal class CellProperties : INotifyPropertyChanged
{
    public Color Color => _settings.CellColor;

    public double Transparency => _settings.CellTransparency;

    public double CircleSize
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CircleSize)));
        }
    }

    public double OffsetX
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OffsetX)));
        }
    }

    public double OffsetY
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OffsetY)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CellProperties()
    {
        _settings.Updated += Settings_Updated;

        CircleSize = _settings.InitialCircleSize;
    }

    /// <summary>
    /// Transfers ownership
    /// </summary>
    /// <param name="telemetryMapper">the mapper, or null</param>
    public void SetTelemetryMapper(TelemetryMapper? telemetryMapper)
    {
        if (_telemetryMapper != null)
        {
            _telemetryMapper.DataReceived -= TelemetryMapper_DataReceived;
            _telemetryMapper.Dispose();
        }
        _telemetryMapper = telemetryMapper;
        if (_telemetryMapper != null)
        {
            _telemetryMapper.DataReceived += TelemetryMapper_DataReceived;
        }
    }

    // Internal

    readonly Models.Settings _settings = Models.Settings.Instance;

    TelemetryMapper? _telemetryMapper = null;

    private void TelemetryMapper_DataReceived(object? sender, DataReceivedEventHandlerArgs e)
    {
        if (e.Size.HasValue)
        {
            CircleSize = e.Size.Value;
        }
        if (e.OffsetX.HasValue)
        {
            OffsetX = e.OffsetX.Value;
        }
        if (e.OffsetY.HasValue)
        {
            OffsetY = e.OffsetY.Value;
        }
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Models.Settings.CellColor))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
        else if (propName == nameof(Models.Settings.CellTransparency))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Transparency)));
        }
        else if (propName == nameof(Models.Settings.InitialCircleSize))
        {
            CircleSize = _settings.InitialCircleSize;
        }
    }
}
