using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Interfaces;

namespace pcrpg.src.Player.Inventory.ItemTypes
{
    public class WeaponItem : BaseItem, IDroppable
    {
        public WeaponHash Hash { get; set; }
        public WorldModel DropModel { get; set; }

        public WeaponItem(string name, string description, int stackSize, WeaponHash hash, string rarity, string icon, WorldModel dropModel) : base(name, description, stackSize, rarity)
        {
            Hash = hash;
            DropModel = dropModel;

            Category = "Armas";
            Icon = icon;
            Usable = true;
            Tradable = true;
            NeedConfirmation = false;
        }

        public override bool Use(Client player)
        {
            player.sendChatMessage($"Received weapon. ({Hash})");
            player.giveWeapon(Hash, 9999, true, true);
            return true;
        }
    }
}