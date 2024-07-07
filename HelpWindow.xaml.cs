using System.Windows;

namespace ProjectManagement;

public partial class HelpWindow {
    public HelpWindow() {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}
