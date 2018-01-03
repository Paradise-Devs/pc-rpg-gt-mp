using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace pcrpg.src.Gameplay.Bank
{
    #region Bank Class
    public class Bank
    {
        public Guid ID { get; }
        public Vector3 Position { get; }
        public int Type { get; private set; }

        // entities
        [JsonIgnore]
        private Blip Blip;

        [JsonIgnore]
        private Marker Marker;

        [JsonIgnore]
        private ColShape ColShape;

        [JsonIgnore]
        private DateTime LastSave;

        public Bank(Guid id, Vector3 position, int type)
        {
            ID = id;
            Position = position;
            Type = type;

            // create blip
            if (Type == 0)
            {
                Blip = API.shared.createBlip(position);
                Blip.name = "Banco";
                Blip.scale = 1f;
                Blip.shortRange = true;
                UpdateBlip();
            }

            // create colshape
            ColShape = API.shared.createCylinderColShape(position, 0.85f, 0.85f);
            ColShape.onEntityEnterColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.setData("BankMarker_ID", ID);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.resetData("BankMarker_ID");
                }
            };

            // create marker
            Marker = API.shared.createMarker(1, position - new Vector3(0.0, 0.0, 1.0), new Vector3(), new Vector3(), new Vector3(0.5, 0.5, 0.25), 150, 25, 255, 25);
        }

        private void UpdateBlip()
        {
            if (!Blip.exists) return;

            Blip.sprite = 108;
            Blip.color = 69;
        }

        public void SetType(int new_type)
        {
            Type = new_type;

            if (new_type == 1 && API.shared.doesEntityExist(Blip)) Blip.delete();
            else if (new_type == 0 && !API.shared.doesEntityExist(Blip))
            {
                Blip = API.shared.createBlip(Position);
                Blip.name = "Banco";
                Blip.scale = 1f;
                Blip.shortRange = true;
                UpdateBlip();
            }

            Save();
        }

        public void Save(bool force = false)
        {
            if (!force && DateTime.Now.Subtract(LastSave).TotalSeconds < Main.SAVE_INTERVAL) return;

            File.WriteAllText(Main.BANK_SAVE_DIR + Path.DirectorySeparatorChar + ID + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
            LastSave = DateTime.Now;
        }

        public void Destroy()
        {
            if (Blip.exists) Blip.delete();
            Marker.delete();
            API.shared.deleteColShape(ColShape);
        }
    }
    #endregion

    public class Main : Script
    {
        // settings
        public static string BANK_SAVE_DIR = "data/Bank";
        public static int SAVE_INTERVAL = 120;

        public static List<Bank> Banks = new List<Bank>();

        public Main()
        {
            API.onResourceStart += Bank_Init;
            API.onResourceStop += Bank_Exit;
        }

        #region Methods
        public static Guid GetGuid()
        {
            Guid new_guid;

            do
            {
                new_guid = Guid.NewGuid();
            } while (Banks.Count(h => h.ID == new_guid) > 0);

            return new_guid;
        }
        #endregion

        #region Events
        public void Bank_Init()
        {
            // load settings
            if (API.hasSetting("bankDirName")) BANK_SAVE_DIR = API.getSetting<string>("bankDirName");

            BANK_SAVE_DIR = API.getResourceFolder() + Path.DirectorySeparatorChar + BANK_SAVE_DIR;
            if (!Directory.Exists(BANK_SAVE_DIR)) Directory.CreateDirectory(BANK_SAVE_DIR);

            if (API.hasSetting("saveInterval")) SAVE_INTERVAL = API.getSetting<int>("saveInterval");
            API.consoleOutput("-> Save Interval: {0}", TimeSpan.FromSeconds(SAVE_INTERVAL).ToString(@"hh\:mm\:ss"));

            // load parking lots
            foreach (string file in Directory.EnumerateFiles(BANK_SAVE_DIR, "*.json"))
            {
                Bank bank = JsonConvert.DeserializeObject<Bank>(File.ReadAllText(file));
                Banks.Add(bank);
            }

            API.consoleOutput("Loaded {0} banks/atms.", Banks.Count);
        }

        public void Bank_Exit()
        {
            foreach (Bank bank in Banks)
            {
                bank.Save(true);
                bank.Destroy();
            }

            Banks.Clear();
        }
        #endregion
    }
}
