using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Extensions;
using pcrpg.src.Player.Inventory.Interfaces;

namespace pcrpg.src.Player.Inventory
{
    public class Main : Script
    {
        public static Dictionary<NetHandle, List<InventoryItem>> Inventories = new Dictionary<NetHandle, List<InventoryItem>>();
        public Dictionary<Guid, DroppedItem> DroppedItems = new Dictionary<Guid, DroppedItem>();

        // these two are modified from meta.xml
        public static int MaxInventorySlots = 32;
        public static int DroppedItemLifetime = 5;

        public static string SaveLocation = "data/Inventories";
        public Timer DropRemover = null;

        public Main()
        {
            API.onResourceStart += Inventory_Init;
            API.onPlayerFinishedDownload += Inventory_PlayerJoin;
            API.onClientEventTrigger += Inventory_EventTrigger;
            API.onPlayerDisconnected += Inventory_PlayerLeave;
            API.onResourceStop += Inventory_Exit;
        }

        #region Events
        public void Inventory_Init()
        {
            SaveLocation = API.getResourceFolder() + Path.DirectorySeparatorChar + "data/Inventories";
            if (!Directory.Exists(SaveLocation)) Directory.CreateDirectory(SaveLocation);

            if (API.hasSetting("inventorySlots")) MaxInventorySlots = API.getSetting<int>("inventorySlots");
            if (API.hasSetting("dropLifetime")) DroppedItemLifetime = API.getSetting<int>("dropLifetime");

            API.consoleOutput($"Inventory Slots: {MaxInventorySlots}");
            API.consoleOutput($"Dropped Item Lifetime: {TimeSpan.FromMinutes(DroppedItemLifetime).ToString(@"hh\:mm\:ss")}");

            DropRemover = API.startTimer(60000, false, () =>
            {
                int count = 0;

                foreach (KeyValuePair<Guid, DroppedItem> item in DroppedItems.Where(kv => kv.Value.RemoveTime <= DateTime.Now).ToList())
                {
                    item.Value.Delete();
                    DroppedItems.Remove(item.Key);
                    count++;
                }

                if (count > 0)
                    API.consoleOutput($"Removed {count} items from the world.");
            });
        }

        public void Inventory_PlayerJoin(Client player)
        {
            player.loadInventory();
        }

        public void Inventory_EventTrigger(Client player, string eventName, params object[] args)
        {
            switch (eventName)
            {
                case "RequestInventory":
                    {
                        if (!Inventories.ContainsKey(player.handle)) return;
                        player.sendInventory();
                        break;
                    }

                case "ConsumeItem":
                    {
                        if (args.Length < 1 || !Inventories.ContainsKey(player.handle)) return;
                        int idx = Convert.ToInt32(args[0]);
                        player.useItem(idx);
                        break;
                    }

                case "DropItem":
                    {
                        if (args.Length < 3 || !Inventories.ContainsKey(player.handle)) return;
                        int idx = Convert.ToInt32(args[0]);
                        int amount = Convert.ToInt32(args[1]);
                        Vector3 pos = (Vector3)args[2];

                        if (idx < 0 || idx >= Inventories[player.handle].Count) return;
                        if (player.position.DistanceTo(pos) >= 3.0) return;

                        if (Inventories[player.handle][idx].Item is IDroppable dropInfo)
                        {
                            Guid dropID;

                            do
                            {
                                dropID = Guid.NewGuid();
                            } while (DroppedItems.ContainsKey(dropID));

                            DroppedItems.Add(dropID, new DroppedItem(dropID, Inventories[player.handle][idx].ID, dropInfo.DropModel, pos, amount));
                            player.removeItem(idx, amount);
                        }

                        break;
                    }

                case "TakeItem":
                    {
                        if (args.Length < 1 || !Inventories.ContainsKey(player.handle)) return;
                        Guid dropID = new Guid(args[0].ToString());
                        if (!DroppedItems.ContainsKey(dropID)) return;

                        DroppedItem item = DroppedItems[dropID];
                        int added = player.giveItem(item.ID, item.Quantity);

                        if (added > 0)
                        {
                            item.Quantity -= added;

                            if (item.Quantity < 1)
                            {
                                item.Delete();
                                DroppedItems.Remove(dropID);
                            }

                            player.sendNotification("", $"+ {item.Name} x{added}.");
                        }

                        break;
                    }
            }
        }

        public void Inventory_PlayerLeave(Client player, string reason)
        {
            player.saveInventory();
            if (Inventories.ContainsKey(player.handle)) Inventories.Remove(player.handle);
        }

        public void Inventory_Exit()
        {
            foreach (Client player in API.getAllPlayers()) player.saveInventory();
            foreach (KeyValuePair<Guid, DroppedItem> item in DroppedItems) item.Value.Delete();
            Inventories.Clear();
            DroppedItems.Clear();
        }
        #endregion
    }
}
