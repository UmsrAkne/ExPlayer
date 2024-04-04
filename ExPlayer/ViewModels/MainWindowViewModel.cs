using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExPlayer.Models;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        public MainWindowViewModel()
        {
            const string path = "C:\\";
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            FileListViewModel = new FileListViewModel();

            FileListViewModel.Files.AddRange(
                files.Select(f => new FileInfoWrapper() { FileSystemInfo = new FileInfo(f), }));

            FileListViewModel.Files.AddRange(
                dirs.Select(d => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(d), }));
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public FileListViewModel FileListViewModel { get; set; }
    }
}