namespace pcrpg.src.Database.Models
{
    public class CharacterClothes
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }

        public int BodyPart { get; set; }

        public int Variation { get; set; }

        public int Texture { get; set; }

        public bool IsAccessory { get; set; }
    }
}
