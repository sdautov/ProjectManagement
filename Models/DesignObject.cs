namespace ProjectManagement.Models;

public class DesignObject : BaseEntity {
    public int ProjectId { get; set; }
    public int? ParentObjectId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ContractorId { get; set; }
    public virtual Contractor Contractor { get; set; } = null!;
    public virtual ICollection<DocumentationSet> DocumentationSets { get; set; } = new List<DocumentationSet>();
    public virtual ICollection<DesignObject> InverseParentObject { get; set; } = new List<DesignObject>();
    public virtual DesignObject? ParentObject { get; set; }
    public virtual Project Project { get; set; } = null!;
}
