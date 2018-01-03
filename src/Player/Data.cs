using System;
using System.Linq;
using pcrpg.src.Database.Models;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Player.Utils;

namespace pcrpg.src.Player
{
    class Data : Script
    {
        public Data()
        {
            API.onResourceStop += OnResourceStop;
            API.onPlayerDisconnected += OnPlayerDisconnect;
        }        

        private void OnPlayerDisconnect(Client player, string reason)
        {
            SaveCharacterData(player);
        }

        private void OnResourceStop()
        {
            var players = API.getAllPlayers();
            foreach (var player in players)
            {
                SaveCharacterData(player);
                API.resetEntityData(player, "User");
                API.resetEntityData(player, "Character");
            }
        }

        private void SaveCharacterData(Client player)
        {
            if (API.hasEntityData(player, "Character"))
            {
                using (var ctx = new ContextFactory().Create())
                {
                    Character tempCharacter = player.getData("Character");
                    Character character = ctx.Characters.FirstOrDefault(x => x.Id == tempCharacter.Id);

                    character.Job = player.getJob();
                    character.Money = player.getMoney();

                    character.PositionX = player.position.X;
                    character.PositionY = player.position.Y;
                    character.PositionZ = player.position.Z;
                    character.RotationZ = player.rotation.Z;

                    character.PlayedTime += (DateTime.Now - tempCharacter.LastLogin).TotalSeconds;
                    character.LastLogin = DateTime.Now;

                    player.setData("Character", character);

                    ctx.SaveChanges();
                }
            }
        }
    }
}
