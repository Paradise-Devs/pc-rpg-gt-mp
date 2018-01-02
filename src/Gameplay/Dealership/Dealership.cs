using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pcrpg.src.Gameplay.Dealership
{
    #region Dealership Class
    public class Dealership
    {
        public Guid ID { get; }
        public string Name { get; private set; }
        public Vector3 Position { get; }
        public Vector3 VehicleSpawn { get; private set; }

        // storage
        public long Money { get; private set; }
        public List<DealershipVehicle> Vehicles { get; }

        public DateTime CreatedAt { get; }

        // entities
        [JsonIgnore]
        private Blip Blip;

        [JsonIgnore]
        private Marker Marker;

        [JsonIgnore]
        private ColShape ColShape;

        [JsonIgnore]
        private TextLabel Label;

        [JsonIgnore]
        private DateTime LastSave;

        public Dealership(Guid id, Vector3 position, string name = "", Vector3 vehiclespawn = null, long money = 0, List<DealershipVehicle> vehicles = null)
        {
            ID = id;
            Position = position;
            VehicleSpawn = vehiclespawn ?? new Vector3(0f, 0f, 0f);
            CreatedAt = DateTime.Now;

            Name = name;

            Money = money;
            Vehicles = (vehicles == null) ? new List<DealershipVehicle>() : vehicles;

            // create blip
            Blip = API.shared.createBlip(position);
            Blip.name = "Concessionária";
            Blip.scale = 1f;
            Blip.shortRange = true;
            UpdateBlip();

            // create colshape
            ColShape = API.shared.createCylinderColShape(position, 0.85f, 0.85f);
            ColShape.onEntityEnterColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.setData("DealershipMarker_ID", ID);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.resetData("DealershipMarker_ID");
                }
            };

            // create marker
            Marker = API.shared.createMarker(1, position - new Vector3(0.0, 0.0, 1.0), new Vector3(), new Vector3(), new Vector3(1.0, 1.0, 0.5), 150, 99, 76, 149);

            // create text label
            Label = API.shared.createTextLabel("Concessionária", position, 15f, 0.65f);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            Label.text = string.Format("~p~Concessionária~n~~w~{0}", Name);
        }

        private void UpdateBlip()
        {
            Blip.sprite = 225;
            Blip.color = 50;
        }

        public void SetSpawn(Vector3 spawn)
        {
            VehicleSpawn = spawn;

            Save();
        }

        public void SetName(string new_name)
        {
            Name = new_name;

            UpdateLabel();
            Save();
        }

        public void GiveMoney(int amount)
        {
            Money += amount;

            Save();
        }

        public void SetMoney(int amount)
        {
            Money = amount;

            Save();
        }

        public void Save(bool force = false)
        {
            if (!force && DateTime.Now.Subtract(LastSave).TotalSeconds < Main.SAVE_INTERVAL) return;

            File.WriteAllText(Main.DEALERSHIP_SAVE_DIR + Path.DirectorySeparatorChar + ID + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
            LastSave = DateTime.Now;
        }

        public void Destroy(bool exit = false)
        {
            Blip.delete();
            Marker.delete();
            API.shared.deleteColShape(ColShape);
            Label.delete();
        }
    }
    #endregion

    public class Main : Script
    {
        // settings
        public static string DEALERSHIP_SAVE_DIR = "data/Dealership";
        public static int PLAYER_DEALERSHIP_LIMIT = 0;
        public static int DEALERSHIP_MONEY_LIMIT = 5000000;
        public static int SAVE_INTERVAL = 120;

        public static List<Dealership> Dealerships = new List<Dealership>();

        public Main()
        {
            API.onResourceStart += Dealership_Init;
            API.onResourceStop += Dealership_Exit;
            API.onClientEventTrigger += Dealership_ClientEvent;
            API.onPlayerEnterVehicle += Dealership_PlayerEnterVehicle;
            API.onPlayerExitVehicle += Dealership_PlayerExitVehicle;
        }

        #region Methods
        public static Guid GetGuid()
        {
            Guid new_guid;

            do
            {
                new_guid = Guid.NewGuid();
            } while (Dealerships.Count(h => h.ID == new_guid) > 0);

            return new_guid;
        }
        #endregion

        #region Events
        public void Dealership_Init()
        {
            // load settings
            if (API.hasSetting("dealershipDirName")) DEALERSHIP_SAVE_DIR = API.getSetting<string>("dealershipDirName");

            DEALERSHIP_SAVE_DIR = API.getResourceFolder() + Path.DirectorySeparatorChar + DEALERSHIP_SAVE_DIR;
            if (!Directory.Exists(DEALERSHIP_SAVE_DIR)) Directory.CreateDirectory(DEALERSHIP_SAVE_DIR);

            if (API.hasSetting("playerDealershipLimit")) PLAYER_DEALERSHIP_LIMIT = API.getSetting<int>("playerDealershipLimit");
            if (API.hasSetting("dealershipMoneyLimit")) DEALERSHIP_MONEY_LIMIT = API.getSetting<int>("dealershipMoneyLimit");
            if (API.hasSetting("saveInterval")) SAVE_INTERVAL = API.getSetting<int>("saveInterval");

            API.consoleOutput("-> Player Dealership Limit: {0}", ((PLAYER_DEALERSHIP_LIMIT == 0) ? "Disabled" : PLAYER_DEALERSHIP_LIMIT.ToString()));
            API.consoleOutput("-> Dealership Safe Limit: ${0:n0}", DEALERSHIP_MONEY_LIMIT);
            API.consoleOutput("-> Save Interval: {0}", TimeSpan.FromSeconds(SAVE_INTERVAL).ToString(@"hh\:mm\:ss"));

            // load dealerships
            foreach (string file in Directory.EnumerateFiles(DEALERSHIP_SAVE_DIR, "*.json"))
            {
                Dealership dealership = JsonConvert.DeserializeObject<Dealership>(File.ReadAllText(file));
                Dealerships.Add(dealership);

                // vehicles
                foreach (DealershipVehicle vehicle in dealership.Vehicles) vehicle.Create();
            }

            API.consoleOutput("Loaded {0} dealerships.", Dealerships.Count);
        }

        public void Dealership_ClientEvent(Client player, string event_name, params object[] args)
        {
            switch (event_name)
            {
                case "RequestDealershipBuyMenu":
                    {
                        if (!player.hasData("DealershipVehicle_ID")) return;

                        DealershipVehicle vehicle = player.getData("DealershipVehicle_ID");
                        player.triggerEvent("Dealership_PurchaseMenu", API.toJson(new { vehicle.Price }));
                        break;
                    }
                case "VehiclePurchase":
                    {
                        if (!player.hasData("DealershipVehicle_ID")) return;

                        int color1 = (int)args[0];
                        int color2 = (int)args[1];

                        player.sendNotification("Dealership", "TO DO: buy car");
                        break;
                    }
            }
        }

        public void Dealership_Exit()
        {
            foreach (Dealership dealership in Dealerships)
            {
                dealership.Save(true);
                dealership.Destroy(true);
            }

            Dealerships.Clear();
        }

        public void Dealership_PlayerEnterVehicle(Client player, NetHandle vehicle, int seat)
        {
            foreach (Dealership dealership in Dealerships)
            {
                foreach (DealershipVehicle v in dealership.Vehicles)
                {
                    if (vehicle == v.Vehicle)
                    {
                        API.sendChatMessageToPlayer(player, "~y~INFO: ~s~Para comprar este veículo, aperte ~y~E~w~.");
                        player.setData("Dealership_ID", dealership);
                        player.setData("DealershipVehicle_ID", v);
                        player.triggerEvent("OnEnterDealershipVehicle");
                    }
                }
            }
        }

        public void Dealership_PlayerExitVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (player.hasData("DealershipVehicle_ID"))
            {
                player.resetData("Dealership_ID");
                player.resetData("DealershipVehicle_ID");
                player.triggerEvent("OnExitDealershipVehicle");
            }
        }

        public static bool IsADealershipVehicle(NetHandle vehicle)
        {
            foreach (Dealership dealership in Dealerships)
            {
                foreach (DealershipVehicle v in dealership.Vehicles)
                {
                    if (vehicle == v.Vehicle)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
