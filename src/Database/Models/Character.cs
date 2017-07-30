using System;
using System.Collections.Generic;

namespace pcrpg.src.Database.Models
{
    public class Character
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public bool Gender { get; set; }

        public int Money { get; set; }

        public int Bank { get; set; }

        public int Level { get; set; }

        public int Experience { get; set; }

        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public float PositionZ { get; set; }

        public float RotationZ { get; set; }

        public double PlayedTime { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual CharacterTrait Trait { get; set; }

        public virtual ICollection<CharacterClothes> Clothes { get; set; }
    }
}
