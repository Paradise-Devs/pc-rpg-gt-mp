using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace pcrpg.src.Gameplay.Actor
{
    #region PlayerDialogue Class
    public class PlayerDialogue
    {
        public Guid ActorId { get; }
        public int CurrentMessage { get; set; }
        public int? LastAnswer { get; set; }

        public PlayerDialogue(Guid actorid, int currentmessage, int? lastanswer)
        {
            ActorId = actorid;
            CurrentMessage = currentmessage;
            LastAnswer = lastanswer;
        }
    }
    #endregion

    class PDialogues : Script
    {
        public static Dictionary<NetHandle, List<PlayerDialogue>> PlayerDialogues = new Dictionary<NetHandle, List<PlayerDialogue>>();
        public static string SaveLocation = "data/Actor/PlayerDialogues";

        public PDialogues()
        {
            API.onResourceStart += PDialogues_Init;
            API.onPlayerFinishedDownload += PDialogues_PlayerJoin;
            API.onPlayerDisconnected += PDialogues_PlayerLeave;
        }

        private void PDialogues_Init()
        {
            // load settings
            if (API.hasSetting("playerDialoguesDirName")) SaveLocation = API.getSetting<string>("playerDialoguesDirName");

            SaveLocation = API.getResourceFolder() + Path.DirectorySeparatorChar + SaveLocation;
            if (!Directory.Exists(SaveLocation)) Directory.CreateDirectory(SaveLocation);
        }

        private void PDialogues_PlayerJoin(Client player)
        {
            if (PlayerDialogues.ContainsKey(player.handle)) return;
            List<PlayerDialogue> dialogues = null;
            string playerFile = SaveLocation + Path.DirectorySeparatorChar + player.socialClubName + ".json";

            if (File.Exists(playerFile)) dialogues = JsonConvert.DeserializeObject<List<PlayerDialogue>>(File.ReadAllText(playerFile));
            PlayerDialogues.Add(player.handle, dialogues ?? new List<PlayerDialogue>());
        }

        private void PDialogues_PlayerLeave(Client player, string reason)
        {
            if (!PlayerDialogues.ContainsKey(player.handle)) return;
            string playerFile = SaveLocation + Path.DirectorySeparatorChar + player.socialClubName + ".json";
            File.WriteAllText(playerFile, JsonConvert.SerializeObject(PlayerDialogues[player.handle], Formatting.Indented));
        }
    }
}
