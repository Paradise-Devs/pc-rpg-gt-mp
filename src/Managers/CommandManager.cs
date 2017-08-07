using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;

namespace pcrpg.src.Managers
{
    public static class CommandManager
    {
        public static Client FindPlayer(Client sender, string playerName)
        {
            playerName = playerName.ToLower();
            List<Client> players = API.shared.getAllPlayers();
            var target = players.Find(x => x.name.ToLower().Equals(playerName));
            if (target != null)
            {
                return target;
            }
            else
            {
                var targetList = players.FindAll(x => x.name.ToLower().Contains(playerName));
                if (targetList.Count < 1)
                {
                    API.shared.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Nenhum jogador com este nome encontrado.");
                    return null;
                }
                else if (targetList.Count > 1)
                {
                    API.shared.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Mais de um jogador encontrado, especifique melhor.");
                    return null;
                }
                else
                {
                    return targetList[0];
                }
            }
        }
    }
}
