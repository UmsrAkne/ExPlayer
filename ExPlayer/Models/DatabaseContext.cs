using System.Data.SQLite;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExPlayer.Models
{
    public class DatabaseContext : DbContext
    {
        private const string DbFileName = "db.sqlite";

        public DbSet<FileInfoWrapper> ListenHistory { get; set; }

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