using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class NewDocumentationSetWindow : Window {
    private readonly AppDbContext _context;
    private readonly DesignObject _designObject;

    public NewDocumentationSetWindow(AppDbContext context, DesignObject designObject) {
        InitializeComponent();
        _context = context;
        _designObject = designObject;
        LoadMarks();
    }

    private void LoadMarks() {
        MarkComboBox.ItemsSource = _context.Marks.ToList();
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
        var regex = NumericRegex();
        e.Handled = regex.IsMatch(e.Text);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (MarkComboBox.SelectedItem is not Mark selectedMark) {
            MessageBox.Show("Необходимо выбрать подрядчика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var newSet = new DocumentationSet {
            Mark = selectedMark,
            Number = int.TryParse(NumberTextBox.Text, out var number) ? number : 0,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now,
            DesignObject = _designObject
        };

        _context.DocumentationSets.Add(newSet);
        _context.SaveChanges();
        DialogResult = true;
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericRegex();
}
