using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using pcrpg.src.Admin;
using System.IO;
using System.Linq;

namespace pcrpg.src.Gameplay.Parkinglot
{
    class Commands : Script
    {
        [Command("parkinglotcmds", Alias = "pcmds")]
        public void CommandsCommand(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Estacionamento ~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /createparkinglot - /deleteparkinglot - /addparkinglotspawn - /deleteparkinglotspawn");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Estacionamento ~~~~~~~~~~~~~~~~~");
        }

        [Command("createparkinglot")]
        public void CMD_CreateParkinglot(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            Parkinglot new_parkinglot = new Parkinglot(Main.GetGuid(), player.position);
            new_parkinglot.Save();

            Main.Parkinglots.Add(new_parkinglot);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você criou um estacionamento.");
        }

        [Command("deleteparkinglot")]
        public void CMD_RemoveParkinglot(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("ParkinglotMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima do checkpoint do estacionamento que deseja excluir.");
                return;
            }

            Parkinglot parkinglot = Main.Parkinglots.FirstOrDefault(h => h.ID == player.getData("ParkinglotMarker_ID"));
            if (parkinglot == null) return;

            parkinglot.Destroy();
            Main.Parkinglots.Remove(parkinglot);

            string parkinglot_file = Main.PARKINGLOT_SAVE_DIR + Path.DirectorySeparatorChar + parkinglot.ID + ".json";
            if (File.Exists(parkinglot_file)) File.Delete(parkinglot_file);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você deletou o estacionamento.");
        }

        [Command("addparkinglotspawn")]
        public void CMD_AddParkinglotSpawn(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.isInVehicle)
                API.sendChatMessageToPlayer(player, $"~r~ERRO: ~s~Você não está em um veículo.");
            else
            {
                foreach (Parkinglot parkinglot in Main.Parkinglots)
                {
                    if (parkinglot.Position.DistanceTo(player.position) < 40f)
                    {
                        parkinglot.Spawns.Add(new ParkingSpace { Position = player.vehicle.position, Rotation = player.vehicle.rotation });
                        parkinglot.Save();
                        API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você adicionou um spawn em sua posição.");
                        return;
                    }
                }

                player.sendNotification("", $"Você não está próximo de um estacionamento.");
            }
        }

        [Command("deleteparkinglotspawn")]
        public void CMD_DeleteParkinglotSpawn(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            foreach (Parkinglot parkinglot in Main.Parkinglots)
            {
                if (parkinglot.Position.DistanceTo(player.position) < 40f)
                {
                    foreach (ParkingSpace space in parkinglot.Spawns)
                    {
                        if (parkinglot.Position.DistanceTo(player.position) < 2f)
                        {
                            parkinglot.Spawns.Remove(space);
                            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você removeu o spawn de sua posição.");
                            parkinglot.Save();
                            return;
                        }
                    }
                }
            }

            player.sendNotification("", $"Você não está próximo de uma vaga de um estacionamento.");
        }
    }
}
