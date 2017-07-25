using pcrpg.src.Database.Models;
using System.Collections.Generic;

namespace pcrpg.Database.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SocialClubName { get; set; }

        public string Email { get; set; }

        public string LastIp { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
