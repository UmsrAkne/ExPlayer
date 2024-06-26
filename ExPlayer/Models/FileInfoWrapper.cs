using System;
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
        private bool playing;
        private bool ignore;

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

                IsM3U = value.Extension.StartsWith(".m3u", StringComparison.OrdinalIgnoreCase);
                fileSystemInfo = value;
            }
        }

        [Required]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public bool IsDirectory => FileSystemInfo is DirectoryInfo || IsM3U;

        [NotMapped]
        public bool IsM3U { get; private init; }

        [NotMapped]
        public int Index { get => index; set => SetProperty(ref index, value); }

        [NotMapped]
        public int OriginalIndex { get; set; }

        [NotMapped]
        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        public int ListenCount { get; set; }

        public long PlaybackProgressTicks { get; set; }

        [Required]
        public string ParentDirectoryPath { get; set; } = string.Empty;

        [Required]
        public bool Ignore { get => ignore; set => SetProperty(ref ignore, value); }

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