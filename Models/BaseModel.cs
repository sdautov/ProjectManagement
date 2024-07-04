namespace ProjectManagement.Models;

public abstract class BaseModel {
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
