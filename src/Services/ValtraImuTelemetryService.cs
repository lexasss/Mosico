namespace Mosico.Services;

internal class ValtraImuTelemetryService : UdpTelemetryService
{
    public static string[] FieldNames => [
        "Time",
        "X (forward)",  // m
        "Y (right)",    // m
        "Z (upward)",   // m
        "Roll",         // deg
        "Pitch",        // deg
        "Yaw",          // deg
        "Vel X",        // m/s
        "Vel Y",        // m/s
        "Vel Z",        // m/s
        "Vel Roll",     // m/s^2
        "Vel Pitch",    // m/s^2
        "Vel Yaw",      // m/s^2
        "Acc X",        // m/s^2
        "Acc Y",        // m/s^2
        "Acc Z",        // m/s^2
        "Acc Roll",     // deg/s^2
        "Acc Pitch",    // deg/s^2
        "Acc Yaw",      // deg/s^2
    ];

    public override int Port => 38777;

    public override DataSource Source => DataSource.VatraIMU;

    protected override string[] GetFieldNames() => FieldNames;
}
