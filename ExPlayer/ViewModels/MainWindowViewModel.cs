using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";

        public MainWindowViewModel()
        {
            FileListViewModel = new FileListViewModel();
            FileListViewModel.MoveDirectory("C:\\");
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public FileListViewModel FileListViewModel { get; set; }
    }
}