using System.Collections.ObjectModel;
using ExPlayer.Models;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FileListViewModel : BindableBase
    {
        private FileInfoWrapper selectedItem;

        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();

        public FileInfoWrapper SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
}