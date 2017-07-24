using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using pcrpg.Database.Models;
using System.Linq;

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

                var characters = ContextFactory.Instance.Characters.Where(up => up.UserId == user.Id);
                API.triggerClientEvent(sender, "UpdateCharactersList", JsonConvert.SerializeObject(characters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            }
        }
    }
}
