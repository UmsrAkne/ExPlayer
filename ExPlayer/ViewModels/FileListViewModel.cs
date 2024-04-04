using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExPlayer.Models;
using Prism.Commands;
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

        public DelegateCommand MoveDirectoryCommand => new DelegateCommand(() =>
        {
            if (SelectedItem is not { IsDirectory: true, })
            {
                return;
            }

            MoveDirectory(SelectedItem.FileSystemInfo.FullName);
        });

        public void MoveDirectory(string path)
        {
            Files.Clear();

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            Files.AddRange(
                files.Select(f => new FileInfoWrapper() { FileSystemInfo = new FileInfo(f), }));

            Files.AddRange(
                dirs.Select(d => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(d), }));
        }
    }
}