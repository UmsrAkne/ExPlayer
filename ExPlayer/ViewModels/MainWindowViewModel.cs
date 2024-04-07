using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ExPlayer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly DispatcherTimer timer;
        private string title = "ExPlayer";
        private string currentDirectoryPath;

        public MainWindowViewModel()
        {
            CurrentDirectoryPath = "C:\\";
            FileListViewModel = new FileListViewModel();
            MoveDirectory(CurrentDirectoryPath);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200),
            };

            timer.Tick += (_, _) =>
            {
                PlaybackPosition = AudioPlayer.Position;
                AudioLength = AudioPlayer.Length;
                RaisePropertyChanged(nameof(PlaybackPosition));
                RaisePropertyChanged(nameof(AudioLength));
            };

            timer.Start();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string CurrentDirectoryPath
        {
            get => currentDirectoryPath;
            set => SetProperty(ref currentDirectoryPath, value);
        }

        public long PlaybackPosition { get; set; }

        public long AudioLength { get; set; }

        public FileListViewModel FileListViewModel { get; set; }

        public DelegateCommand OpenCommand => new DelegateCommand(() =>
        {
            if (FileListViewModel.SelectedItem == null)
            {
                return;
            }

            if (FileListViewModel.SelectedItem.IsSoundFile())
            {
                Play();
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

        public DelegateCommand StopCommand => new (() =>
        {
            AudioPlayer.Stop();
        });

        private AudioPlayer AudioPlayer { get; set; } = new ();

        private void Play()
        {
            if (!FileListViewModel.SelectedItem.IsSoundFile())
            {
                return;
            }

            AudioPlayer.Play(FileListViewModel.SelectedItem.FileSystemInfo.FullName);
        }

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