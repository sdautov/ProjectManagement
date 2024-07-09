using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement;

public partial class MainWindow : INotifyPropertyChanged {
    private readonly AppDbContext _context = new();
    private ObservableCollection<Project> _projects = [];

    public MainWindow() {
        _context.Database.Migrate();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnClosing(CancelEventArgs e) {
        _context.Dispose();
        base.OnClosing(e);
    }

    private void LoadProjects() {
        var projects = _context.Projects
            .Include(p => p.DesignObjects)
            .Include(p => p.Contractor)
            .ToList();

        foreach (var project in projects) {
            project.Children = new ObservableCollection<ITreeViewItem>(project.DesignObjects
                .Where(d => d.ParentObjectId == null)
                .ToList());

            foreach (var designObject in project.DesignObjects) LoadDesignObjectDetails(designObject);
        }

        Projects = new ObservableCollection<Project>(projects);
    }

    private void LoadDesignObjectDetails(DesignObject designObject) {
        _context.Entry(designObject)
            .Collection(d => d.InverseParentObject)
            .Load();

        _context.Entry(designObject)
            .Collection(d => d.DocumentationSets)
            .Query()
            .Include(ds => ds.Mark)
            .Include(ds => ds.Documents)
            .Load();

        _context.Entry(designObject)
            .Reference(d => d.Contractor)
            .Load();

        PopulateDesignObjectChildren(designObject);
    }

    private static void PopulateDesignObjectChildren(DesignObject designObject) {
        var children = new ObservableCollection<ITreeViewItem>();
        foreach (var childObject in designObject.InverseParentObject) {
            PopulateDesignObjectChildren(childObject);
            children.Add(childObject);
        }
        foreach (var documentationSet in designObject.DocumentationSets) children.Add(documentationSet);
        designObject.Children = children;
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
        var projectData = project.DesignObjects.SelectMany(obj => obj.DocumentationSets).ToList();
        ProjectDataGrid.ItemsSource = projectData;
    }

    private void ShowDesignObjectOverview(DesignObject designObject) {
        ProjectOverviewPanel.Visibility = Visibility.Visible;
        DocumentationSetPanel.Visibility = Visibility.Collapsed;
        EmptyStatePanel.Visibility = Visibility.Collapsed;
        ProjectNameTextBlock.Text = designObject.Name;
        var projectData = designObject.DocumentationSets.ToList();
        ProjectDataGrid.ItemsSource = projectData;
    }

    private void AddDocumentButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectTreeView.SelectedItem is DocumentationSet selectedSet) {
            var newDocumentWindow = new NewDocumentWindow(_context);
            if (newDocumentWindow.ShowDialog() == true) {
                var newDocument = new Document {
                    DocumentType = newDocumentWindow.DocumentType,
                    Number = newDocumentWindow.Number,
                    Name = newDocumentWindow.DocumentName,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now
                };
                selectedSet.Documents.Add(newDocument);
                _context.SaveChanges();
                DocumentationDataGrid.ItemsSource = selectedSet.Documents.ToList();
            }
        }
        else {
            MessageBox.Show("Выберите комплект для добавления документа.", "Добавление документа", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DeleteDocumentButton_Click(object sender, RoutedEventArgs e) {
        if (DocumentationDataGrid.SelectedItem is Document selectedDocument) {
            var result = MessageBox.Show($"Вы действительно хотите удалить документ '{selectedDocument.Name}'?", "Удаление документа", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) {
                _context.Documents.Remove(selectedDocument);
                _context.SaveChanges();
                LoadProjects();
            }
        }
        else {
            MessageBox.Show("Выберите документ для удаления.", "Удаление документа", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void EditDocumentButton_Click(object sender, RoutedEventArgs e) {
        if (DocumentationDataGrid.SelectedItem is Document selectedDocument) {
            var editDocumentWindow = new NewDocumentWindow(_context) {
                DocumentType = selectedDocument.DocumentType,
                Number = selectedDocument.Number,
                DocumentName = selectedDocument.Name
            };
            if (editDocumentWindow.ShowDialog() == true) {
                selectedDocument.DocumentType = editDocumentWindow.DocumentType;
                selectedDocument.Number = editDocumentWindow.Number;
                selectedDocument.Name = editDocumentWindow.DocumentName;
                selectedDocument.ModificationDate = DateTime.Now;
                _context.SaveChanges();
                LoadProjects();
                ShowDocumentationSetOverview(selectedDocument.DocumentationSet);
            }
        }
        else {
            MessageBox.Show("Выберите документ для редактирования.", "Редактирование документа", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ShowDocumentationSetOverview(DocumentationSet set) {
        ProjectOverviewPanel.Visibility = Visibility.Collapsed;
        DocumentationSetPanel.Visibility = Visibility.Visible;
        EmptyStatePanel.Visibility = Visibility.Collapsed;
        DocumentationDataGrid.ItemsSource = set.Documents.ToList();
    }

    private void HideAllPanels() {
        ProjectOverviewPanel.Visibility = Visibility.Collapsed;
        DocumentationSetPanel.Visibility = Visibility.Collapsed;
        EmptyStatePanel.Visibility = Visibility.Visible;
    }

    private void AddDocumentationSetButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectTreeView.SelectedItem is DesignObject selectedObject) {
            var newDocumentationSetWindow = new NewDocumentationSetWindow(_context, selectedObject);
            if (newDocumentationSetWindow.ShowDialog() == true) LoadProjects();
        }
        else {
            MessageBox.Show("Выберите объект проектирования для добавления комплекта.", "Добавление комплекта", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void DeleteDocumentationSetButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectDataGrid.SelectedItem is DocumentationSet selectedSet) {
            var result = MessageBox.Show($"Вы действительно хотите удалить комплект '{selectedSet.FullSetCode}'?", "Удаление комплекта", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                try {
                    _context.DocumentationSets.Remove(selectedSet);
                    _context.SaveChanges();
                    LoadProjects();
                }
                catch (Exception ex) {
                    MessageBox.Show($"Ошибка при удалении комплекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        else {
            MessageBox.Show("Выберите комплект для удаления.", "Удаление комплекта", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void EditDocumentationSetButton_Click(object sender, RoutedEventArgs e) {
        if (ProjectDataGrid.SelectedItem is DocumentationSet selectedSet) {
            var editDocumentationSetWindow = new EditDocumentationSetWindow(_context, selectedSet);
            if (editDocumentationSetWindow.ShowDialog() == true) {
                LoadProjects();
                if (ProjectTreeView.SelectedItem is DesignObject selectedDesignObject) ShowDesignObjectOverview(selectedDesignObject);
            }
        }
        else {
            MessageBox.Show("Выберите комплект для редактирования.", "Редактирование комплекта", MessageBoxButton.OK, MessageBoxImage.Information);
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

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
