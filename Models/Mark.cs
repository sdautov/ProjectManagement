namespace ProjectManagement.Models;

public class Mark : BaseLookup {
    public new string ShortName { get; set; } = null!;
    public new string FullName { get; set; } = null!;
    public virtual ICollection<DocumentationSet> DocumentationSets { get; set; } = new List<DocumentationSet>();

    public override string ToString() {
        return ShortName;
    }
}
