using System;

namespace pcrpg.src.Database.Models
{
    public class Dialog
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }

        public int NpcId { get; set; }

        public int CurrentConversation { get; set; }

        public int? LastAnswer { get; set; }

        public DateTime CreatedAt { get; set; }

        public Dialog()
        {
            CurrentConversation = 0;
            CreatedAt = DateTime.Now;
        }
    }
}
