using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Server.Chat;
using System;

namespace pcrpg.src.Player.Utils
{
    public static class ClientExtensions
    {
        public static bool isLogged(this Client player)
        {
            if (Data.Character.ContainsKey(player)) return true;
            return false;
        }

        public static int getBank(this Client player)
        {
            if (!Data.Character.ContainsKey(player)) return 0;
            return Data.Character[player].Bank;
        }

        public static int getMoney(this Client player)
        {
            if (!Data.Character.ContainsKey(player)) return 0;
            return Data.Character[player].Money;
        }

        public static void giveMoney(this Client player, int money)
        {
            if (!Data.Character.ContainsKey(player)) return;

            Data.Character[player].Money = money;
            API.shared.triggerClientEvent(player, "UpdateMoneyHUD", Convert.ToString(Data.Character[player].Money), Convert.ToString(money));
        }

        public static int? getJob(this Client player)
        {
            if (!Data.Character.ContainsKey(player)) return null;
            return Data.Character[player].Job;
        }

        public static void setJob(this Client player, int? job)
        {
            if (!Data.Character.ContainsKey(player)) return;
            Data.Character[player].Job = job;
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
