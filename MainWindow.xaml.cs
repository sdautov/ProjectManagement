using System.ComponentModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement;

public partial class MainWindow {
    private readonly AppDbContext _context = new();

    public MainWindow() {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        _context.Database.Migrate();
        // _context.Database.EnsureCreated();
    }

    protected override void OnClosing(CancelEventArgs e) {
        _context.Dispose();
        base.OnClosing(e);
    }
}
