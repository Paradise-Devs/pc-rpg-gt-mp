using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using pcrpg.src.Database.Models;
using System;
using System.Linq;

namespace pcrpg.src.Player.Creation
{
    class CharacterSelection : Script
    {        
        public CharacterSelection()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "RetrieveCharactersList")
            {
                if (!API.hasEntityData(sender, "User")) return;
                User user = API.getEntityData(sender, "User");
                
                var characters = ContextFactory.Instance.Characters.Where(up => up.UserId == user.Id);
                API.triggerClientEvent(sender, "UpdateCharactersList", JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
            else if (eventName == "SelectCharacter")
            {
                int characterId = (int)arguments[0];
                var character = ContextFactory.Instance.Characters.FirstOrDefault(up => up.Id == characterId);
                API.setEntityData(sender, "Character", character);
                API.setEntityData(sender, "LoggedInTime", DateTime.Now);

                sender.name = character.Name;

                // Sync player face with other players
                Faces.GTAOnlineCharacter gtao = new Faces.GTAOnlineCharacter();
                gtao.InitializePedFace(sender);
                gtao.UpdatePlayerFace(sender);

                Managers.DimensionManager.DismissPrivateDimension(sender);
                API.setEntityDimension(sender, 0);
            }
            else if (eventName == "DeleteCharacter")
            {
                int characterId = (int)arguments[0];
                var character = ContextFactory.Instance.Characters.FirstOrDefault(up => up.Id == characterId);
                if (character != null)
                {
                    ContextFactory.Instance.Characters.Remove(character);
                    ContextFactory.Instance.SaveChanges();
                }

                if (!API.hasEntityData(sender, "User")) return;
                User user = API.getEntityData(sender, "User");

                var characters = ContextFactory.Instance.Characters.Where(up => up.UserId == user.Id);
                API.triggerClientEvent(sender, "UpdateCharactersList", JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
        }
    }
}
