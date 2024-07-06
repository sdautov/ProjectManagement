namespace ProjectManagement.Models;

public class Contractor : BaseLookup {
    public virtual ICollection<DesignObject> DesignObjects { get; set; } = new List<DesignObject>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
