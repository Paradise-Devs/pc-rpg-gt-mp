using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Database.Models;
using System.Linq;
using System.Collections.Generic;

namespace pcrpg.src.Player.Inventory
{
    public class InventoryModel : Script
    {
        public InventoryModel()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "GetCharacterItems")
            {
                if (!API.hasEntityData(sender, "Character")) return;

                Character character = API.getEntityData(sender, "Character");

                List<int> cash = new List<int>
                {
                    character.Money,
                    character.Bank
                };
                API.triggerClientEvent(sender, "UpdateCharacterItems", API.toJson(character.Items), API.toJson(cash));
            }
            else if (eventName == "UseItem")
            {
                // Called when a player uses an item
            }
            else if (eventName == "DiscardItem")
            {
                if (!API.hasEntityData(sender, "Character")) return;

                int itemId = (int)arguments[0];
                using (var ctx = new ContextFactory().Create())
                {
                    var item = ctx.Items.FirstOrDefault(up => up.Id == itemId);
                    if (item != null)
                    {
                        ctx.Items.Remove(item);
                        API.triggerClientEvent(sender, "OnItemDiscarded", true);
                        ctx.SaveChanges();
                    }
                }
            }
        }
    }
}
