namespace ProjectManagement.Models;

public class DocumentationSet : BaseEntity {
    public int DesignObjectId { get; set; }
    public int MarkId { get; set; }
    public int Number { get; set; }
    public virtual DesignObject DesignObject { get; set; } = null!;
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual Mark Mark { get; set; } = null!;
}
