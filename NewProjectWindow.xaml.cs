using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class NewProjectWindow {
    private readonly AppDbContext _context;

    public NewProjectWindow(AppDbContext context) {
        InitializeComponent();
        _context = context;
        LoadContractors();
    }

    private void LoadContractors() {
        ContractorComboBox.ItemsSource = _context.Contractors.ToList();
    }

    private void CreateProjectButton_Click(object sender, RoutedEventArgs e) {
        var projectName = ProjectNameTextBox.Text;
        var projectCode = ProjectCodeTextBox.Text;
        var selectedContractor = ContractorComboBox.SelectedItem as Contractor;

        if (string.IsNullOrEmpty(projectName)) {
            MessageBox.Show("Название проекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrEmpty(projectCode)) {
            MessageBox.Show("Код проекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (selectedContractor == null) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newProject = new Project {
            Name = projectName,
            Code = projectCode,
            Contractor = selectedContractor,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now
        };

        _context.Projects.Add(newProject);
        _context.SaveChanges();

        DialogResult = true;
        Close();
    }
}
