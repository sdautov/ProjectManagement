namespace ProjectManagement.Models;

public abstract class BaseEntity : BaseModel {
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
}
