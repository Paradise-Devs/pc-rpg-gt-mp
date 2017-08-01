using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcrpg.src.Database.Models
{
    public abstract class Item
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Description { get; set; }

        [StringLength(24)]
        public string Rarity { get; set; }

        [StringLength(24)]
        public string Category { get; set; }

        [StringLength(24)]
        public string Icon { get; set; }

        public bool Usable { get; set; }

        public bool Tradable { get; set; }

        public bool NeedConfirmation { get; set; }

        public int Quantity { get; set; }

        public int MaximumStackableQuantity { get; set; }

        public DateTime CreatedAt { get; set; }

        protected Item()
        {
            Usable = false;
            Tradable = true;
            Quantity = 1;
            CreatedAt = DateTime.Now;
            NeedConfirmation = false;
            MaximumStackableQuantity = 1;
        }
    }

    [Table("Food")]
    public class Food : Item
    {
        public int HungerRegeneration { get; set; }

        public int ThirstRegeneration { get; set; }

        public Food()
        {
            Usable = true;
            Category = "food";
            MaximumStackableQuantity = 10;
        }
    }
}
