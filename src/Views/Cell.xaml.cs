using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mosico.Views;

public partial class Cell : UserControl
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        name: "Color",
        propertyType: typeof(Brush),
        ownerType: typeof(Cell),
        typeMetadata: new FrameworkPropertyMetadata(
            defaultValue: Brushes.HotPink,
            flags: FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public Brush Color
    {
        get => (Brush)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public Cell()
    {
        InitializeComponent();
    }
}
