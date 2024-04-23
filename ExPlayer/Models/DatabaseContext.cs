using System.Data.SQLite;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExPlayer.Models
{
    public class DatabaseContext : DbContext
    {
        private const string DbFileName = "db.sqlite";

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<FileInfoWrapper> ListenHistory { get; set; }

        public DbSet<DirectoryInfoWrapper> OpenedDirectoryHistory { get; set; }

        public DbSet<FavoriteDirectoryInfo> FavoriteDirectories { get; set; }

        /// <summary>
        /// FileInfoWrapper の ListenCount を 1 増やして DB に記録します。
        /// DB に該当ファイルが記録されていなかった場合は、ファイル自体も記録します。
        /// </summary>
        /// <param name="wrapper">ListenCount を増やしたい FileInfoWrapper</param>
        public void AddListenCount(FileInfoWrapper wrapper)
        {
            var l = ListenHistory.FirstOrDefault(ll => ll.FullName == wrapper.FullName);

            if (l != null)
            {
                l.ListenCount++;
            }
            else
            {
                wrapper.ListenCount++;
                ListenHistory.Add(wrapper);
            }

            SaveChanges();
        }

        public void AddDirectoryHistory(DirectoryInfoWrapper directoryInfoWrapper)
        {
            if (string.IsNullOrWhiteSpace(directoryInfoWrapper.FullName))
            {
                return;
            }

            OpenedDirectoryHistory.Add(directoryInfoWrapper);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!File.Exists(DbFileName))
            {
                SQLiteConnection.CreateFile(DbFileName);
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = DbFileName, }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }
    }
}