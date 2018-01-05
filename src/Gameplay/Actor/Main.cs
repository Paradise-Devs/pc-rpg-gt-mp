using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pcrpg.src.Gameplay.Actor
{
    public class Main : Script
    {
        // settings
        public static string ACTOR_SAVE_DIR = "data/Actor";
        public static int SAVE_INTERVAL = 120;

        public static List<Actor> Actors = new List<Actor>();

        public Main()
        {
            API.onResourceStart += Actor_Init;
            API.onClientEventTrigger += Actor_ClientEvent;
            API.onResourceStop += Actor_Exit;
        }

        private void Actor_ClientEvent(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "ActorInteract":
                    {
                        if (!sender.hasData("ActorMarker_ID")) return;
                        
                        Actor actor = Actors.FirstOrDefault(h => h.ID == sender.getData("ActorMarker_ID"));
                        if (actor == null) return;

                        if (!PDialogues.PlayerDialogues.ContainsKey(sender.handle))
                            PDialogues.PlayerDialogues.Add(sender.handle, new List<PlayerDialogue>() { new PlayerDialogue(actor.ID, 0, null) });

                        var dialogData = PDialogues.PlayerDialogues[sender.handle].FirstOrDefault(d => d.ActorId == actor.ID);
                        if (dialogData == null)
                        {
                            dialogData = new PlayerDialogue(actor.ID, 0, null);
                            PDialogues.PlayerDialogues[sender.handle].Add(dialogData);
                        }
                        
                        API.triggerClientEvent(sender, "ShowActorDialogue", API.toJson(actor.Dialogues), API.toJson(dialogData));
                        break;
                    }
                case "OnSelectAnswer":
                    {
                        if (!sender.hasData("ActorMarker_ID")) return;

                        Actor actor = Actors.FirstOrDefault(h => h.ID == sender.getData("ActorMarker_ID"));
                        if (actor == null) return;

                        int answerid = (int)arguments[0];
                        var dialogData = PDialogues.PlayerDialogues[sender.handle].FirstOrDefault(d => d.ActorId == actor.ID);

                        dialogData.CurrentMessage++;
                        dialogData.LastAnswer = answerid;
                        API.triggerClientEvent(sender, "ShowActorDialogue", API.toJson(actor.Dialogues), API.toJson(dialogData));
                        break;
                    }
            }
        }

        #region Methods
        public static Guid GetGuid()
        {
            Guid new_guid;

            do
            {
                new_guid = Guid.NewGuid();
            } while (Actors.Count(h => h.ID == new_guid) > 0);

            return new_guid;
        }
        #endregion

        #region Events
        public void Actor_Init()
        {
            // load settings
            if (API.hasSetting("actorDirName")) ACTOR_SAVE_DIR = API.getSetting<string>("actorDirName");

            ACTOR_SAVE_DIR = API.getResourceFolder() + Path.DirectorySeparatorChar + ACTOR_SAVE_DIR;
            if (!Directory.Exists(ACTOR_SAVE_DIR)) Directory.CreateDirectory(ACTOR_SAVE_DIR);

            if (API.hasSetting("saveInterval")) SAVE_INTERVAL = API.getSetting<int>("saveInterval");
            API.consoleOutput("-> Save Interval: {0}", TimeSpan.FromSeconds(SAVE_INTERVAL).ToString(@"hh\:mm\:ss"));

            // load parking lots
            foreach (string file in Directory.EnumerateFiles(ACTOR_SAVE_DIR, "*.json"))
            {
                Actor actor = JsonConvert.DeserializeObject<Actor>(File.ReadAllText(file));
                Actors.Add(actor);
            }

            API.consoleOutput("Loaded {0} actors.", Actors.Count);
        }

        public void Actor_Exit()
        {
            foreach (Actor actor in Actors)
            {
                actor.Save(true);
                actor.Destroy();
            }

            Actors.Clear();
        }
        #endregion
    }
}
