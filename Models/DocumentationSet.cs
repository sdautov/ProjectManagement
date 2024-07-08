﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models;

public class DocumentationSet : BaseEntity {
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
            ? $"{DesignObject.Project.Code}-{DesignObject.ParentObject?.Code}-{Mark.ShortName}{Number}"
            : $"{DesignObject.Project.Code}-{DesignObject.ParentObject?.Code}-{Mark.ShortName}";

    [NotMapped] public string ProjectCode => DesignObject.Project.Code;

    [NotMapped] public string DesignObjectCode => DesignObject.FullDesignObjectCode;

    [NotMapped] public Contractor Contractor => DesignObject.Contractor;
}
