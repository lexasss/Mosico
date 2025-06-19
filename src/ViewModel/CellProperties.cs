using Mosico.Services;
using System.ComponentModel;

namespace Mosico.ViewModel;

internal class CellProperties : INotifyPropertyChanged
{
    public double Size
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Size)));
        }
    } = DefaultSize;

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
    }

    // Internal

    const double MotionDamping_Longitudinal = 0.3;
    const double MotionDamping_Lateral = 0.2;

    const double DefaultSize = 40;

    const double GForceToSize_Scale = 0.7;
    const double GForceToSize_CriticalLongitudinalForce = -2.5;
    const double GForceToSize_CriticalLongitudinalForceSteepness = 5;

    const double GForceToOffset_Scale = 50;

    static double SizeAtCriticalBrakeForce = Math.Exp(-GForceToSize_Scale * GForceToSize_CriticalLongitudinalForce);


    private void OnTelemetryReceived(object? sender, Dictionary<string, float> e)
    {
        if (e.TryGetValue("gforce_longitudinal", out var gforceLongitudinal))
        {
            // The idea is that we take an exponential function of longitudinal g-force to calculate the size: size = b * e^(-a*x),
            // where "x" is the value of "gforce_longitudinal" field
            // However, we want to limit it when desseleration is strong, so the cirlce size do not grow further after reaching some limit.
            // We can do that by mixing two components:
            //  one component is just normally calculate the exponential function of the current longitudinal g-force,
            //  the other component is the same exponential function, but calculated at some fixed longitudinal g-force.
            // The weight is an inversed sigmoid function of proximity to this fixed longitudinal g-force: 1 / (1 + e^(-a*x))
            // where x is the difference between the current and the fixed longitudinal g-forces, and "a" controls the steepness of the transition.

            var weight = 1.0 / (1.0 + Math.Exp(GForceToSize_CriticalLongitudinalForceSteepness * (GForceToSize_CriticalLongitudinalForce - gforceLongitudinal)));

            var scale = Math.Exp(-GForceToSize_Scale * gforceLongitudinal) * weight        // component that depends on "gforce_longitudinal" field
                      + SizeAtCriticalBrakeForce * (1.0 - weight);                         // constant component

            Size += (DefaultSize * scale - Size) * MotionDamping_Longitudinal;

            // Another way to limit the circle size is simply to apply Math.Min function given that we have specified GForceToSize_MaxSize:
            // var size = DefaultSize * Math.Exp(-GForceToSize_Scale * gforceLongitudinal);
            // Size += (Math.Min(scale, GForceToSize_MaxSize) - Size) * MotionDamping_Longitudinal;
        }

        if (e.TryGetValue("gforce_lateral", out var gforceLateral))
        {
            OffsetX += (GForceToOffset_Scale * gforceLateral - OffsetX) * MotionDamping_Lateral;
        }
    }
}
