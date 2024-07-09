using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Models;

public class Project : BaseEntity, ITreeViewItem {
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int ContractorId { get; set; }
    public virtual Contractor Contractor { get; set; } = null!;
    public virtual ICollection<DesignObject> DesignObjects { get; set; } = new List<DesignObject>();
    [NotMapped] public string Title => Name;
    [NotMapped] public ObservableCollection<ITreeViewItem> Children { get; set; } = [];
}
