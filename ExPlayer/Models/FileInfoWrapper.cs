using System.IO;
using Prism.Mvvm;

namespace ExPlayer.Models
{
    public class FileInfoWrapper : BindableBase
    {
        private int index;

        public FileSystemInfo FileSystemInfo { get; init; }

        public string Name => FileSystemInfo != null ? FileSystemInfo.Name : string.Empty;

        public bool IsDirectory => FileSystemInfo is DirectoryInfo;

        public int Index { get => index; set => SetProperty(ref index, value); }

        public int OriginalIndex { get; set; }

        public bool IsSoundFile()
        {
            if (IsDirectory)
            {
                return false;
            }

            var f = FileSystemInfo as FileInfo;
            return f is { Extension: ".mp3" or ".ogg" or ".wav", };
        }
    }
}