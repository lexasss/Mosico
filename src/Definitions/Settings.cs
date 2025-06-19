using System.Windows.Media;

namespace Mosico.Definitions;

public class Settings
{
    public static Settings Instance => _instance ??= new Settings(true);

    public SolidColorBrush CellColor
    {
        get
        {
            var num = Get<uint>(nameof(CellColor));
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = (byte)(num & 0xFF);
                num >>= 8;
            }
            var color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            return new SolidColorBrush(color);
        }
        set
        {
            uint num = value.Color.A;
            num = (num << 8) | value.Color.R;
            num = (num << 8) | value.Color.G;
            num = (num << 8) | value.Color.B;

            Save(nameof(CellColor), num);
        }
    }

    public double CellSize
    {
        get => Get<double>(nameof(CellSize));
        set => Save(nameof(CellSize), value);
    }

    public double CircleSize
    {
        get => Get<double>(nameof(CircleSize));
        set => Save(nameof(CircleSize), value);
    }

    public string BindSizeField
    {
        get => Get<string>(nameof(BindSizeField));
        set => Save(nameof(CircleSize), value);
    }

    public double BindSizeScale
    {
        get => Get<double>(nameof(BindSizeScale));
        set => Save(nameof(BindSizeScale), value);
    }

    public double BindSizeMax
    {
        get => Get<double>(nameof(BindSizeMax));
        set => Save(nameof(BindSizeMax), value);
    }

    public double BindSizeDamp
    {
        get => Get<double>(nameof(BindSizeDamp));
        set => Save(nameof(BindSizeDamp), value);
    }

    public string BindOffsetField
    {
        get => Get<string>(nameof(BindOffsetField));
        set => Save(nameof(BindOffsetField), value);
    }

    public double BindOffsetScale
    {
        get => Get<double>(nameof(BindOffsetScale));
        set => Save(nameof(BindOffsetScale), value);
    }

    public double BindOffsetDamp
    {
        get => Get<double>(nameof(BindOffsetDamp));
        set => Save(nameof(BindOffsetDamp), value);
    }

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
        if (_canSaveToStorage || !_cache.ContainsKey(prop))
        {
            _cache[prop] = Properties.Settings.Default[prop];
            return (T)Properties.Settings.Default[prop];
        }
        else
        {
            return (T)_cache[prop];
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
            if (_cache.ContainsKey(prop))
            {
                _cache[prop] = value;
            }
            else
            {
                _cache.Add(prop, value);
            }
        }
    }
}
