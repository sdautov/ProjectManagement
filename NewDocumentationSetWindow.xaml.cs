using System.Windows;
using ProjectManagement.Models;
using ProjectManagement.Services;

namespace ProjectManagement;

public partial class NewDocumentationSetWindow {
    private readonly AppDbContext _context;
    private readonly DesignObject _designObject;
    private readonly DocumentationSetService _documentationSetService;

    public NewDocumentationSetWindow(AppDbContext context, DesignObject designObject) {
        InitializeComponent();
        _context = context;
        _designObject = designObject;
        LoadMarks();
        _documentationSetService = new DocumentationSetService(context);
    }

    private void LoadMarks() {
        MarkComboBox.ItemsSource = _context.Marks.ToList();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (MarkComboBox.SelectedItem is not Mark selectedMark) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newSet = new DocumentationSet {
            Mark = selectedMark,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now,
            DesignObjectId = _designObject.Id
        };

        _documentationSetService.AddDocumentationSet(newSet);
        DialogResult = true;
    }
}
