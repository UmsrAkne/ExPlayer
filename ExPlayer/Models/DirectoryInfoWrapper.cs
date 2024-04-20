using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace ExPlayer.Models
{
    public class DirectoryInfoWrapper
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [NotMapped]
        public DirectoryInfo DirectoryInfo { get; set; }
        
        [Required]
        public string FullName { get; set; }

        [Required]
        public DateTime OpenDateTime { get; set; }
    }
}