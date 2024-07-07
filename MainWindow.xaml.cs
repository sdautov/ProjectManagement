using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class MainWindow : INotifyPropertyChanged {
    private readonly AppDbContext _context = new();
    private ObservableCollection<Project> _projects;

    public MainWindow() {
        InitializeComponent();
        DataContext = this;
        LoadProjects();
    }

    public ObservableCollection<Project> Projects {
        get => _projects;
        set {
            if (_projects != value) {
                _projects = value;
                OnPropertyChanged(nameof(Projects));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        _context.Database.Migrate();
    }

    protected override void OnClosing(CancelEventArgs e) {
        _context.Dispose();
        base.OnClosing(e);
    }

    private void LoadProjects() {
        var projects = _context.Projects
            .Include(p => p.DesignObjects)
            .ThenInclude(d => d.InverseParentObject)
            .ToList();
        foreach (var project in projects) {
            foreach (var designObject in project.DesignObjects.ToList()) {
                RemoveChildDesignObjects(project.DesignObjects, designObject);
            }
        }

        Projects = new ObservableCollection<Project>(projects);
    }

    private void RemoveChildDesignObjects(ICollection<DesignObject> designObjects, DesignObject parentObject) {
        foreach (var childObject in parentObject.InverseParentObject.ToList()) {
            designObjects.Remove(childObject);
            RemoveChildDesignObjects(designObjects, childObject);
        }
    }

    private void ProjectTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
        if (ProjectTreeView.SelectedItem is Project selectedProject)
            ShowProjectOverview(selectedProject);
        else if (ProjectTreeView.SelectedItem is DesignObject selectedDesignObject)
            ShowDesignObjectOverview(selectedDesignObject);
        else if (ProjectTreeView.SelectedItem is DocumentationSet selectedSet)
            ShowDocumentationSetOverview(selectedSet);
        else
            HideAllPanels();
    }

    private void ShowProjectOverview(Project project) {
        ProjectOverviewPanel.Visibility = Visibility.Visible;
        DocumentationSetPanel.Visibility = Visibility.Collapsed;
        EmptyStatePanel.Visibility = Visibility.Collapsed;

        ProjectNameTextBlock.Text = project.Name;

        var projectData = project.DesignObjects.SelectMany(obj => obj.DocumentationSets.Select(set => new {
            ProjectCode = project.Code,
            FullObjectCode = obj.ParentObject.Code != null ? $"{obj.ParentObject.Code}.{obj.Code}" : obj.Code,
            set.Mark,
            set.Number,
            FullSetCode = set.Number > 0
                ? $"{project.Code}-{obj.ParentObject.Code ?? obj.Code}-{set.Mark}{set.Number}"
                : $"{project.Code}-{obj.ParentObject.Code ?? obj.Code}-{set.Mark}",
            Contractor = obj.Contractor ?? project.Contractor,
            set.CreationDate,
            set.ModificationDate
        })).ToList();

        ProjectDataGrid.ItemsSource = projectData;
    }

    private void ShowDesignObjectOverview(DesignObject designObject) {
        ProjectOverviewPanel.Visibility = Visibility.Visible;
        DocumentationSetPanel.Visibility = Visibility.Collapsed;
        EmptyStatePanel.Visibility = Visibility.Collapsed;

        ProjectNameTextBlock.Text = designObject.Name;

        var projectData = designObject.DocumentationSets.Select(set => new {
            ProjectCode = designObject.Project.Code,
            FullObjectCode = designObject.ParentObject.Code != null ? $"{designObject.ParentObject.Code}.{designObject.Code}" : designObject.Code,
            set.Mark,
            set.Number,
            FullSetCode = set.Number > 0
                ? $"{designObject.Project.Code}-{designObject.ParentObject.Code ?? designObject.Code}-{set.Mark}{set.Number}"
                : $"{designObject.Project.Code}-{designObject.ParentObject.Code ?? designObject.Code}-{set.Mark}",
            Contractor = designObject.Contractor ?? designObject.Project.Contractor,
            set.CreationDate,
            set.ModificationDate
        }).ToList();

        ProjectDataGrid.ItemsSource = projectData;
    }

    private void ShowDocumentationSetOverview(DocumentationSet set) {
        ProjectOverviewPanel.Visibility = Visibility.Collapsed;
        DocumentationSetPanel.Visibility = Visibility.Visible;
        EmptyStatePanel.Visibility = Visibility.Collapsed;

        var documentationData = set.Documents.Select(doc => new {
            doc.DocumentType,
            doc.Number,
            doc.Name,
            FullDocumentCode = $"-{doc.DocumentType}-{doc.Number}",
            doc.CreationDate,
            doc.ModificationDate
        }).ToList();

        DocumentationDataGrid.ItemsSource = documentationData;
    }

    private void HideAllPanels() {
        ProjectOverviewPanel.Visibility = Visibility.Collapsed;
        DocumentationSetPanel.Visibility = Visibility.Collapsed;
        EmptyStatePanel.Visibility = Visibility.Visible;
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
        else if (ProjectTreeView.SelectedItem is DesignObject selectedObject) {
            var newDesignObjectWindow = new NewDesignObjectWindow(_context, null, selectedObject);
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
            if (result == MessageBoxResult.Yes)
                try {
                    _context.Projects.Remove(selectedProject);
                    _context.SaveChanges();
                    LoadProjects();
                }
                catch (Exception ex) {
                    MessageBox.Show($"Ошибка при удалении проекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        else {
            MessageBox.Show("Выберите проект для удаления.", "Удаление проекта", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    protected virtual void OnPropertyChanged(string propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
