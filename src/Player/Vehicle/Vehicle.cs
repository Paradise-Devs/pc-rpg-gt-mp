using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Database.Models;
using pcrpg.src.Gameplay.Parkinglot;
using pcrpg.src.Player.Utils;
using System;
using System.Linq;

namespace pcrpg.src.Player.Vehicle
{
    class Vehicle : Script
    {
        const int DepositPrice = 100;

        public Vehicle()
        {
            API.onClientEventTrigger += OnClientEvent;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
        }

        private void OnClientEvent(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "on_player_request_vehicle_list")
            {
                if (!sender.isLogged()) return;

                var vehicles = (from v in Data.Character[sender].Vehicles select new { v.Id, v.Model, v.PositionX, v.PositionY, v.PositionZ }).ToList();
                API.triggerClientEvent(sender, "retrieve_vehicle_list", API.toJson(vehicles));
            }
            else if (eventName == "on_player_select_owned_vehicle")
            {
                if (sender.hasData("ParkinglotMarker_ID") && sender.getMoney() < DepositPrice)
                {
                    sender.sendNotification("", "Você não tem dinheiro suficiente.");
                    return;
                }

                int vehicle_id = (int)arguments[0];
                var vehicle = Data.Character[sender].Vehicles.FirstOrDefault(x => x.Id == vehicle_id);
                if (vehicle == null) return;

                if (sender.hasData("ParkinglotMarker_ID"))
                {
                    Parkinglot parkinglot = Gameplay.Parkinglot.Main.Parkinglots.FirstOrDefault(h => h.ID == sender.getData("ParkinglotMarker_ID"));
                    if (parkinglot == null) return;
                    else if (parkinglot.Spawns.Count < 1)
                    {
                        sender.sendNotification("", "Este estacionamento está fechado.");
                        return;
                    }

                    var rand = new Random();
                    var parkingSpace = parkinglot.Spawns[rand.Next(parkinglot.Spawns.Count)];
                    API.setEntityPosition(vehicle.Vehicle, parkingSpace.Position);
                    API.setEntityRotation(vehicle.Vehicle, parkingSpace.Rotation);
                    sender.giveMoney(-DepositPrice);
                }
                else if (sender.position.DistanceTo(API.getEntityPosition(vehicle.Vehicle)) < 40f)
                {
                    bool isLocked = API.getVehicleLocked(vehicle.Vehicle);
                    if (isLocked) sender.sendChatAction((sender.isInVehicle) ? ((vehicle.Vehicle == sender.vehicle) ? "destranca o veículo." : "destranca o veículo com o controle remoto.") : "destranca o veículo com o controle remoto.");
                    else sender.sendChatAction((sender.isInVehicle) ? ((vehicle.Vehicle == sender.vehicle) ? "tranca o veículo." : "tranca o veículo com o controle remoto.") : "tranca o veículo com o controle remoto.");
                    API.setVehicleLocked(vehicle.Vehicle, !isLocked);
                }
                else
                {
                    API.triggerClientEvent(sender, "show_owned_vehicle_blip", vehicle.Vehicle);
                }
            }
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (Data.Character.ContainsKey(player))
            {
                foreach (var personalVehicle in Data.Character[player].Vehicles)
                {
                    if (personalVehicle.Vehicle == vehicle)
                    {
                        API.triggerClientEvent(player, "on_player_enter_owned_vehicle");
                    }
                }
            }
        }
    }
}
