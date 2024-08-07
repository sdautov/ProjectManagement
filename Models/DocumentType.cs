﻿namespace ProjectManagement.Models;

public class DocumentType : BaseLookup {
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
