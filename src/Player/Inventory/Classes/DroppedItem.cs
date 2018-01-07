using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Player.Inventory.Classes
{
    public class DroppedItem
    {
        public ItemID ID { get; private set; }
        public string Name { get; private set; }
        public DateTime RemoveTime { get; private set; }

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                _quantity = value;
                _itemLabel.text = $"{Name} x{value}";
            }
        }

        private int _quantity;
        private GrandTheftMultiplayer.Server.Elements.Object _itemObject;
        private TextLabel _itemLabel;

        public DroppedItem(Guid dropID, ItemID itemID, WorldModel model, Vector3 position, int quantity)
        {
            ID = itemID;
            Name = ItemDefinitions.ItemDictionary[itemID].Name;
            _quantity = quantity;
            RemoveTime = DateTime.Now.AddMinutes(Main.DroppedItemLifetime);

            _itemObject = API.shared.createObject(API.shared.getHashKey(model.ModelName), position + model.Offset, model.Rotation);
            _itemObject.setSyncedData("ItemDropID", dropID.ToString());

            _itemLabel = API.shared.createTextLabel($"{Name} x{quantity}", position + model.Offset + new Vector3(0.0, 0.0, 0.20), 10f, 0.5f, false);
        }

        public void Delete()
        {
            _itemObject.delete();
            _itemLabel.delete();
        }
    }
}