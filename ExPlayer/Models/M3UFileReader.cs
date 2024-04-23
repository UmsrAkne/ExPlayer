using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExPlayer.Models
{
    public class M3UFileReader
    {
        public static List<FileInfoWrapper> GetFilesFrom(string m3UFilePath)
        {
            if (!File.Exists(m3UFilePath))
            {
                return new List<FileInfoWrapper>();
            }

            return GetFileInfoWrappers(File.ReadAllLines(m3UFilePath), new FileInfo(m3UFilePath).Directory?.FullName);
        }

        public static List<FileInfoWrapper> GetFileInfoWrappers(string[] paths, string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return new List<FileInfoWrapper>();
            }

            return paths
            .Where(str => !str.StartsWith("#") && !str.TrimStart().StartsWith("#"))
            .Select(path => new FileInfoWrapper()
            {
                FileSystemInfo = new FileInfo(Path.Combine(basePath, path)),
            })
            .ToList();
        }
    }
}