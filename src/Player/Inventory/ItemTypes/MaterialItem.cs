using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Interfaces;

namespace pcrpg.src.Player.Inventory.ItemTypes
{
    public class MaterialItem : BaseItem, IDroppable
    {
        public WorldModel DropModel { get; set; }

        public MaterialItem(string name, string description, int stackSize, string rarity, WorldModel dropModel) : base(name, description, stackSize, rarity)
        {
            DropModel = dropModel;

            Category = "Materiais";
            Icon = "clothes-hoodie";
            Usable = false;
            Tradable = true;
            NeedConfirmation = false;
        }

        public override bool Use(Client player)
        {
            return false;
        }
    }
}
