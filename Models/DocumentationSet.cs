using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Models;

public class DocumentationSet : BaseEntity, ITreeViewItem {
    public int DesignObjectId { get; set; }
    public int MarkId { get; set; }
    public int Number { get; set; }
    public virtual DesignObject DesignObject { get; set; } = null!;
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual Mark Mark { get; set; } = null!;
    [NotMapped] public string Code => Number.ToString();

    [NotMapped]
    public string FullSetCode =>
        Number > 0
            ? $"{DesignObject.Project.Code}-{DesignObject.FullDesignObjectCode}-{Mark.ShortName}{Number}"
            : $"{DesignObject.Project.Code}-{DesignObject.FullDesignObjectCode}-{Mark.ShortName}";

    [NotMapped] public string ProjectCode => DesignObject.Project.Code;
    [NotMapped] public string DesignObjectCode => DesignObject.FullDesignObjectCode;
    [NotMapped] public Contractor Contractor => DesignObject.Contractor;
    [NotMapped] public string Title => FullSetCode;
    [NotMapped] public ObservableCollection<ITreeViewItem> Children { get; set; } = [];
}
