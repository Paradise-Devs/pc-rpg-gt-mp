using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Database.Models;

namespace pcrpg.src.Server.Chat
{
    public class ChatApi : Script
    {
        public ChatApi()
        {
            API.onChatMessage += OnChatMessage;
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnChatMessage(Client sender, string message, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "OnSendChatMessage")
            {
                string message = (string)arguments[0];
                string tab = (string)arguments[1];

                if (tab.Equals("Local"))
                {
                    var players = API.getPlayersInRadiusOfPlayer(15f, sender);
                    string formattedMessage = $"~w~{sender.name} diz: {message}";
                    foreach (var player in players)
                    {
                        SendClientMessage(player, formattedMessage, tab);
                    }
                }
                else if (tab.Equals("Global"))
                {
                    string formattedMessage = $"~c~{sender.name}: ~w~{message}";
                    SendClientMessageToAll(formattedMessage, tab);
                }
                else if (tab.Equals("Admin"))
                {
                    if (!API.hasEntityData(sender, "User")) return;

                    User user = API.getEntityData(sender, "User");
                    if (user.Admin > 0)                    
                    {
                        string formattedMessage = $"~o~{sender.name} diz: ~w~{message}";
                        SendClientMessageToAll(formattedMessage, tab);
                    }
                }
            }
        }

        public static void SendClientMessage(Client player, string message, string tab)
        {            
            API.shared.triggerClientEvent(player, "OnCommitChatMessage", message, tab);
        }

        public static void SendClientMessageToAll(string message, string tab)
        {
            API.shared.triggerClientEventForAll("OnCommitChatMessage", message, tab);
        }

        public static void ClearAllChat()
        {
            API.shared.triggerClientEventForAll("ClearChat");
        }
    }
}
