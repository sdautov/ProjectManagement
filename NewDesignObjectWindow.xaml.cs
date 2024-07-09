using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class NewDesignObjectWindow {
    private readonly AppDbContext _context;
    private readonly DesignObject? _parentObject;
    private readonly Project _project;

    public NewDesignObjectWindow(AppDbContext context, Project project) {
        InitializeComponent();
        _context = context;
        _project = project;
        _parentObject = null;
        LoadContractors();
    }

    public NewDesignObjectWindow(AppDbContext context, DesignObject parentObject) {
        InitializeComponent();
        _context = context;
        _project = parentObject.Project;
        _parentObject = parentObject;
        LoadContractors();
    }

    private void LoadContractors() {
        ContractorComboBox.ItemsSource = _context.Contractors.ToList();
    }

    private void CreateDesignObjectButton_Click(object sender, RoutedEventArgs e) {
        var objectName = DesignObjectNameTextBox.Text;
        var objectCode = DesignObjectCodeTextBox.Text;
        var selectedContractor = ContractorComboBox.SelectedItem as Contractor;

        if (string.IsNullOrEmpty(objectName)) {
            MessageBox.Show("Название объекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrEmpty(objectCode)) {
            MessageBox.Show("Код объекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (selectedContractor == null) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newDesignObject = new DesignObject {
            Name = objectName,
            Code = objectCode,
            Contractor = selectedContractor,
            ParentObjectId = _parentObject?.Id,
            ProjectId = _project.Id,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now
        };
        _context.DesignObjects.Add(newDesignObject);
        _context.SaveChanges();
        DialogResult = true;
        Close();
    }
}
