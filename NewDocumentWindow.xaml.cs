﻿using System.Windows;
using ProjectManagement.Models;
using ProjectManagement.Services;

namespace ProjectManagement;

public partial class NewDocumentWindow {
    private readonly AppDbContext _context;
    private readonly DocumentationSet _documentationSet;
    private readonly DocumentService _documentService;

    public NewDocumentWindow(AppDbContext context, DocumentationSet selectedSet) {
        InitializeComponent();
        _context = context;
        LoadDocumentTypes();
        _documentService = new DocumentService(context);
        _documentationSet = selectedSet;
    }

    private void LoadDocumentTypes() {
        DocumentTypeComboBox.ItemsSource = _context.DocumentTypes.ToList();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
        if (DocumentTypeComboBox.SelectedItem is not DocumentType selectedMark) {
            MessageBox.Show("Необходимо выбрать тип документа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var docName = DocumentNameTextBox.Text;
        if (string.IsNullOrEmpty(docName)) {
            MessageBox.Show("Название объекта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        var newDocument = new Document {
            DocumentType = selectedMark,
            Name = docName,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now,
            DocumentationSetId = _documentationSet.Id
        };
        _context.SaveChanges();
        
        _documentService.AddDocument(newDocument);
        DialogResult = true;
    }
}
