using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models;

public class Document : BaseEntity {
    public int DocumentationSetId { get; set; }
    public int DocumentTypeId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = null!;
    public virtual DocumentType DocumentType { get; set; } = null!;
    public virtual DocumentationSet DocumentationSet { get; set; } = null!;

    [NotMapped] public string FullDocumentCode => $"{DocumentType}-{Number}";
}
