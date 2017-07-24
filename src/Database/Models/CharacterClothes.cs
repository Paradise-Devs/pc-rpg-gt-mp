using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcrpg.src.Database.Models
{
    public class CharacterClothes
    {
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }
        [ForeignKey("CharacterId")]
        public Characters Characters { get; set; }

        public bool Using { get; set; }

        public int BodyPart { get; set; }

        public int Variation { get; set; }

        public int? Torso { get; set; }

        public bool? IsAccessory { get; set; }

    }
}
