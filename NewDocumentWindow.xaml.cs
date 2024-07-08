using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class NewDocumentWindow {
    private readonly AppDbContext _context;

    public NewDocumentWindow(AppDbContext context) {
        InitializeComponent();
        _context = context;
        LoadDocumentTypes();
    }

    public DocumentType DocumentType { get; set; }
    public int Number { get; set; }
    public string DocumentName { get; set; }

    private void LoadDocumentTypes() {
        DocumentTypeComboBox.ItemsSource = _context.DocumentTypes.ToList();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) {
        if (DocumentTypeComboBox.SelectedItem is not DocumentType selectedMark) {
            MessageBox.Show("Необходимо выбрать тип документа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var docName = DocumentNameTextBox.Text;
        if (string.IsNullOrEmpty(docName)) {
            MessageBox.Show("Название объекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        DocumentType = selectedMark;
        Number = int.Parse(DocumentNumberTextBox.Text);
        DocumentName = docName;
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
    }
}
