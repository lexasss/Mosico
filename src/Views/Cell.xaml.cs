using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;

namespace Mosico.Views;

public partial class Cell : UserControl, INotifyPropertyChanged
{
    public Brush MainBrush
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainBrush)));
        }
    } = Brushes.HotPink;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Cell()
    {
        InitializeComponent();
    }
}
