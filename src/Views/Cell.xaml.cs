using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mosico.Views;

public partial class Cell : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        name: "Color",
        propertyType: typeof(Color),
        ownerType: typeof(Cell),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: Colors.HotPink,
            propertyChangedCallback: new PropertyChangedCallback((s, e) =>
            {
                var cell = (Cell)s;
                cell.PropertyChanged?.Invoke(s, new PropertyChangedEventArgs(nameof(FillBrush)));
            }))
    );

    public static readonly DependencyProperty TransparencyProperty = DependencyProperty.Register(
        name: "Transparency",
        propertyType: typeof(double),
        ownerType: typeof(Cell),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: 0.0,
            propertyChangedCallback: new PropertyChangedCallback((s, e) =>
            {
                var cell = (Cell)s;
                cell.PropertyChanged?.Invoke(s, new PropertyChangedEventArgs(nameof(FillBrush)));
            }))
    );

    public Brush FillBrush
    {
        get
        {
            var color = (Color)GetValue(ColorProperty);
            var transparency = (double)GetValue(TransparencyProperty);
            return new SolidColorBrush(Color.FromArgb((byte)(255 - 255 * transparency), color.R, color.G, color.B));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    public Cell()
    {
        InitializeComponent();
    }
}
