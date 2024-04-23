using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ExPlayer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private readonly DatabaseContext databaseContext;
        private readonly DispatcherTimer timer;
        private string title = "ExPlayer";
        private string currentDirectoryPath;
        private long playbackPosition;
        private string message;

        public MainWindowViewModel()
        {
            databaseContext = new DatabaseContext();
            databaseContext.Database.EnsureCreated();

            CurrentDirectoryPath = "C:\\";
            SetCurrentDirectory(@"C:\MyFiles\temp"); // デバッグビルドの時にだけ実行されるメソッド

            // 最後に開いていたディレクトリを取得する
            var lastVisitedDir = databaseContext.OpenedDirectoryHistory
                .OrderByDescending(d => d.OpenDateTime)
                .FirstOrDefault();

            if (lastVisitedDir != null)
            {
                CurrentDirectoryPath = lastVisitedDir.FullName;
            }

            FileListViewModel = new FileListViewModel();
            MoveDirectory(CurrentDirectoryPath);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200),
            };

            timer.Tick += (_, _) =>
            {
                AudioLength = AudioPlayer.Length;
                RaisePropertyChanged(nameof(PlaybackPosition));
                RaisePropertyChanged(nameof(AudioLength));
            };

            AudioPlayer.PlayCompleted += (_, _) =>
            {
                if (FileListViewModel.AudioProvider.HasNext())
                {
                    Play(true);
                }
            };

            timer.Start();
        }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string CurrentDirectoryPath
        {
            get => currentDirectoryPath;
            set => SetProperty(ref currentDirectoryPath, value);
        }

        public long PlaybackPosition
        {
            get => AudioPlayer.GetCurrentTime();
            set
            {
                AudioPlayer.Seek(value);
                SetProperty(ref playbackPosition, value);
            }
        }

        public long AudioLength { get; set; }

        public FileListViewModel FileListViewModel { get; set; }

        public string Message { get => message; set => SetProperty(ref message, value); }

        public DelegateCommand OpenCommand => new DelegateCommand(() =>
        {
            if (FileListViewModel.SelectedItem == null)
            {
                return;
            }

            if (FileListViewModel.SelectedItem.IsSoundFile())
            {
                Play(false);
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
            if (Directory.Exists(CurrentDirectoryPath) || IsM3U(CurrentDirectoryPath))
            {
                MoveDirectory(CurrentDirectoryPath);
            }
        });

        public DelegateCommand StopCommand => new (() =>
        {
            AudioPlayer.Stop();
        });

        public DelegateCommand SavePlayingAudioInfoCommand => new DelegateCommand(() =>
        {
            if (AudioPlayer.CurrentFile == null)
            {
                return;
            }

            var a = databaseContext.ListenHistory
                .FirstOrDefault(f => f.FullName == AudioPlayer.CurrentFile.FullName);

            if (a != null)
            {
                a.PlaybackProgressTicks = TimeSpan.FromSeconds(AudioPlayer.GetCurrentTime()).Ticks;
                databaseContext.SaveChanges();
            }
        });

        public DelegateCommand ToggleIgnorePropertyCommand => new (() =>
        {
            var item = FileListViewModel.SelectedItem;
            if (item == null || !item.IsSoundFile())
            {
                return;
            }

            item.Ignore = !item.Ignore;

            var f = databaseContext.ListenHistory.FirstOrDefault(f => f.FullName == item.FullName);
            if (f == null)
            {
                return;
            }

            f.Ignore = item.Ignore;
            databaseContext.SaveChanges();
        });

        public DelegateCommand OpenFavoritesCommand => new DelegateCommand(() =>
        {
            var list = databaseContext.FavoriteDirectories
                .Select(f => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(f.FullName), });

            FileListViewModel.ReplaceFileInfoWrappers(list);
        });

        public DelegateCommand AddFavoriteDirectoryCommand => new DelegateCommand(() =>
        {
            if (CurrentDirectoryPath != null && Directory.Exists(CurrentDirectoryPath))
            {
                databaseContext.FavoriteDirectories.Add(new FavoriteDirectoryInfo()
                {
                    DirectoryInfo = new DirectoryInfo(CurrentDirectoryPath),
                });

                databaseContext.SaveChanges();
            }
        });

        private AudioPlayer AudioPlayer { get; set; } = new ();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            AudioPlayer.Dispose();
            databaseContext.Dispose();
        }

        private void Play(bool autoPlaying)
        {
            var item = FileListViewModel.SelectedItem;
            if (!item.IsSoundFile())
            {
                return;
            }

            if (!autoPlaying)
            {
                FileListViewModel.AudioProvider.FirstCall = true;
                FileListViewModel.AudioProvider.Index = FileListViewModel.SelectedIndex;
            }

            SavePlayingAudioInfoCommand.Execute();

            var sound = FileListViewModel.AudioProvider.GetNext();

            AudioPlayer.Play(sound);
            databaseContext.AddListenCount(sound);
        }

        private void MoveDirectory(string path)
        {
            CurrentDirectoryPath = path;

            // 開いたディレクトリの情報の記録処理
            databaseContext.AddDirectoryHistory(new DirectoryInfoWrapper()
            {
                OpenDateTime = DateTime.Now,
                FullName = Directory.Exists(path) ? new DirectoryInfo(path).FullName : path,
            });

            var pathIsM3UFile = IsM3U(path);
            var files = pathIsM3UFile ? M3UFileReader.GetFilesFrom(path) : GetFiles(path);
            var dirs = !pathIsM3UFile ? GetDirectories(path) : new List<FileInfoWrapper>();

            // 視聴回数を入力する処理
            // 作業ディレクトリのファイルリストと、現在のディレクトリパスで検索したファイルリストを辞書化する。
            // ファイル名をキーにして、２つのリストを結びつけて、 ListenCount を入力する。
            var listenCountList = databaseContext.ListenHistory.Where(l => l.ParentDirectoryPath == path).ToList();

            var la = files.Where(f => f.IsSoundFile()).ToDictionary(item => item.Name);
            var lb = listenCountList.ToDictionary(item => item.Name);

            foreach (var item in lb)
            {
                if (la.TryGetValue(item.Key, out var itemA))
                {
                    itemA.ListenCount = item.Value.ListenCount;
                    itemA.PlaybackProgressTicks = item.Value.PlaybackProgressTicks;
                    itemA.Ignore = item.Value.Ignore;

                    // ここで値が見つかった場合、DB 登録済みということなので、リストから消しておく
                    la.Remove(item.Key);
                }
            }

            // la には DB に未登録のファイルが残っている。全て新規登録する。
            databaseContext.AddRange(la.Values.ToList());

            FileListViewModel.ReplaceFileInfoWrappers(files.Concat(dirs));
        }

        /// <summary>
        /// path の中に入っているファイルを詰め込んだリストを取得します。
        /// </summary>
        /// <param name="path">ディレクトリのパスを入力します</param>
        /// <returns>入力されたパスのディレクトリの中にあるファイルのリストです。ディレクトリは含まれません。ディレクトリへのアクセスが失敗した場合は空のリストを返します。</returns>
        private List<FileInfoWrapper> GetFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path)
                    .Select(f => new FileInfoWrapper() { FileSystemInfo = new FileInfo(f), }).ToList();
            }
            catch (UnauthorizedAccessException e)
            {
                Message = $"{CurrentDirectoryPath} へのアクセスに失敗しました";
                Debug.WriteLine(e);
                return new List<FileInfoWrapper>();
            }
        }

        /// <summary>
        /// path の中に入っているディレクトリを詰め込んだリストを取得します。
        /// </summary>
        /// <param name="path">ディレクトリのパスを入力します</param>
        /// <returns>入力されたパスのディレクトリの中にあるディレクトリのリストです。ファイルは含まれません。ディレクトリへのアクセスが失敗した場合は空のリストを返します。</returns>
        private IEnumerable<FileInfoWrapper> GetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path)
                    .Select(d => new FileInfoWrapper() { FileSystemInfo = new DirectoryInfo(d), });
            }
            catch (UnauthorizedAccessException e)
            {
                Message = $"{CurrentDirectoryPath} へのアクセスに失敗しました";
                Debug.WriteLine(e);
                return new List<FileInfoWrapper>();
            }
        }

        private bool IsM3U(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Path.GetExtension(path).StartsWith(".m3u", StringComparison.OrdinalIgnoreCase);
        }

        [Conditional("DEBUG")]
        private void SetCurrentDirectory(string path) => CurrentDirectoryPath = path;
    }
}