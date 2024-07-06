using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class NewDesignObjectWindow {
    private readonly AppDbContext _context;
    private readonly Project _project;

    public NewDesignObjectWindow(AppDbContext context, Project project) {
        InitializeComponent();
        _context = context;
        _project = project;
    }

    private void CreateDesignObjectButton_Click(object sender, RoutedEventArgs e) {
        var objectName = DesignObjectNameTextBox.Text;
        var objectCode = DesignObjectCodeTextBox.Text;
        if (string.IsNullOrEmpty(objectName) || string.IsNullOrEmpty(objectCode)) {
            MessageBox.Show("Название и код объекта не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var newDesignObject = new DesignObject {
            Name = objectName,
            Code = objectCode,
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
