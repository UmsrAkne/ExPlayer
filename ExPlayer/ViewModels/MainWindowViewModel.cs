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
            FileListViewModel.MoveDirectory(CurrentDirectoryPath);
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string CurrentDirectoryPath
        {
            get => currentDirectoryPath;
            set => SetProperty(ref currentDirectoryPath, value);
        }

        public FileListViewModel FileListViewModel { get; set; }
    }
}