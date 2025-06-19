using Mosico.Definitions;
using System.Windows;

namespace Mosico.Views;

public partial class SettingsDialog : Window
{
    public Settings Settings => Settings.Instance;

    public SettingsDialog()
    {
        InitializeComponent();
    }

    // Internal

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Settings.Save();
    }
}
