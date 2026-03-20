using System.Windows.Media;

namespace Mosico.Models;

public class Settings
{
    public static Settings Instance => _instance ??= new Settings(true);

    public DataSource DataSource
    {
        get => (DataSource)Get<int>(nameof(DataSource));
        set => Save(nameof(DataSource), (int)value);
    }

    public SolidColorBrush CellColor
    {
        get => new(IntToColor(Get<uint>(nameof(CellColor))));
        set => Save(nameof(CellColor), ColorToInt(value.Color));
    }

    public double CellSize
    {
        get => Get<double>(nameof(CellSize));
        set => Save(nameof(CellSize), value);
    }

    public double InitialCircleSize
    {
        get => Get<double>(nameof(InitialCircleSize));
        set => Save(nameof(InitialCircleSize), value);
    }

    public double MaxCircleSize
    {
        get => Get<double>(nameof(MaxCircleSize));
        set => Save(nameof(MaxCircleSize), value);
    }

    public double SizeChangeDamping
    {
        get => Get<double>(nameof(SizeChangeDamping));
        set => Save(nameof(SizeChangeDamping), value);
    }

    public double OffsetChangeDamping
    {
        get => Get<double>(nameof(OffsetChangeDamping));
        set => Save(nameof(OffsetChangeDamping), value);
    }

    #region Carla bindings

    public string CarlaSizeBindingField
    {
        get => Get<string>(nameof(CarlaSizeBindingField));
        set => Save(nameof(CarlaSizeBindingField), value);
    }

    public double CarlaSizeBindingScale
    {
        get => Get<double>(nameof(CarlaSizeBindingScale));
        set => Save(nameof(CarlaSizeBindingScale), value);
    }

    public string CarlaOffsetBindingField
    {
        get => Get<string>(nameof(CarlaOffsetBindingField));
        set => Save(nameof(CarlaOffsetBindingField), value);
    }

    public double CarlaOffsetBindingScale
    {
        get => Get<double>(nameof(CarlaOffsetBindingScale));
        set => Save(nameof(CarlaOffsetBindingScale), value);
    }

    #endregion

    #region ValtraIMU bindings

    public string ValtraImuSizeBindingField
    {
        get => Get<string>(nameof(ValtraImuSizeBindingField));
        set => Save(nameof(ValtraImuSizeBindingField), value);
    }

    public double ValtraImuSizeBindingScale
    {
        get => Get<double>(nameof(ValtraImuSizeBindingScale));
        set => Save(nameof(ValtraImuSizeBindingScale), value);
    }

    public string ValtraImuOffsetXBindingField
    {
        get => Get<string>(nameof(ValtraImuOffsetXBindingField));
        set => Save(nameof(ValtraImuOffsetXBindingField), value);
    }

    public string ValtraImuOffsetYBindingField
    {
        get => Get<string>(nameof(ValtraImuOffsetYBindingField));
        set => Save(nameof(ValtraImuOffsetYBindingField), value);
    }

    public double ValtraImuOffsetBindingScale
    {
        get => Get<double>(nameof(ValtraImuOffsetBindingScale));
        set => Save(nameof(ValtraImuOffsetBindingScale), value);
    }

    #endregion
    public event EventHandler<string>? Updated;

    /// <summary>
    /// This constructor should normaly not be used as instance created with the public constuructor 
    /// do not save values to the storage. Consinder using <see cref="Instance"/> instead.
    /// </summary>
    public Settings() : this(false) { }

    public void Save()
    {
        if (_instance != null)
        {
            var storage = Properties.Settings.Default;

            foreach (var kv in _cache)
            {
                storage[kv.Key] = kv.Value;
            }

            storage.Save();

            _instance.FireAllUpdates();
        }
    }

    // Internal

    static Settings? _instance = null;

    readonly Dictionary<string, object> _cache = [];

    readonly bool _canSaveToStorage;

    private Settings(bool canSaveToStorage)
    {
        _canSaveToStorage = canSaveToStorage;
    }

    protected void FireAllUpdates()
    {
        foreach (var kv in _cache)
        {
            Updated?.Invoke(this, kv.Key);
        }
    }

    private T Get<T>(string prop)
    {
        if (_canSaveToStorage || !_cache.TryGetValue(prop, out object? value))
        {
            try
            {
                _cache[prop] = Properties.Settings.Default[prop];
                return (T)_cache[prop];
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"Property '{prop}' does not exist");
                return default!;
            }
        }
        else
        {
            return (T)value;
        }
    }

    private void Save(string prop, object value)
    {
        if (_canSaveToStorage)
        {
            Properties.Settings.Default[prop] = value;
            Properties.Settings.Default.Save();

            Updated?.Invoke(this, prop);
        }
        else
        {
            if (!_cache.TryAdd(prop, value))
            {
                _cache[prop] = value;
            }
        }
    }

    private static Color IntToColor(uint value)
    {
        byte[] bytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            bytes[i] = (byte)(value & 0xFF);
            value >>= 8;
        }
        return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static uint ColorToInt(Color color)
    {
        uint result = color.A;
        result = (result << 8) | color.R;
        result = (result << 8) | color.G;
        result = (result << 8) | color.B;
        return result;
    }
}
