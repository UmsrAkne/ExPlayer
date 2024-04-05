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

        public DelegateCommand OpenCommand => new DelegateCommand(() =>
        {
            if (FileListViewModel.SelectedItem == null)
            {
                return;
            }

            if (FileListViewModel.SelectedItem.IsSoundFile())
            {
                FileListViewModel.Play();
                return;
            }

            if (FileListViewModel.SelectedItem.IsDirectory)
            {
                MoveDirectory(FileListViewModel.SelectedItem.FileSystemInfo.FullName);
            }
        });

        public DelegateCommand MoveParentDirectoryCommand => new DelegateCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(CurrentDirectoryPath))
            {
                return;
            }

            var di = Directory.GetParent(CurrentDirectoryPath);
            if (di == null)
            {
                return;
            }

            MoveDirectory(di.FullName);
        });

        public DelegateCommand JumpToDirectoryCommand => new DelegateCommand(() =>
        {
            if (Directory.Exists(CurrentDirectoryPath))
            {
                MoveDirectory(CurrentDirectoryPath);
            }
        });

        private void MoveDirectory(string path)
        {
            CurrentDirectoryPath = path;

            var files = Directory.GetFiles(path)
                .Select(f => new FileInfoWrapper() { FileSystemInfo = new FileInfo(f), });

            var dirs = Directory.GetDirectories(path)
                .Select(d => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(d), });

            FileListViewModel.ReplaceFileInfoWrappers(files.Concat(dirs));
        }
    }
}