namespace ProjectManagement.Models;

public class Project : BaseEntity {
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ContractorId { get; set; }
    public virtual Contractor Contractor { get; set; } = null!;
    public virtual ICollection<DesignObject> DesignObjects { get; set; } = new List<DesignObject>();
}
