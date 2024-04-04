using System.Collections.ObjectModel;
using ExPlayer.Models;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FileListViewModel : BindableBase
    {
        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();
    }
}