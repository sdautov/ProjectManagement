using System.Windows;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class EditLookupWindow {
    private readonly AppDbContext _context;
    private readonly BaseLookup _item;
    private readonly Constants.Lookups _lookup;

    public EditLookupWindow(AppDbContext context, Constants.Lookups lookup, BaseLookup? item = null) {
        InitializeComponent();
        _context = context;
        _lookup = lookup;
        _item = item ?? GetNewLookup();
    }

    private BaseLookup GetNewLookup() {
        return _lookup switch {
            Constants.Lookups.Contractor => new Contractor(),
            Constants.Lookups.Mark => new Mark(),
            Constants.Lookups.DocumentType => new DocumentType(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        _item.ShortName = ShortNameTextBox.Text;
        _item.FullName = FullNameTextBox.Text;
        if (_item.Id == 0)
            switch (_lookup) {
                case Constants.Lookups.Contractor:
                    _context.Set<Contractor>().Add((_item as Contractor)!);
                    break;
                case Constants.Lookups.Mark:
                    _context.Set<Mark>().Add((_item as Mark)!);
                    break;
                case Constants.Lookups.DocumentType:
                    _context.Set<DocumentType>().Add((_item as DocumentType)!);
                    break;
            }
        _context.SaveChanges();
        DialogResult = true;
        Close();
    }
}
