using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using pcrpg.Database.Models;

namespace pcrpg.src.Database.Models
{
    public class Characters
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users Users { get; set; }

        public int Money { get; set; }

        public int Bank { get; set; }

        public int Level { get; set; }

        public int Experience { get; set; }

        public double PlayedTime { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
