using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using pcrpg.src.Database.Models;
using System;
using System.Linq;

namespace pcrpg.src.Player.Selection
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
                using (var ctx = new ContextFactory().Create())
                {
                    var characters = ctx.Characters.Where(up => up.UserId == user.Id).ToList();
                    API.triggerClientEvent(sender, "UpdateCharactersList", JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                }
            }
            else if (eventName == "SelectCharacter")
            {
                int characterId = (int)arguments[0];
                using (var ctx = new ContextFactory().Create())
                {
                    var character = ctx.Characters.FirstOrDefault(up => up.Id == characterId);
                    API.setEntityData(sender, "Character", character);

                    sender.name = character.Name;
                    character.LastLogin = DateTime.Now;
                    Vehicle.Vehicle.SpawnPlayerPersonalVehicles(sender);

                    API.setEntityPositionFrozen(sender, false);
                    API.setEntityPosition(sender, new Vector3(character.PositionX, character.PositionY, character.PositionZ));
                    API.setEntityRotation(sender, new Vector3(0f, 0f, character.RotationZ));

                    // Sync player face with other players
                    Customization.CustomizationModel gtao = new Customization.CustomizationModel();
                    gtao.InitializePedFace(sender);
                    gtao.UpdatePlayerFace(sender);

                    Managers.DimensionManager.DismissPrivateDimension(sender);
                    API.setEntityDimension(sender, 0);
                }                    
            }
            else if (eventName == "DeleteCharacter")
            {
                int characterId = (int)arguments[0];
                using (var ctx = new ContextFactory().Create())
                {
                    var character = ctx.Characters.FirstOrDefault(up => up.Id == characterId);
                    if (character != null)
                    {
                        ctx.Characters.Remove(character);
                        ctx.SaveChanges();
                    }

                    if (!API.hasEntityData(sender, "User")) return;
                    User user = API.getEntityData(sender, "User");

                    var characters = ctx.Characters.Where(up => up.UserId == user.Id);
                    API.triggerClientEvent(sender, "UpdateCharactersList", JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                }
            }
        }
    }
}
