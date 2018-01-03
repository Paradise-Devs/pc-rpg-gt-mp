using pcrpg.src.Player.Inventory.Classes;

namespace pcrpg.src.Player.Inventory.Interfaces
{
    interface IDroppable
    {
        WorldModel DropModel { get; set; }
    }
}
