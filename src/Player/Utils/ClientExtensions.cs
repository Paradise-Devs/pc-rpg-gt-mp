using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Server.Chat;
using System;

namespace pcrpg.src.Player.Utils
{
    public static class ClientExtensions
    {
        public static int getMoney(this Client player)
        {
            if (player.hasData("Character"))
            {
                return player.getData("Character").Money;
            }
            return 0;
        }

        public static void giveMoney(this Client player, int money)
        {
            if (player.hasData("Character"))
            {
                player.getData("Character").Money += money;
                API.shared.triggerClientEvent(player, "UpdateMoneyHUD", Convert.ToString(player.getData("Character").Money), Convert.ToString(money));
            }
        }

        public static int? getJob(this Client player)
        {
            if (player.hasData("Character"))
            {
                return player.getData("Character").Job;
            }
            return null;
        }

        public static void setJob(this Client player, int? job)
        {
            if (player.hasData("Character"))
            {
                player.getData("Character").Job = job;
            }
        }

        public static void sendChatAction(this Client player, string action, int style = 0)
        {
            var msg = (style == 0) ? $"~p~* {player.name} {action}" : $"~p~* {action} (({player.name}))";
            var players = API.shared.getPlayersInRadiusOfPlayer(15f, player);

            foreach (var p in players)
            {
                if (p.dimension != player.dimension) continue;
                ChatApi.SendClientMessage(p, msg, "Local");
            }
        }
    }
}
