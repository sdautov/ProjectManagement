using System.Windows;
using ProjectManagement.Models;
using ProjectManagement.Services;

namespace ProjectManagement;

public partial class EditDocumentWindow {
    private readonly AppDbContext _context;
    private readonly Document? _currentDoc;
    private readonly DocumentationSet _documentationSet;
    private readonly DocumentService _documentService;

    public EditDocumentWindow(AppDbContext context, DocumentationSet selectedSet) {
        InitializeComponent();
        _context = context;
        _documentationSet = selectedSet;
        _documentService = new DocumentService(context);
        LoadDocumentTypes();
    }

    public EditDocumentWindow(AppDbContext context, Document currentDoc) {
        InitializeComponent();
        _context = context;
        _documentationSet = currentDoc.DocumentationSet;
        _currentDoc = currentDoc;
        _documentService = new DocumentService(context);
        LoadDocumentTypes();
        FillValues();
    }

    private void FillValues() {
        DocumentTypeComboBox.SelectedItem = _currentDoc!.DocumentType;
        DocumentNameTextBox.Text = _currentDoc!.Name;
        Title = _currentDoc!.Name;
    }

    private void LoadDocumentTypes() {
        DocumentTypeComboBox.ItemsSource = _context.DocumentTypes.ToList();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (DocumentTypeComboBox.SelectedItem is not DocumentType selectedDocumentType) {
            MessageBox.Show("Необходимо выбрать тип документа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var docName = DocumentNameTextBox.Text;
        if (string.IsNullOrEmpty(docName)) {
            MessageBox.Show("Название объекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (_currentDoc != null) {
            _currentDoc.DocumentType = selectedDocumentType;
            _currentDoc.Name = docName;
            _currentDoc.ModificationDate = DateTime.Now;
            _documentService.EditDocument(_currentDoc);
        }
        else {
            var newDocument = new Document {
                DocumentType = selectedDocumentType,
                Name = docName,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                DocumentationSetId = _documentationSet.Id
            };
            _documentService.AddDocument(newDocument);
        }
        DialogResult = true;
    }
}
