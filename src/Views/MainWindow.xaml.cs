using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        _cellProperies = new ViewModels.CellProperties();

        _settings.Updated += Settings_Updated;
    }

    // Internal

    readonly ViewModels.CellProperties _cellProperies;
    readonly Models.Settings _settings = Models.Settings.Instance;

    Services.UdpTelemetryService? _telemetryService;
    SettingsDialog? _settingsDialog;

    private void CreateGrid()
    {
        ugdContainer.Children.Clear();

        var sizeBinding = new Binding(nameof(ViewModels.CellProperties.CircleSize)) { Source = _cellProperies };
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

    private void SetDataSource()
    {
        if (_telemetryService?.Source == _settings.DataSource)
            return;

        _telemetryService?.Dispose();
        _telemetryService = null;

        Services.TelemetryMapper? telemetryMapper = null;

        switch (_settings.DataSource)
        {
            case DataSource.Carla:
                _telemetryService = new Services.CarlaTelemetryService();
                telemetryMapper = new Services.CarlaTelemetryMapper(_telemetryService);
                break;
            case DataSource.VatraIMU:
                _telemetryService = new Services.ValtraImuTelemetryService();
                telemetryMapper = new Services.ValtraImuTelemetryMapper(_telemetryService);
                break;
        }

        _cellProperies.SetTelemetryMapper(telemetryMapper);
    }

    private void Settings_Updated(object? sender, string propName)
    {
        if (propName == nameof(Models.Settings.CellSize))
        {
            CreateGrid();
        }
        else if (propName == nameof(Models.Settings.DataSource))
        {
            SetDataSource();
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        Services.WindowsServices.SetWindowTransparent(this);
    }

    // UI

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CreateGrid();
        SetDataSource();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _telemetryService?.Dispose();
    }

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_settingsDialog == null)
        {
            Hide();

            _settingsDialog = new SettingsDialog();
            _settingsDialog.ShowDialog();
            _settingsDialog = null;

            Show();

            _telemetryService?.IsPaused = false;
        }
        else
        {
            _settingsDialog.Activate();
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TrayPopup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        trbNotifyIcon.CloseTrayPopup();
    }

    private void NotifyIcon_TrayContextMenuOpen(object sender, RoutedEventArgs e)
    {
        /// Somewhat complex way to pause and resume telemetry streaming when a tray menu is opened.
        /// Here, it pauses when the menu pops up. It then contantly checks whether the menu is still visible,
        /// and resumes when the menu dissappears, but only if the window (actually, dots) are visible, 
        /// i.e. the settings window is not visible. Otherwise the telemetry should resume after the 
        /// setting window is closed (in <see cref="SettingsMenuItem_Click(object, RoutedEventArgs)"/>).

        _telemetryService?.IsPaused = true;

        Task.Run(async () => {
            while (true)
            {
                await Task.Delay(500);
                bool isMenuVisible = false;
                Dispatcher.Invoke(() => isMenuVisible = trbNotifyIcon.ContextMenu.IsVisible);

                if (isMenuVisible)
                    continue;
                
                if (IsVisible)
                    _telemetryService?.IsPaused = false;

                break;
            }
        });
    }
}