using System;
using System.Collections.ObjectModel;
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

        public ObservableCollection<FileInfoWrapper> Files { get; init; } = new ();

        public FileInfoWrapper SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public DelegateCommand StopCommand => new (() =>
        {
            audioPlayer.Stop();
        });

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