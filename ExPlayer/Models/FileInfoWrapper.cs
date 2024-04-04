using System.IO;
using Prism.Mvvm;

namespace ExPlayer.Models
{
    public class FileInfoWrapper : BindableBase
    {
        public FileInfo FileInfo { get; init; }

        public string Name => FileInfo != null ? FileInfo.Name : string.Empty;
    }
}