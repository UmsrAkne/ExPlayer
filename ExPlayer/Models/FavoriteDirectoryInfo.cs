using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace ExPlayer.Models
{
    public class FavoriteDirectoryInfo
    {
        private DirectoryInfo directoryInfo;

        [Key]
        [Required]
        public int Id { get; set; }

        [NotMapped]
        public DirectoryInfo DirectoryInfo
        {
            get => directoryInfo;
            set
            {
                if (value == null)
                {
                    return;
                }

                FullName = value.FullName;
                directoryInfo = value;
            }
        }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty;
    }
}