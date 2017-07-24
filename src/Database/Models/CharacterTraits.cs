using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcrpg.src.Database.Models
{
    public class CharacterTraits
    {
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }
        [ForeignKey("CharacterId")]
        public Characters Characters { get; set; }

        public int FaceFirst { get; set; }

        public int FaceSecond { get; set; }

        public float FaceMix { get; set; }

        public int SkinFirst { get; set; }

        public int SkinSecond { get; set; }

        public float SkinMix { get; set; }

        public int HairType { get; set; }

        public int HairColor { get; set; }

        public int HairHighlight { get; set; }

        public int EyeColor { get; set; }

        public int Eyebrows { get; set; }

        public int EyebrowsColor1 { get; set; }

        public int EyebrowsColor2 { get; set; }

        public int? Beard { get; set; }

        public int? BeardColor { get; set; }

        public int? Makeup { get; set; }

        public int? MakeupColor { get; set; }

        public int? Lipstick { get; set; }

        public int? LipstickColor { get; set; }

    }
}
