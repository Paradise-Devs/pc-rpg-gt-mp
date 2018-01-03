using System;
using System.IO;
using Newtonsoft.Json;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Job
{
    public enum JobType
    {
        SecurityGuard
    }    

    public class Job
    {
        public Guid ID { get; }
        public string Name { get; private set; }
        public JobType Type { get; private set; }
        public Vector3 Position { get; }

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

        public Job(Guid id, Vector3 position, JobType type)
        {
            ID = id;
            Position = position;
            Name = Main.GetJobName(type);
            Type = type;

            // create blip
            Blip = API.shared.createBlip(position);
            Blip.sprite = 351;
            Blip.name = "Emprego";
            Blip.color = 15;
            Blip.scale = 1f;
            Blip.shortRange = true;

            // create marker
            Marker = API.shared.createMarker(1, position - new Vector3(0.0, 0.0, 1.0), new Vector3(), new Vector3(), new Vector3(1.0, 1.0, 0.5), 150, 64, 196, 255);

            // create colshape
            ColShape = API.shared.createCylinderColShape(position, 0.85f, 0.85f);
            ColShape.onEntityEnterColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.setData("JobMarker_ID", ID);
                    player.triggerEvent("ShowJobText", 1);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.resetData("JobMarker_ID");
                    player.triggerEvent("ShowJobText", 0);
                }
            };

            // create text label
            Label = API.shared.createTextLabel("Emprego", position, 15f, 0.65f);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            Label.text = string.Format("~g~Emprego~n~~w~{0}", Name);
        }

        private void UpdateBlip()
        {
            if (string.IsNullOrEmpty(Name))
                Blip.name = $"Emprego de {Name}";
            else
                Blip.name = "Emprego";
        }

        public void SetType(JobType new_type)
        {
            Type = new_type;
            Name = Main.GetJobName(new_type);

            UpdateBlip();
            UpdateLabel();
            Save();
        }

        public void Save(bool force = false)
        {
            if (!force && DateTime.Now.Subtract(LastSave).TotalSeconds < Main.SAVE_INTERVAL) return;

            File.WriteAllText(Main.JOB_SAVE_DIR + Path.DirectorySeparatorChar + ID + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
            LastSave = DateTime.Now;
        }

        public void Destroy()
        {
            Blip.delete();
            Marker.delete();
            API.shared.deleteColShape(ColShape);
            Label.delete();
        }
    }
}
