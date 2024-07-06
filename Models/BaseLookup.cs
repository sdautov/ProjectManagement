namespace ProjectManagement.Models;

public abstract class BaseLookup : BaseModel {
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
}
