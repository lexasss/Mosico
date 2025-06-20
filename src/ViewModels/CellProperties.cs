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

    //static double SizeAtCriticalBrakeForce = Math.Exp(-GForceToSize_Scale * GForceToSize_CriticalLongitudinalForce);

    readonly Models.Settings _settings = Models.Settings.Instance;

    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue(_settings.BindSizeField, out var telemetryValueForSize))
        {
            // The idea is that we take an exponential function of longitudinal g-force to calculate the size: size = b * e^(-a*x),
            // where "x" is the value of "gforce_longitudinal" field
            // However, we want to limit it when desseleration is strong, so the cirlce size do not grow further after reaching some limit.
            // We can do that by mixing two components:
            //  one component is just normally calculate the exponential function of the current longitudinal g-force,
            //  the other component is the same exponential function, but calculated at some fixed longitudinal g-force.
            // The weight is an inversed sigmoid function of proximity to this fixed longitudinal g-force: 1 / (1 + e^(-a*x))
            // where x is the difference between the current and the fixed longitudinal g-forces, and "a" controls the steepness of the transition.

            /*
            var weight = 1.0 / (1.0 + Math.Exp(GForceToSize_CriticalLongitudinalForceSteepness * (GForceToSize_CriticalLongitudinalForce - gforceLongitudinal)));

            var scale = Math.Exp(-GForceToSize_Scale * telemetryValueForSize) * weight        // component that depends on "gforce_longitudinal" field
                      + SizeAtCriticalBrakeForce * (1.0 - weight);                         // constant component

            Size += (DefaultSize * scale - Size) * MotionDamping_Longitudinal;
            */

            // Another way to limit the circle size is simply to apply Math.Min function given that we have specified GForceToSize_MaxSize:

            var size = _settings.CircleSize * Math.Exp(-_settings.BindSizeScale * telemetryValueForSize);
            Size += (Math.Min(size, _settings.BindSizeMax) - Size) * _settings.BindSizeDamp;
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
