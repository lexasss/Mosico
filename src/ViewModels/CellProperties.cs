using Mosico.Services;
using System.ComponentModel;
using System.Windows.Media;

namespace Mosico.ViewModels;

internal class CellProperties : INotifyPropertyChanged
{
    public SolidColorBrush Color
    {
        get => _settings.CellColor;
        set
        {
            _settings.CellColor = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }

    public double Size
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Size)));
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

    public CellProperties(TelemetryService telemetryService)
    {
        telemetryService.TelemetryReceived += OnTelemetryReceived;
        _settings.Updated += Settings_Updated;

        Size = _settings.CircleSize;
    }

    // Internal

    readonly Models.Settings _settings = Models.Settings.Instance;

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue(_settings.BindSizeField, out var telemetryValueForSize))
        {
            // METHOD 1:
            // The idea is that we take an exponential function of longitudinal g-force to calculate the size: size = a * e^(-b*x),
            // where "x" is "telemetryValueForSize".
            // However, we want to limit it when desseleration is strong, so the cirlce does not grow further after reaching some limit.
            // We can do that by adding another component that is the same exponential function, but calculated at some fixed x: x = c.
            // The weight is an inversed sigmoid function of proximity to this fixed longitudinal g-force: 1 / (1 + e^(-k*d))
            // where d = c - x, i.e. the difference between the current and the fixed longitudinal g-forces,
            // and "k" controls the steepness of the transition.
            // See the Excel's sheet "size1and2" for details

            /*
            var m = _settings.BindSizeMax;
            var a = _settings.CircleSize;
            var b = _settings.BindSizeScale;
            var c = Math.Log(a / m) / b;
            var k = 5;
            var weight = 1.0 / (1.0 + Math.Exp(k * (c - telemetryValueForSize)));

            var size = a * Math.Exp(-b * telemetryValueForSize) * weight        // component that depends on "gforce_longitudinal" field
                     + a * Math.Exp(-b * c) * (1.0 - weight);                   // constant component

            Size += (size - Size) * _settings.BindSizeDamp;
            */

            /*
            // METHOD 2:
            // Same as METHOD 1: size = a * e^(-b*x), but size limiting is achieved with Math.Min

            var m = _settings.BindSizeMax;
            var a = _settings.CircleSize;
            var b = _settings.BindSizeScale;
            var size = a * Math.Exp(-b * telemetryValueForSize);
            Size += (Math.Min(size, m) - Size) * _settings.BindSizeDamp;
            */

            // METHOD 3:
            var m = _settings.BindSizeMax;
            var a = _settings.CircleSize;
            var b = m / (m - 2 * a);            // well, "b" is the function inversed scale, but we can derive it from "m" and "a", therefore
            var c = _settings.BindSizeScale;    // "BindSizeScale" is used for controlling the function steepness that is "c"
            var x = Math.Exp(-c * telemetryValueForSize);
            var size = a * b * 2 * x * x / (1 + b * x + (b - 1) * x * x);   // see the Excel's sheet "size3" for details
            var minSize = 5;
            Size += (Math.Max(size, minSize) - Size) * _settings.BindSizeDamp;
        }

        if (e.TryGetValue(_settings.BindOffsetField, out var telemetryValueForOffset))
        {
            OffsetX += (_settings.BindOffsetScale * telemetryValueForOffset - OffsetX) * _settings.BindOffsetDamp;
        }
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Models.Settings.CellColor))
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }
}
