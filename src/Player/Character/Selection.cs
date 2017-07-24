using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.Database.Models;
using System.Linq;
using System.Web.Script.Serialization;

namespace pcrpg.src.Player.Character
{
    class Selection : Script
    {
        public Selection()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "RetrieveCharactersList")
            {
                if (!API.hasEntityData(sender, "User")) return;

                Users user = API.getEntityData(sender, "User");
                var characterList = from characters
                                    in ContextFactory.Instance.Characters
                                    where characters.UserId == user.Id
                                    join traits in ContextFactory.Instance.CharacterTraits
                                    on characters.Id equals traits.CharacterId
                                    select new { characters.Id, characters.Name, characters.Gender, characters.Level, characters.Money, characters.Bank, characters.LastLogin, characters.PlayedTime, traits };

                API.triggerClientEvent(sender, "UpdateCharactersList", new JavaScriptSerializer().Serialize(characterList));
            }
        }
    }
}
