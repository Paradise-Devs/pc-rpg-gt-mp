using System;
using pcrpg.src.Database.Models;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;

namespace pcrpg.src.Player
{
    class Data : Script
    {
        public Data()
        {     
            API.onPlayerDisconnected += OnPlayerDisconnect;      
        }        

        private void OnPlayerDisconnect(Client player, string reason)
        {
            if (API.hasEntityData(player, "Character"))
            {
                Character character = API.getEntityData(player, "Character");
                character.LastLogin = DateTime.Now;
                character.PlayedTime += (DateTime.Now - API.getEntityData(player, "LoggedInTime")).TotalSeconds;
            }
            ContextFactory.Instance.SaveChanges();
        }
    }
}
