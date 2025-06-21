using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        _cellProperies = new ViewModels.CellProperties(_telemetryService);

        _settings.Updated += Settings_Updated;
    }

    // Internal

    readonly Services.TelemetryService _telemetryService = new();
    readonly ViewModels.CellProperties _cellProperies;
    readonly Models.Settings _settings = Models.Settings.Instance;

    private void CreateGrid()
    {
        ugdContainer.Children.Clear();

        var sizeBinding = new Binding(nameof(ViewModels.CellProperties.Size)) { Source = _cellProperies };
        var xOffsetBinding = new Binding(nameof(ViewModels.CellProperties.OffsetX)) { Source = _cellProperies };
        var yOffsetBinding = new Binding(nameof(ViewModels.CellProperties.OffsetY)) { Source = _cellProperies };
        var colorBinding = new Binding(nameof(ViewModels.CellProperties.Color)) { Source = _cellProperies };

        int rows = (int)(ActualHeight / _settings.CellSize);
        int columns = (int)(ActualWidth / _settings.CellSize);

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
                var cell = new Cell
                {
                    RenderTransform = transformGroup
                };

                BindingOperations.SetBinding(cell, WidthProperty, sizeBinding);
                BindingOperations.SetBinding(cell, HeightProperty, sizeBinding);
                BindingOperations.SetBinding(cell, Cell.ColorProperty, colorBinding);

                ugdContainer.Children.Add(cell);
            }
        }
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Models.Settings.CellSize))
        {
            CreateGrid();
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        Services.WindowsServices.SetWindowTransparent(this);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CreateGrid();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _telemetryService.Dispose();
    }

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SettingsDialog();
        dialog.ShowDialog();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TrayPopup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        trbNotifyIcon.CloseTrayPopup();
    }
}