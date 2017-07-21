using System.ComponentModel.DataAnnotations;

namespace pcrpg.Database.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SocialClubName { get; set; }

        public string Email { get; set; }

        public string LastIp { get; set; }
    }
}
