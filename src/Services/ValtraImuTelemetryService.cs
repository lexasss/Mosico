namespace Mosico.Services;

internal class ValtraImuTelemetryService : UdpTelemetryService
{
    public static string[] FieldNames => [
        "Week",
        "Time",
        "Latitude",     // m
        "Longitude",    // m
        "Elevation",    // m
        "Easting",      // m
        "Northing",     // m
        "Roll",         // deg
        "Pitch",        // deg
        "Heading",      // deg
        "VEast",        // m/s
        "VNorth",       // m/s
        "VUp",          // m/s
        "AccEast",      // m/s^2
        "AccNrth",      // m/s^2
        "AccUp",        // m/s^2
        "AccBdyX",      // m/s^2
        "AccBdyY",      // m/s^2
        "AccBdyZ",      // m/s^2
        "AngRateX",     // deg/s
        "AngRateY",     // deg/s
        "AngRateZ",     // deg/s
    ];

    public override int Port => 38777;

    public override DataSource Source => DataSource.VatraIMU;

    protected override string[] GetFieldNames() => FieldNames;
}
