using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExPlayer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private string currentDirectoryPath;

        public MainWindowViewModel()
        {
            CurrentDirectoryPath = "C:\\";
            FileListViewModel = new FileListViewModel();
            MoveDirectory(CurrentDirectoryPath);
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string CurrentDirectoryPath
        {
            get => currentDirectoryPath;
            set => SetProperty(ref currentDirectoryPath, value);
        }

        public FileListViewModel FileListViewModel { get; set; }

        public DelegateCommand MoveDirectoryCommand => new DelegateCommand(() =>
        {
            if (FileListViewModel.SelectedItem is not { IsDirectory: true, })
            {
                return;
            }

            MoveDirectory(FileListViewModel.SelectedItem.FileSystemInfo.FullName);
        });

        private void MoveDirectory(string path)
        {
            FileListViewModel.Files.Clear();

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            FileListViewModel.Files.AddRange(
                files.Select(f => new FileInfoWrapper() { FileSystemInfo = new FileInfo(f), }));

            FileListViewModel.Files.AddRange(
                dirs.Select(d => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(d), }));
        }
    }
}