using System.Windows;

namespace Mosico.Views;

public partial class SettingsDialog : Window
{
    public Models.Settings Settings { get; } = new Models.Settings();

    public SettingsDialog()
    {
        InitializeComponent();
    }

    // Internal

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Settings.Save();
        Close();
    }
}
