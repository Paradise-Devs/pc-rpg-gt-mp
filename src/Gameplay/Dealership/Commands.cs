using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using pcrpg.src.Admin;
using System.IO;
using System.Linq;

namespace pcrpg.src.Gameplay.Dealership
{
    class Commands : Script
    {
        [Command("dealershipcmds", Alias = "dcmds")]
        public void CommandsCommand(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Concessionária ~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /createdealership - /deletedealership - /adddealershipvehicle - /deletedealershipvehicle - /dealershipspawn");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Concessionária ~~~~~~~~~~~~~~~~~");
        }

        [Command("createdealership", GreedyArg = true)]
        public void CMD_CreateDealership(Client player, string name)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            Dealership new_dealership = new Dealership(Main.GetGuid(), player.position, name);
            new_dealership.Save();

            Main.Dealerships.Add(new_dealership);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você criou uma concessionária com o nome: ~g~{name}~s~.");
        }

        [Command("deletedealership")]
        public void CMD_RemoveDealership(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("DealershipMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima do checkpoint da concessionária que deseja excluir.");
                return;
            }

            Dealership dealership = Main.Dealerships.FirstOrDefault(h => h.ID == player.getData("DealershipMarker_ID"));
            if (dealership == null) return;

            dealership.Destroy();
            Main.Dealerships.Remove(dealership);

            string dealership_file = Main.DEALERSHIP_SAVE_DIR + Path.DirectorySeparatorChar + dealership.ID + ".json";
            if (File.Exists(dealership_file)) File.Delete(dealership_file);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você deletou a concessionária: ~g~{dealership.Name}~s~.");
        }

        [Command("adddealershipvehicle", "~y~USO: ~w~/adddealershipvehicle [price]")]
        public void CMD_AddDealershipVehicle(Client sender, int price)
        {
            if (!sender.isInVehicle)
                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está em um veículo.");
            else if (price < 1)
                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Preço inválido.");
            else
            {
                foreach (Dealership dealership in Main.Dealerships)
                {
                    if (dealership.Position.DistanceTo(sender.position) < 40f)
                    {
                        if (dealership.VehicleSpawn.X == 0.0f && dealership.VehicleSpawn.Y == 0.0f)
                        {
                            API.sendChatMessageToPlayer(sender, "~r~ERRO: ~w~Você precisar configurar o spawn desta concessionária primeiro. (/dealershipspawn)");
                            return;
                        }

                        DealershipVehicle new_vehicle = new DealershipVehicle(sender.vehicle.model, sender.vehicle.primaryColor, sender.vehicle.secondaryColor, sender.vehicle.position, sender.vehicle.rotation, price);
                        dealership.Vehicles.Add(new_vehicle);

                        sender.vehicle.delete();
                        sender.position.Z += 0.5f;

                        new_vehicle.Create();
                        dealership.Save();

                        API.sendChatMessageToPlayer(sender, $"~g~SUCESSO: ~s~Você adicionou um {sender.vehicle.displayName} na concessionária: ~g~{dealership.Name}~s~.");
                        return;
                    }
                }

                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está próximo de uma concessionária.");
            }
        }

        [Command("deletedealershipvehicle")]
        public void CMD_RemoveDealershipVehicle(Client sender)
        {
            if (!sender.isInVehicle)
                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está em um veículo.");
            else
            {
                foreach (Dealership dealership in Main.Dealerships)
                {
                    foreach (DealershipVehicle vehicle in dealership.Vehicles)
                    {
                        if (sender.vehicle == vehicle.Vehicle)
                        {
                            vehicle.Destroy();
                            dealership.Vehicles.Remove(vehicle);
                            dealership.Save();

                            API.sendChatMessageToPlayer(sender, $"~g~SUCESSO: ~s~Você deletou um veículo da concessionária: ~g~{dealership.Name}~s~.");
                            return;
                        }
                    }
                }

                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está próximo de uma concessionária.");
            }
        }

        [Command("dealershipspawn")]
        public void CMD_DealershipVehicleSpawn(Client sender)
        {
            if (!sender.isInVehicle)
                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está em um veículo.");
            else
            {
                var distance = 40f;
                foreach (Dealership dealership in Main.Dealerships)
                {
                    if (dealership.Position.DistanceTo(sender.position) < distance)
                    {
                        distance = dealership.Position.DistanceTo(sender.position);
                        dealership.SetSpawn(sender.vehicle.position);
                        API.sendChatMessageToPlayer(sender, $"~g~SUCESSO: ~s~Você alterou o spawn de veículos da concessionária: ~g~{dealership.Name}~s~.");
                        return;
                    }
                }

                API.sendChatMessageToPlayer(sender, $"~r~ERRO: ~s~Você não está próximo de uma concessionária.");
            }
        }
    }
}

