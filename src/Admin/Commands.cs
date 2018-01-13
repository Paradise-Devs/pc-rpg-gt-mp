using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Database.Models;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Extensions;
using pcrpg.src.Player.Utils;
using pcrpg.src.Server.Chat;

namespace pcrpg.src.Admin
{
    class Commands : Script
    {
        [Command("admincmds", Alias = "acmds")]
        public void CommandsCommand(Client player)
        {
            if (!player.IsAdmin())
            {
                API.sendNotificationToPlayer(player, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~~~~~ Comandos Admin ~~~~~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /ir - /puxar - /congelar - /descongelar - irpos - /say - /limparchat - /sethp - /kill - /pm");
            API.sendChatMessageToPlayer(player, "* /givemoney - /giveitem - /clearitems - /setadmin");
            API.sendChatMessageToPlayer(player, "* /dcmds - /pcmds - /hcmds - /jcmds - /bcmds");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~~~~~ Comandos Admin ~~~~~~~~~~~~~~~~~~~~~");
        }

        [Command("ir", "~y~USO: ~w~/ir [jogador]")]
        public void GoTo(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (sender == target)
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não pode ir até você mesmo.");
                return;
            }

            else if (!target.isLogged())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                return;
            }

            if (!sender.isInVehicle) API.setEntityPosition(sender, target.position);
            else API.setEntityPosition(sender.vehicle, target.position);

            API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou até {target.name}.");
        }

        [Command("puxar", "~y~USO: ~w~/puxar [jogador]")]
        public void GetHere(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (sender == target)
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não pode puxar você mesmo.");
            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                if (!target.isInVehicle)
                    API.setEntityPosition(target, sender.position);
                else
                    API.setEntityPosition(target.vehicle, sender.position);

                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} teleportou você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou {target.name} até você.");
            }
        }

        [Command("congelar", "~y~USO: ~w~/congelar [jogador]")]
        public void Freeze(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (sender == target)
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não pode congelar você mesmo.");
            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.freeze(true);
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} congelou você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você congelou {target.name}.");
            }
        }

        [Command("descongelar", "~y~USO: ~w~/descongelar [jogador]")]
        public void Unfreeze(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.freeze(false);
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} descongelou você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você descongelou {target.name}.");
            }
        }

        [Command("irpos", "~y~USO: ~w~/irpos [X] [Y] [Z]")]
        public void GoToPos(Client sender, float x, float y, float z)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            sender.position = new Vector3(x, y, z);
            API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou até a coordenada {x}, {y}, {z}.");
        }

        [Command("say", "~y~USO: ~w~/say [mensagem]", GreedyArg = true)]
        public void Say(Client sender, string message)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }
            ChatApi.SendClientMessageToAll($"~o~* Admin {sender.name} diz: ~w~{message}", "Global");
        }

        [Command("limparchat")]
        public void ClearChat(Client sender)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            ChatApi.ClearAllChat();
            API.sendNotificationToAll("O chat foi limpo por um admin");
        }

        [Command("sethp", "~y~USO: ~w~/sethp [vida] [jogador]")]
        public void SetHealth(Client sender, int health, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.health = health;
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} alterou sua vida para {health}%.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você alterou a vida de {target.name} para {health}%.");
            }
        }

        [Command("kill", "~y~USO: ~w~/kill [jogador]")]
        public void KillPlayer(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.kill();
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} matou você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você matou {target.name}.");
            }
        }

        [Command("pm", "~y~USO: ~w~/pm [jogador] [mensagem]", GreedyArg = true)]
        public void PrivateMessage(Client sender, Client target, string message)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                ChatApi.SendClientMessage(target, $"~y~PM de {target.name}: {message}", "Admin");
                ChatApi.SendClientMessage(sender, $"~y~PM para {target.name}: {message}", "Admin");
            }
        }

        [Command("givemoney", "~y~USO: ~w~/givemoney [dinheiro] [jogador]")]
        public void GiveMoney(Client sender, int money, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.giveMoney(money);
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} deu ~g~${money} ~w~para você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você deu ~g~${money} ~w~para {target.name}.");
            }
        }

        [Command("giveitem", "~y~USO: ~w~/giveitem [jogador] [item] [qtd]")]
        public void GiveItem(Client sender, Client target, ItemID item, int amount)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.giveItem(item, amount);
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} deu ~g~{item} ~w~para você.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você deu ~g~{item} ~w~para {target.name}.");
            }
        }

        [Command("clearitems", "~y~USO: ~w~/clearitems [jogador]")]
        public void ClearItems(Client sender, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                target.clearInventory();
                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} limpou seu inventário.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você limpou o inventário de {target.name}.");
            }
        }

        [Command("setadmin", "~y~USO: ~w~/setadmin [nivel] [jogador]")]
        public void SetAdmin(Client sender, int level, Client target)
        {
            if (!sender.IsAdmin())
            {
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            else if (!target.isLogged())
                API.sendNotificationToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
            else
            {
                User targetUser = API.getEntityData(target, "User");
                targetUser.Admin = level;

                API.sendNotificationToPlayer(target, $"~r~ADMIN: ~w~{sender.name} alterou seu nível de admin para {level}.");
                API.sendNotificationToPlayer(sender, $"~r~ADMIN: ~w~Você alterou o nível de admin de {target.name} para {level}.");
            }
        }
    }
}
