namespace Mosico.Services;

public class DataReceivedEventHandlerArgs(double? size = null, double? offsetX = null, double? offsetY = null) : EventArgs
{
    public double? Size { get; init; } = size;
    public double? OffsetX { get; init; } = offsetX;
    public double? OffsetY { get; init; } = offsetY;
}

internal abstract class TelemetryMapper : IDisposable
{
    public event EventHandler<DataReceivedEventHandlerArgs>? DataReceived;

    public TelemetryMapper(UdpTelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
        _settings.Updated += Settings_Updated;

        _size = _settings.InitialCircleSize;
    }

    public void Dispose()
    {
        _telemetryService.Dispose();
        GC.SuppressFinalize(this);
    }

    const double MIN_SIZE = 5;

    protected readonly UdpTelemetryService _telemetryService;

    protected double _size;
    protected double _offsetX = 0;
    protected double _offsetY = 0;

    protected readonly Models.Settings _settings = Models.Settings.Instance;

    protected virtual void UpdateSize(double value, double scale)
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
        var m = _settings.MaxCircleSize;
        var a = _settings.InitialCircleSize;
        var b = m / (m - 2 * a);        // well, "b" is the function inversed scale, but we can derive it from "m" and "a", therefore
        var c = scale;                  // scale is used for controlling the function steepness that is "c"
        var x = Math.Exp(-c * value);
        var size = a * b * 2 * x * x / (1 + b * x + (b - 1) * x * x);   // see the Excel's sheet "size3" for details

        _size += (Math.Max(size, MIN_SIZE) - _size) * _settings.SizeChangeDamping;
    }

    protected virtual void UpdateOffsetX(double value, double scale)
    {
        _offsetX += (scale * value - _offsetX) * _settings.OffsetChangeDamping;
    }

    protected virtual void UpdateOffsetY(double value, double scale)
    {
        _offsetY += (scale * value - _offsetY) * _settings.OffsetChangeDamping;
    }

    protected void FireDataUpdate()
    {
        DataReceived?.Invoke(this, new DataReceivedEventHandlerArgs(_size, _offsetX, _offsetY));
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Models.Settings.InitialCircleSize))
        {
            _size = _settings.InitialCircleSize;
        }
    }
}
