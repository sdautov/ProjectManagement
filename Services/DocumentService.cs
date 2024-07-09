using ProjectManagement.Models;

namespace ProjectManagement.Services;

public class DocumentService {
    private readonly AppDbContext _context;

    public DocumentService(AppDbContext context) {
        _context = context;
    }

    public void AddDocument(Document document) {
        var maxNumber = _context.Documents
            .Where(ds => ds.DocumentationSetId == document.DocumentationSetId)
            .Max(ds => (int?)ds.Number);

        if (maxNumber != null) {
            document.Number = (int)(maxNumber + 1);
        }
        else {
            document.Number = 1;
        }

        _context.Documents.Add(document);
        _context.SaveChanges();
    }
}
