using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class MainWindow {
    private readonly AppDbContext _context = new();

    public MainWindow() {
        InitializeComponent();
        LoadProjects();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        _context.Database.Migrate();
    }

    protected override void OnClosing(CancelEventArgs e) {
        _context.Dispose();
        base.OnClosing(e);
    }

    private void LoadProjects() {
        ProjectTreeView.ItemsSource = _context.Projects.Include(p => p.DesignObjects).ToList();
    }

    private void ProjectTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
        if (ProjectTreeView.SelectedItem is Project selectedProject) {
            ProjectOverviewPanel.Visibility = Visibility.Visible;
            EmptyStatePanel.Visibility = Visibility.Collapsed;
            ProjectNameTextBlock.Text = selectedProject.Name;
        }
        else {
            ProjectOverviewPanel.Visibility = Visibility.Collapsed;
            EmptyStatePanel.Visibility = Visibility.Visible;
        }
    }

    private void CreateProjectButton_Click(object sender, RoutedEventArgs e) {
        var newProjectWindow = new NewProjectWindow(_context);
        if (newProjectWindow.ShowDialog() == true) LoadProjects();
    }

    private void AddDesignObjectButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectTreeView.SelectedItem is Project selectedProject) {
            var newDesignObjectWindow = new NewDesignObjectWindow(_context, selectedProject);
            if (newDesignObjectWindow.ShowDialog() == true) LoadProjects();
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }

    private void OpenLookupWindow_Click(object sender, RoutedEventArgs e) {
        if (sender is not MenuItem { Tag: string typeName }) return;
        if (!Enum.TryParse(typeName, out Constants.Lookups lookup)) return;
        var lookupWindow = new LookupManagementWindow(_context, lookup);
        lookupWindow.ShowDialog();
    }

    private void Help_Click(object sender, RoutedEventArgs e) {
        var helpWindow = new HelpWindow();
        helpWindow.ShowDialog();
    }

    private void DeleteProjectButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectTreeView.SelectedItem is Project selectedProject) {
            var result = MessageBox.Show($"Вы действительно хотите удалить проект '{selectedProject.Name}'?", "Удаление проекта", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) {
                try {
                    _context.Projects.Remove(selectedProject);
                    _context.SaveChanges();
                    LoadProjects();
                }
                catch (Exception ex) {
                    MessageBox.Show($"Ошибка при удалении проекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else {
            MessageBox.Show("Выберите проект для удаления.", "Удаление проекта", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
