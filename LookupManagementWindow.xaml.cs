using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class LookupManagementWindow {
    private readonly AppDbContext _context;
    private readonly Constants.Lookups _lookup;

    public LookupManagementWindow(AppDbContext context, Constants.Lookups lookup) {
        InitializeComponent();
        _context = context;
        _lookup = lookup;
        LoadData();
    }

    private void LoadData() {
        switch (_lookup) {
            case Constants.Lookups.Contractor:
                LookupListView.ItemsSource = _context.Contractors.ToList();
                break;
            case Constants.Lookups.Mark:
                LookupListView.ItemsSource = _context.Marks.ToList();
                break;
            case Constants.Lookups.DocumentType:
                LookupListView.ItemsSource = _context.DocumentTypes.ToList();
                break;
        }
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e) {
        var window = new EditLookupWindow(_context, _lookup);
        if (window.ShowDialog() == true) LoadData();
    }

    private void EditItemButton_Click(object sender, RoutedEventArgs e) {
        if (LookupListView.SelectedItem is null) return;
        EditLookupWindow window;
        switch (_lookup) {
            case Constants.Lookups.Contractor:
                window = new EditLookupWindow(_context, _lookup, LookupListView.SelectedItem as Contractor);
                if (window.ShowDialog() == true) LoadData();
                break;
            case Constants.Lookups.Mark:
                window = new EditLookupWindow(_context, _lookup, LookupListView.SelectedItem as Mark);
                if (window.ShowDialog() == true) LoadData();
                break;
            case Constants.Lookups.DocumentType:
                window = new EditLookupWindow(_context, _lookup, LookupListView.SelectedItem as DocumentType);
                if (window.ShowDialog() == true) LoadData();
                break;
        }
    }

    private void DeleteItemButton_Click(object sender, RoutedEventArgs e) {
        if (LookupListView.SelectedItem is null) return;
        switch (_lookup) {
            case Constants.Lookups.Contractor:
                _context.Contractors.Remove((LookupListView.SelectedItem as Contractor)!);
                break;
            case Constants.Lookups.Mark:
                _context.Marks.Remove((LookupListView.SelectedItem as Mark)!);
                break;
            case Constants.Lookups.DocumentType:
                _context.DocumentTypes.Remove((LookupListView.SelectedItem as DocumentType)!);
                break;
        }
        _context.SaveChanges();
        LoadData();
    }
}
