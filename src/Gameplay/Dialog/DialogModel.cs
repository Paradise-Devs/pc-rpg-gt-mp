using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Database.Models;

namespace pcrpg.src.Gameplay.DialogSystem
{
    class DialogModel : Script
    {
        public DialogModel()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "OnPlayerTalkToNpc")
            {
                if (!API.hasEntityData(sender, "Character"))
                    return;

                int npcid = (int)arguments[0];
                Character character = API.getEntityData(sender, "Character");

                var dialogData = ContextFactory.Instance.Dialogs.FirstOrDefault(up => up.NpcId == npcid && up.CharacterId == character.Id);
                API.triggerClientEvent(sender, "NpcDialogData", npcid, API.toJson(dialogData));
            }
            else if (eventName == "OnSelectAnswer")
            {
                int npcid = (int)arguments[0];
                int answerid = (int)arguments[1];

                Character character = API.getEntityData(sender, "Character");
                Dialog dialogData = ContextFactory.Instance.Dialogs.FirstOrDefault(up => up.NpcId == npcid && up.CharacterId == character.Id);
                if (dialogData == null)
                {
                    dialogData = new Dialog { CharacterId = character.Id, NpcId = npcid, CurrentConversation = 0 };
                    ContextFactory.Instance.Dialogs.Add(dialogData);
                }
                dialogData.CurrentConversation++;
                dialogData.LastAnswer = answerid;
                API.triggerClientEvent(sender, "NpcDialogData", npcid, API.toJson(dialogData));
                ContextFactory.Instance.SaveChanges();
            }
        }
    }
}
