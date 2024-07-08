using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class EditDocumentationSetWindow : Window {
    private readonly AppDbContext _context;
    private readonly DocumentationSet _documentationSet;

    public EditDocumentationSetWindow(AppDbContext context, DocumentationSet documentationSet) {
        InitializeComponent();
        _context = context;
        _documentationSet = documentationSet;
        LoadMarks();
        MarkComboBox.SelectedItem = _documentationSet.Mark;
        NumberTextBox.Text = _documentationSet.Number.ToString();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
        var regex = NumericRegex();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void LoadMarks() {
        MarkComboBox.ItemsSource = _context.Marks.ToList();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (MarkComboBox.SelectedItem is not Mark selectedMark) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        _documentationSet.Mark = selectedMark;
        _documentationSet.Number = int.TryParse(NumberTextBox.Text, out var number) ? number : 0;
        _documentationSet.ModificationDate = DateTime.Now;
        _context.DocumentationSets.Update(_documentationSet);
        _context.SaveChanges();
        DialogResult = true;
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericRegex();
}
