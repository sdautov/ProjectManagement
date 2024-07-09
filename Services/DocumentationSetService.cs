using ProjectManagement.Models;

namespace ProjectManagement.Services;

public class DocumentationSetService {
    private readonly AppDbContext _context;

    public DocumentationSetService(AppDbContext context) {
        _context = context;
    }

    public void AddDocumentationSet(DocumentationSet documentationSet) {
        var maxNumber = _context.DocumentationSets
            .Where(ds => ds.DesignObjectId == documentationSet.DesignObjectId)
            .Max(ds => (int?)ds.Number);

        if (maxNumber != null)
            documentationSet.Number = (int)(maxNumber + 1);
        else
            documentationSet.Number = 0;

        _context.DocumentationSets.Add(documentationSet);
        _context.SaveChanges();
    }

    public void EditDocumentationSet(DocumentationSet documentationSet) {
        _context.DocumentationSets.Update(documentationSet);
        _context.SaveChanges();
    }
    
    public void DeleteDocumentationSet(DocumentationSet documentationSet) {
        _context.DocumentationSets.Remove(documentationSet);
        _context.SaveChanges();
    }
}
