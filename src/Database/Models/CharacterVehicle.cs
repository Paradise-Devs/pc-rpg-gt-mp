using System;

namespace pcrpg.src.Database.Models
{
    public class CharacterVehicle
    {
        public int Id { get; set; }

        public Character Character { get; set; }

        public int Price { get; set; }

        public int Model { get; set; }

        public int Color1 { get; set; }

        public int Color2 { get; set; }

        public int Fuel { get; set; }

        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public float PositionZ { get; set; }

        public float RotationX { get; set; }

        public float RotationY { get; set; }

        public float RotationZ { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}