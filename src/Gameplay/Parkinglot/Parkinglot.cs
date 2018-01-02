using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace pcrpg.src.Gameplay.Parkinglot
{
    public class ParkingSpace
    {
        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }
    }

    #region Parkinglot Class
    public class Parkinglot
    {
        public Guid ID { get; }
        public Vector3 Position { get; }
        public List<ParkingSpace> Spawns { get; private set; }

        // storage
        public long Money { get; private set; }

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

        public Parkinglot(Guid id, Vector3 position, long money = 0, List<ParkingSpace> spawns = null)
        {
            ID = id;
            Position = position;
            Spawns = spawns ?? new List<ParkingSpace>();
            Money = money;

            // create blip
            Blip = API.shared.createBlip(position);
            Blip.name = "Estacionamento";
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
                    player.setData("ParkinglotMarker_ID", ID);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.resetData("ParkinglotMarker_ID");
                }
            };

            // create marker
            Marker = API.shared.createMarker(1, position - new Vector3(0.0, 0.0, 1.0), new Vector3(), new Vector3(), new Vector3(1.0, 1.0, 0.5), 150, 255, 165, 0);

            // create text label
            Label = API.shared.createTextLabel("~o~Estacionamento~s~\nPresione ~o~F3 ~s~para usar\n~g~$~s~100", position, 15f, 0.65f);
        }

        private void UpdateBlip()
        {
            Blip.sprite = 50;
            Blip.color = 47;
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

            File.WriteAllText(Main.PARKINGLOT_SAVE_DIR + Path.DirectorySeparatorChar + ID + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
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
        public static string PARKINGLOT_SAVE_DIR = "data/Parkinglot";
        public static int PLAYER_PARKINGLOT_LIMIT = 0;
        public static int PARKINGLOT_MONEY_LIMIT = 5000000;
        public static int SAVE_INTERVAL = 120;

        public static List<Parkinglot> Parkinglots = new List<Parkinglot>();

        public Main()
        {
            API.onResourceStart += Parkinglot_Init;
            API.onResourceStop += Parkinglot_Exit;
        }

        #region Methods
        public static Guid GetGuid()
        {
            Guid new_guid;

            do
            {
                new_guid = Guid.NewGuid();
            } while (Parkinglots.Count(h => h.ID == new_guid) > 0);

            return new_guid;
        }
        #endregion

        #region Events
        public void Parkinglot_Init()
        {
            // load settings
            if (API.hasSetting("parkinglotDirName")) PARKINGLOT_SAVE_DIR = API.getSetting<string>("parkinglotDirName");

            PARKINGLOT_SAVE_DIR = API.getResourceFolder() + Path.DirectorySeparatorChar + PARKINGLOT_SAVE_DIR;
            if (!Directory.Exists(PARKINGLOT_SAVE_DIR)) Directory.CreateDirectory(PARKINGLOT_SAVE_DIR);

            if (API.hasSetting("playerParkinglotLimit")) PLAYER_PARKINGLOT_LIMIT = API.getSetting<int>("playerParkinglotLimit");
            if (API.hasSetting("parkinglotMoneyLimit")) PARKINGLOT_MONEY_LIMIT = API.getSetting<int>("parkinglotMoneyLimit");
            if (API.hasSetting("saveInterval")) SAVE_INTERVAL = API.getSetting<int>("saveInterval");

            API.consoleOutput("-> Player Parking lot Limit: {0}", ((PLAYER_PARKINGLOT_LIMIT == 0) ? "Disabled" : PLAYER_PARKINGLOT_LIMIT.ToString()));
            API.consoleOutput("-> Parking lot Safe Limit: ${0:n0}", PARKINGLOT_MONEY_LIMIT);
            API.consoleOutput("-> Save Interval: {0}", TimeSpan.FromSeconds(SAVE_INTERVAL).ToString(@"hh\:mm\:ss"));

            // load parking lots
            foreach (string file in Directory.EnumerateFiles(PARKINGLOT_SAVE_DIR, "*.json"))
            {
                Parkinglot parkinglot = JsonConvert.DeserializeObject<Parkinglot>(File.ReadAllText(file));
                Parkinglots.Add(parkinglot);
            }

            API.consoleOutput("Loaded {0} parking lots.", Parkinglots.Count);
        }

        public void Parkinglot_Exit()
        {
            foreach (Parkinglot parkinglot in Parkinglots)
            {
                parkinglot.Save(true);
                parkinglot.Destroy(true);
            }

            Parkinglots.Clear();
        }
        #endregion
    }
}
