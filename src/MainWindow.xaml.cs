using Mosico.Definitions;
using Mosico.ViewModels;
using Mosico.Views;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        _cellProperies = new CellProperties(_telemetryService);

        _settings.Updated += Settings_Updated;
    }

    // Internal

    readonly Services.TelemetryService _telemetryService = new();
    readonly CellProperties _cellProperies;
    readonly Settings _settings = Settings.Instance;

    private void CreateGrid()
    {
        ugdContainer.Children.Clear();

        var sizeBinding = new Binding(nameof(CellProperties.Size)) { Source = _cellProperies };
        var xOffsetBinding = new Binding(nameof(CellProperties.OffsetX)) { Source = _cellProperies };
        var yOffsetBinding = new Binding(nameof(CellProperties.OffsetY)) { Source = _cellProperies };
        var colorBinding = new Binding(nameof(CellProperties.Color)) { Source = _cellProperies };

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
                var cell = new Views.Cell
                {
                    RenderTransform = transformGroup
                };

                BindingOperations.SetBinding(cell, WidthProperty, sizeBinding);
                BindingOperations.SetBinding(cell, HeightProperty, sizeBinding);
                BindingOperations.SetBinding(cell, Views.Cell.ColorProperty, colorBinding);

                ugdContainer.Children.Add(cell);
            }
        }
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Settings.CellSize))
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