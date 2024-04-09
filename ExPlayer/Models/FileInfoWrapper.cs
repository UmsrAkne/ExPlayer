using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Prism.Mvvm;

namespace ExPlayer.Models
{
    public class FileInfoWrapper : BindableBase
    {
        private readonly FileSystemInfo fileSystemInfo;
        private int index;

        [Key]
        [Required]
        public int Id { get; set; }

        [NotMapped]
        public FileSystemInfo FileSystemInfo
        {
            get => fileSystemInfo;
            init
            {
                Name = value.Name;
                FullName = value.FullName;

                ParentDirectoryPath = value switch
                {
                    FileInfo f => f.Directory?.FullName,
                    DirectoryInfo d => d.Parent?.FullName,
                    _ => ParentDirectoryPath,
                };

                fileSystemInfo = value;
            }
        }

        [Required]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public bool IsDirectory => FileSystemInfo is DirectoryInfo;

        [NotMapped]
        public int Index { get => index; set => SetProperty(ref index, value); }

        [NotMapped]
        public int OriginalIndex { get; set; }

        public int ListenCount { get; set; }

        public long PlaybackProgressTicks { get; set; }

        [Required]
        public string ParentDirectoryPath { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; }

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