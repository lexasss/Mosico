using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        _cellProperies = new ViewModel.CellProperties(_telemetryService);
    }

    // Internal

    const int CELL_SIZE = 300;  // px

    readonly Services.TelemetryService _telemetryService = new();
    readonly ViewModel.CellProperties _cellProperies;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        Services.WindowsServices.SetWindowTransparent(this);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var sizeBinding = new Binding("Size") { Source = _cellProperies };
        var xOffsetBinding = new Binding("OffsetX") { Source = _cellProperies };
        var yOffsetBinding = new Binding("OffsetY") { Source = _cellProperies };

        int rows = (int)(ActualHeight / CELL_SIZE);
        int columns = (int)(ActualWidth / CELL_SIZE);

        ugdContainer.Rows = rows > 0 ? rows : 1;
        ugdContainer.Columns = columns > 0 ? columns : 1;

        var translateTransform = new TranslateTransform();
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(translateTransform);

        BindingOperations.SetBinding(translateTransform, TranslateTransform.XProperty, xOffsetBinding);
        BindingOperations.SetBinding(translateTransform, TranslateTransform.YProperty, yOffsetBinding);

        for (int i = 0; i < ugdContainer.Rows; i++)
        {
            for (int j = 0; j < ugdContainer.Columns; j++)
            {
                var cell = new Views.Cell
                {
                    RenderTransform = transformGroup
                };

                BindingOperations.SetBinding(cell, WidthProperty, sizeBinding);
                BindingOperations.SetBinding(cell, HeightProperty, sizeBinding);
                
                ugdContainer.Children.Add(cell);
            }
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _telemetryService.Dispose();
    }
}