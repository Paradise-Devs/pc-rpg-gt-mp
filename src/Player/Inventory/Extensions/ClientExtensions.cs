using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Utils;

namespace pcrpg.src.Player.Inventory.Extensions
{
    public static class ClientExtensions
    {
        public static void loadInventory(this Client player)
        {
            if (Main.Inventories.ContainsKey(player.handle)) return;
            List<InventoryItem> items = null;
            string playerFile = Main.SaveLocation + Path.DirectorySeparatorChar + player.socialClubName + ".json";

            if (File.Exists(playerFile)) items = JsonConvert.DeserializeObject<List<InventoryItem>>(File.ReadAllText(playerFile));
            Main.Inventories.Add(player.handle, items ?? new List<InventoryItem>());
        }

        public static int giveItem(this Client player, ItemID ID, int amount)
        {
            if (!Main.Inventories.ContainsKey(player.handle) || !ItemDefinitions.ItemDictionary.ContainsKey(ID)) return -1;
            List<InventoryItem> playerInventory = Main.Inventories[player.handle];
            int stackSize = ItemDefinitions.ItemDictionary[ID].StackSize;
            int added = 0;

            // https://scottlilly.com/how-to-build-stackable-inventory-for-a-game-in-c/
            while (amount > 0)
            {
                if (playerInventory.Exists(i => (i.ID == ID) && (i.Quantity < stackSize)))
                {
                    InventoryItem invItem = playerInventory.First(i => (i.ID == ID) && (i.Quantity < stackSize));
                    int maxQuantity = (stackSize - invItem.Quantity);
                    int toAdd = Math.Min(amount, maxQuantity);

                    invItem.Quantity += toAdd;
                    added += toAdd;
                    amount -= toAdd;
                }
                else
                {
                    if (playerInventory.Count >= Main.MaxInventorySlots)
                    {
                        player.sendNotification("", $"Inventário cheio, não foi possível armazenar {ID} x{amount}.");
                        break;
                    }

                    playerInventory.Add(new InventoryItem(ID, 0));
                }
            }

            player.triggerEvent("CloseInventoryMenus");
            return added;
        }

        public static bool hasItem(this Client player, ItemID ID)
        {
            if (!Main.Inventories.ContainsKey(player.handle) || !ItemDefinitions.ItemDictionary.ContainsKey(ID)) return false;
            return (Main.Inventories[player.handle].Exists(i => i.ID == ID));
        }

        public static int getItem(this Client player, ItemID ID)
        {
            if (!Main.Inventories.ContainsKey(player.handle) || !ItemDefinitions.ItemDictionary.ContainsKey(ID)) return -1;
            return (Main.Inventories[player.handle].FindIndex(i => i.ID == ID));
        }

        public static int getItemAmount(this Client player, ItemID ID)
        {
            if (!Main.Inventories.ContainsKey(player.handle) || !ItemDefinitions.ItemDictionary.ContainsKey(ID)) return 0;
            return (Main.Inventories[player.handle].First(i => i.ID == ID).Quantity);
        }

        public static void useItem(this Client player, int index, int quantity = 1)
        {
            if (!Main.Inventories.ContainsKey(player.handle)) return;
            if (index < 0 || index >= Main.Inventories[player.handle].Count) return;
            InventoryItem item = Main.Inventories[player.handle][index];

            if (item.Item.Use(player))
            {
                item.Quantity -= quantity;
                if (item.Quantity < 1) Main.Inventories[player.handle].RemoveAt(index);

                sendInventory(player);
            }
        }

        public static void removeItem(this Client player, int index, int amount)
        {
            if (!Main.Inventories.ContainsKey(player.handle)) return;
            if (index < 0 || index >= Main.Inventories[player.handle].Count) return;
            InventoryItem item = Main.Inventories[player.handle][index];

            item.Quantity -= amount;
            if (item.Quantity < 1) Main.Inventories[player.handle].RemoveAt(index);

            sendInventory(player);
        }

        public static void sendInventory(this Client player)
        {
            if (!Main.Inventories.ContainsKey(player.handle)) return;
            List<InventoryItemData> playerInventory = Main.Inventories[player.handle].Select(i => new InventoryItemData(i)).ToList();
            player.triggerEvent("ReceiveInventory", API.shared.toJson(playerInventory), API.shared.toJson(new { Money = player.getMoney(), Bank = player.getBank() }));
        }

        public static void clearInventory(this Client player)
        {
            if (!Main.Inventories.ContainsKey(player.handle)) return;
            Main.Inventories[player.handle].Clear();
            player.triggerEvent("CloseInventoryMenus");
        }

        public static void saveInventory(this Client player)
        {
            if (!Main.Inventories.ContainsKey(player.handle)) return;
            string playerFile = Main.SaveLocation + Path.DirectorySeparatorChar + player.socialClubName + ".json";
            File.WriteAllText(playerFile, JsonConvert.SerializeObject(Main.Inventories[player.handle], Formatting.Indented));
        }
    }
}
