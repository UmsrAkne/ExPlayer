using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ExPlayer.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExPlayer.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FileListViewModel : BindableBase, IDisposable
    {
        private readonly AudioPlayer audioPlayer = new ();
        private FileInfoWrapper selectedItem;
        private bool includeAllFiles = true;

        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();

        public FileInfoWrapper SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        /// <summary>
        /// ReplaceFileInfoWrappers() で値を入力する際、サウンドファイルやディレクトリ以外のファイルもリストに入力するかを設定します。
        /// </summary>
        /// <value>
        /// デフォルトは true です。この状態は、全てのファイルの入力を受け付けます。
        /// </value>
        public bool IncludeAllFiles { get => includeAllFiles; set => SetProperty(ref includeAllFiles, value); }

        public DelegateCommand StopCommand => new (() =>
        {
            audioPlayer.Stop();
        });

        public long Position => audioPlayer.Position;

        public long AudioLength => audioPlayer.Length;

        /// <summary>
        /// Files のリストを、入力したリストに置き換えます。
        /// IncludeAllFiles == true の場合、不要なファイルはリストから除外されます。
        /// </summary>
        /// <param name="fileInfoWrappers">FileInfoWrapper のリストを入力します</param>
        public void ReplaceFileInfoWrappers(IEnumerable<FileInfoWrapper> fileInfoWrappers)
        {
            if (!IncludeAllFiles)
            {
                fileInfoWrappers = fileInfoWrappers.Where(f => f.IsDirectory || f.IsSoundFile());
            }

            Files.Clear();
            Files.AddRange(fileInfoWrappers);
        }

        public void Play()
        {
            if (!SelectedItem.IsSoundFile())
            {
                return;
            }

            audioPlayer.Play(SelectedItem.FileSystemInfo.FullName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            audioPlayer.Dispose();
        }
    }
}