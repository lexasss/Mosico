using System.Windows.Media;

namespace Mosico.Definitions;

public class Settings
{
    public static Settings Instance = _instance ??= new Settings();

    public SolidColorBrush CellColor
    {
        get
        {
            var num = Properties.Settings.Default.CellColor;
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
        get => Properties.Settings.Default.CellSize;
        set => Save(nameof(CellSize), value);
    }

    public double CircleSize
    {
        get => Properties.Settings.Default.CircleSize;
        set => Save(nameof(CircleSize), value);
    }

    public string BindSizeField
    {
        get => Properties.Settings.Default.BindSizeField;
        set => Save(nameof(CircleSize), value);
    }

    public double BindSizeScale
    {
        get => Properties.Settings.Default.BindSizeScale;
        set => Save(nameof(BindSizeScale), value);
    }

    public double BindSizeMax
    {
        get => Properties.Settings.Default.BindSizeMax;
        set => Save(nameof(BindSizeMax), value);
    }

    public double BindSizeDamp
    {
        get => Properties.Settings.Default.BindSizeDamp;
        set => Save(nameof(BindSizeDamp), value);
    }

    public string BindOffsetField
    {
        get => Properties.Settings.Default.BindOffsetField;
        set => Save(nameof(BindOffsetField), value);
    }

    public double BindOffsetScale
    {
        get => Properties.Settings.Default.BindOffsetScale;
        set => Save(nameof(BindOffsetScale), value);
    }

    public double BindOffsetDamp
    {
        get => Properties.Settings.Default.BindOffsetDamp;
        set => Save(nameof(BindOffsetDamp), value);
    }

    public event EventHandler<string>? Updated;

    // Internal

    static Settings? _instance = null;

    private Settings() { }

    private void Save(string prop, object value)
    {
        Properties.Settings.Default[prop] = value;
        Properties.Settings.Default.Save();

        Updated?.Invoke(this, prop);
    }
}
