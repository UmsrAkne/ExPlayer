using System.IO;
using Prism.Mvvm;

namespace ExPlayer.Models
{
    public class FileInfoWrapper : BindableBase
    {
        public FileSystemInfo FileSystemInfo { get; init; }

        public string Name => FileSystemInfo != null ? FileSystemInfo.Name : string.Empty;

        public bool IsDirectory => FileSystemInfo is DirectoryInfo;

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