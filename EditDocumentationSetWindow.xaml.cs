using System.Windows;
using ProjectManagement.Models;
using ProjectManagement.Services;

namespace ProjectManagement;

public partial class EditDocumentationSetWindow {
    private readonly AppDbContext _context;
    private readonly DocumentationSet? _currentSet;
    private readonly DesignObject _designObject;
    private readonly DocumentationSetService _documentationSetService;

    public EditDocumentationSetWindow(AppDbContext context, DesignObject designObject) {
        InitializeComponent();
        _context = context;
        _designObject = designObject;
        _documentationSetService = new DocumentationSetService(context);
        LoadMarks();
    }

    public EditDocumentationSetWindow(AppDbContext context, DocumentationSet currentSet) {
        InitializeComponent();
        _context = context;
        _designObject = currentSet.DesignObject;
        _currentSet = currentSet;
        _documentationSetService = new DocumentationSetService(context);
        LoadMarks();
        FillValues();
    }

    private void FillValues() {
        MarkComboBox.SelectedItem = _currentSet!.Mark;
        Title = _currentSet.FullSetCode;
    }

    private void LoadMarks() {
        MarkComboBox.ItemsSource = _context.Marks.ToList();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (MarkComboBox.SelectedItem is not Mark selectedMark) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (_currentSet != null) {
            _currentSet.Mark = selectedMark;
            _currentSet.ModificationDate = DateTime.Now;
            _documentationSetService.EditDocumentationSet(_currentSet);
        }
        else {
            var newSet = new DocumentationSet {
                Mark = selectedMark,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                DesignObjectId = _designObject.Id
            };
            _documentationSetService.AddDocumentationSet(newSet);
        }
        DialogResult = true;
    }
}
