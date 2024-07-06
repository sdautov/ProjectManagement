namespace ProjectManagement.Models;

public class Mark : BaseLookup {
    public virtual ICollection<DocumentationSet> DocumentationSets { get; set; } = new List<DocumentationSet>();
}
