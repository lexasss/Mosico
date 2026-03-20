using System.ComponentModel;

namespace Mosico;

[TypeConverter(typeof(Utils.FriendlyEnumConverter))]
public enum DataSource
{
    None,
    Carla,
    VatraIMU
}
