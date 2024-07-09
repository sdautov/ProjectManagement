using System.Collections.ObjectModel;

namespace ProjectManagement.ViewModels;

public interface ITreeViewItem {
    public string Title { get; }
    public ObservableCollection<ITreeViewItem> Children { get; set; }
}
