using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Database.Models;
using pcrpg.src.Managers;

namespace pcrpg.src.Admin
{
    class Commands : Script
    {
        [Command("ir", "~y~USO: ~w~/ir [jogador]", GreedyArg = true)]
        public void GoTo(Client sender, string targetName)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (sender == target)
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não pode ir até você mesmo.");
                else if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    if (!sender.isInVehicle)
                        API.setEntityPosition(sender, target.position);
                    else
                        API.setEntityPosition(sender.vehicle, target.position);

                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} teleportou até você.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou até {target.name}.");                
                }
            }
        }

        [Command("puxar", "~y~USO: ~w~/puxar [jogador]", GreedyArg = true)]
        public void GetHere(Client sender, string targetName)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (sender == target)
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não pode puxar você mesmo.");
                else if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    if (!target.isInVehicle)
                        API.setEntityPosition(target, sender.position);
                    else
                        API.setEntityPosition(target.vehicle, sender.position);

                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} teleportou você.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou {target.name} até você.");
                }
            }
        }

        [Command("congelar", "~y~USO: ~w~/congelar [jogador]", GreedyArg = true)]
        public void Freeze(Client sender, string targetName)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (sender == target)
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não pode congelar você mesmo.");
                else if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    target.freeze(true);
                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} congelou você.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você congelou {target.name}.");
                }
            }
        }

        [Command("descongelar", "~y~USO: ~w~/descongelar [jogador]", GreedyArg = true)]
        public void Unfreeze(Client sender, string targetName)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    target.freeze(true);
                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} descongelou você.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você descongelou {target.name}.");
                }
            }
        }

        [Command("irpos", "~y~USO: ~w~/irpos [X] [Y] [Z]", GreedyArg = true)]
        public void GoToPos(Client sender, string x, string y, string z)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                sender.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
                API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você teleportou até a coordenada {x}, {y}, {z}.");
            }
        }

        [Command("say", "~y~USO: ~w~/say [mensagem]", GreedyArg = true)]
        public void Say(Client sender, string message)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
                API.sendChatMessageToAll("~#FF4444~", $"* Admin {sender} diz: ~w~{message}");
        }

        [Command("limparchat")]
        public void ClearChat(Client sender)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                for (int i = 0; i < 25; i++)
                {
                    API.sendChatMessageToAll(" ");
                }

                API.sendNotificationToAll("O chat foi limpo por um admin");
            }
        }

        [Command("sethp", "~y~USO: ~w~/sethp [jogador] [vida]", GreedyArg = true)]
        public void SetHealth(Client sender, string targetName, string health)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    target.health = int.Parse(health);
                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} alterou sua vida para {health}%.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você alterou a vida de {target.name} para {health}%.");
                }
            }
        }

        [Command("kill", "~y~USO: ~w~/kill [jogador]", GreedyArg = true)]
        public void KillPlayer(Client sender, string targetName)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    target.kill();
                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} matou você.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você matou {target.name}.");
                }
            }
        }

        [Command("pm", "~y~USO: ~w~/pm [jogador] [mensagem]", GreedyArg = true)]
        public void PrivateMessage(Client sender, string targetName, string message)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    API.sendChatMessageToPlayer(target, "~#FFFF00~", $"PM de {target.name}: {message}");
                    API.sendChatMessageToPlayer(sender, "~#FFFF00~", $"PM para {target.name}: {message}");
                }
            }
        }

        [Command("setadmin", "~y~USO: ~w~/setadmin [jogador] [nivel]", GreedyArg = true)]
        public void SetAdmin(Client sender, string targetName, string level)
        {
            User user = API.getEntityData(sender, "User");
            if (user.Admin < 1)
                API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você não tem permissão.");
            else
            {
                Client target = CommandManager.FindPlayer(sender, targetName);
                if (target == null) return;

                if (!API.hasEntityData(target, "Character"))
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~O jogador não está conectado.");
                else
                {
                    User targetUser = API.getEntityData(target, "User");
                    targetUser.Admin = int.Parse(level);

                    API.sendChatMessageToPlayer(target, $"~r~ADMIN: ~w~{sender.name} alterou seu nível de admin para {level}.");
                    API.sendChatMessageToPlayer(sender, $"~r~ADMIN: ~w~Você alterou o nível de admin de {target.name} para {level}.");
                }
            }
        }
    }
}
