using System.IO;
using Prism.Mvvm;

namespace ExPlayer.Models
{
    public class FileInfoWrapper : BindableBase
    {
        public FileSystemInfo FileSystemInfo { get; init; }

        public string Name => FileSystemInfo != null ? FileSystemInfo.Name : string.Empty;
    }
}