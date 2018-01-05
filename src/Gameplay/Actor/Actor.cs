using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.IO;
using GrandTheftMultiplayer.Server.Constant;
using System.Collections.Generic;

namespace pcrpg.src.Gameplay.Actor
{
    #region Actor Class
    public class Actor
    {
        public Guid ID { get; }
        public string Name { get; }
        public PedHash Skin { get; }
        public Vector3 Position { get; }
        public float Heading { get; }
        public List<Dialogue> Dialogues { get; }

        // entities
        [JsonIgnore]
        private Ped Ped;

        [JsonIgnore]
        private Blip Blip;

        [JsonIgnore]
        private TextLabel Label;

        [JsonIgnore]
        private ColShape ColShape;

        [JsonIgnore]
        private DateTime LastSave;

        public Actor(Guid id, string name, PedHash skin, Vector3 position, float heading, List<Dialogue> dialogues = null)
        {
            ID = id;
            Name = name;
            Skin = skin;
            Position = position;
            Heading = heading;
            Dialogues = dialogues ?? new List<Dialogue>();

            // create blip
            Blip = API.shared.createBlip(position);
            Blip.shortRange = true;
            Blip.sprite = 1;
            Blip.color = 43;
            Blip.name = "NPC";
            Blip.scale = 0.5f;

            // create ped
            Ped = API.shared.createPed(Skin, Position, heading);

            // create label
            Label = API.shared.createTextLabel(Name, Position + new Vector3(0.0, 0.0, 1.0), 15f, 0.65f);

            // create colshape
            ColShape = API.shared.createCylinderColShape(position, 0.85f, 0.85f);
            ColShape.onEntityEnterColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.setData("ActorMarker_ID", ID);
                    player.triggerEvent("ShowActorText", 1);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.resetData("ActorMarker_ID");
                    player.triggerEvent("ShowActorText", 0);
                }
            };
        }

        public void Save(bool force = false)
        {
            if (!force && DateTime.Now.Subtract(LastSave).TotalSeconds < Main.SAVE_INTERVAL) return;

            File.WriteAllText(Main.ACTOR_SAVE_DIR + Path.DirectorySeparatorChar + ID + ".json", JsonConvert.SerializeObject(this, Formatting.Indented));
            LastSave = DateTime.Now;
        }

        public void Destroy()
        {
            Blip.delete();
            Label.delete();
            Ped.delete();
            API.shared.deleteColShape(ColShape);
        }
    }
    #endregion
}
