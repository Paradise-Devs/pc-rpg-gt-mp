using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Database.Models;
using pcrpg.src.Gameplay.Parkinglot;
using pcrpg.src.Player.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pcrpg.src.Player.Vehicle
{
    public class PlayerVehicle
    {
        public int Id;

        public NetHandle Entity;
    }

    class Vehicle : Script
    {
        const int DepositPrice = 100;

        public static Dictionary<Client, List<PlayerVehicle>> PlayerVehicles = new Dictionary<Client, List<PlayerVehicle>>();

        public Vehicle()
        {
            API.onClientEventTrigger += OnClientEvent;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            API.onPlayerDisconnected += OnPlayerDisconnected;
        }

        private void OnClientEvent(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "on_player_request_vehicle_list")
            {
                if (API.hasEntityData(sender, "Character"))
                {
                    Character character = API.getEntityData(sender, "Character");
                    using (var ctx = new ContextFactory().Create())
                    {
                        var vehicles = (from v in ctx.CharacterVehicle where v.Character.Id == character.Id select new { v.Id, v.Model, v.PositionX, v.PositionY, v.PositionZ }).ToList();
                        API.triggerClientEvent(sender, "retrieve_vehicle_list", API.toJson(vehicles));
                    }
                }
            }
            else if (eventName == "on_player_select_owned_vehicle")
            {
                if (sender.hasData("ParkinglotMarker_ID") && sender.getMoney() < DepositPrice)
                {
                    API.sendChatMessageToPlayer(sender, "~r~ERRO: ~s~Você não tem dinheiro suficiente.");
                    return;
                }

                int vehicle_id = (int)arguments[0];
                var vehicle = PlayerVehicles[sender].FirstOrDefault(x => x.Id == vehicle_id);
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
                    API.setEntityPosition(vehicle.Entity, parkingSpace.Position);
                    API.setEntityRotation(vehicle.Entity, parkingSpace.Rotation);
                    sender.giveMoney(-DepositPrice);
                }
                else
                {
                    Vector3 position = API.getEntityPosition(vehicle.Entity);
                    API.triggerClientEvent(sender, "show_owned_vehicle_blip", position.X, position.Y, position.Z);
                }
            }
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (PlayerVehicles.ContainsKey(player))
            {
                foreach (var personalVehicle in PlayerVehicles[player])
                {
                    if (personalVehicle.Entity == vehicle)
                    {
                        API.triggerClientEvent(player, "on_player_enter_owned_vehicle");
                    }
                }
            }
        }

        private void OnPlayerDisconnected(Client player, string reason)
        {
            if (PlayerVehicles.ContainsKey(player))
            {
                SavePlayerPersonalVehicles(player, true);
                PlayerVehicles.Remove(player);
            }
        }

        private void SavePlayerPersonalVehicles(Client player, bool deleteAfterSaving = false)
        {
            if (!PlayerVehicles.ContainsKey(player)) return;

            using (var ctx = new ContextFactory().Create())
            {
                foreach (var vehicle in PlayerVehicles[player])
                {
                    // Saving last position
                    float fuel = API.getVehicleFuelLevel(vehicle.Entity);
                    var position = API.getEntityPosition(vehicle.Entity);
                    var rotation = API.getEntityRotation(vehicle.Entity);

                    CharacterVehicle _veh = ctx.CharacterVehicle.FirstOrDefault(x => x.Id == vehicle.Id);

                    _veh.PositionX = position.X;
                    _veh.PositionY = position.Y;
                    _veh.PositionZ = position.Z;
                    _veh.RotationX = rotation.X;
                    _veh.RotationY = rotation.Y;
                    _veh.RotationZ = rotation.Z;

                    _veh.Fuel = (int)fuel;

                    if (deleteAfterSaving)
                    {
                        if (API.doesEntityExist(vehicle.Entity)) API.deleteEntity(vehicle.Entity);
                    }
                }
                ctx.SaveChanges();
            }
        }

        public static void SpawnPlayerPersonalVehicles(Client player)
        {
            if (PlayerVehicles.ContainsKey(player))
            {
                foreach (var vehicle in PlayerVehicles[player])
                {
                    if (API.shared.doesEntityExist(vehicle.Entity)) API.shared.deleteEntity(vehicle.Entity);
                }
            }

            Character character = API.shared.getEntityData(player, "Character");
            if (character == null) return;

            using (var ctx = new ContextFactory().Create())
            {
                Character _char = ctx.Characters.First(x => x.Id == character.Id);

                foreach (var vehicle in _char.Vehicles)
                {
                    var veh = API.shared.createVehicle(vehicle.Model, new Vector3(vehicle.PositionX, vehicle.PositionY, vehicle.PositionZ), new Vector3(vehicle.RotationX, vehicle.RotationY, vehicle.RotationZ), vehicle.Color1, vehicle.Color2);
                    API.shared.setVehicleEngineStatus(veh, false);
                    API.shared.setVehicleFuelLevel(veh, (vehicle.Fuel >= 1 && vehicle.Fuel <= 3) ? 4 : vehicle.Fuel);

                    if (PlayerVehicles.ContainsKey(player))
                        PlayerVehicles[player].Add(new PlayerVehicle { Id = vehicle.Id, Entity = veh });
                    else
                        PlayerVehicles.Add(player, new List<PlayerVehicle> { new PlayerVehicle { Id = vehicle.Id, Entity = veh } });
                }
            }
        }

        [Command("trancar", Alias = "destrancar")]
        public void LockVehCommand(Client player)
        {
            if (PlayerVehicles.ContainsKey(player))
            {
                NetHandle closestVehicle = new NetHandle();
                float distanceFromClosestVehicle = 40f;
                foreach (var vehicle in PlayerVehicles[player])
                {
                    var distance = player.position.DistanceTo(API.getEntityPosition(vehicle.Entity));
                    if (player.position.DistanceTo(API.getEntityPosition(vehicle.Entity)) < 40.0f && distance < distanceFromClosestVehicle)
                    {
                        closestVehicle = vehicle.Entity;
                        distanceFromClosestVehicle = distance;
                    }
                }

                foreach (var vehicle in PlayerVehicles[player])
                {
                    if (vehicle.Entity == closestVehicle)
                    {
                        bool isLocked = API.getVehicleLocked(vehicle.Entity);
                        if (isLocked) player.sendChatAction((player.isInVehicle) ? ((vehicle.Entity == player.vehicle) ? "desbloquea el vehículo." : "desbloquea el vehículo con control remoto.") : "desbloquea el vehículo con control remoto.");
                        else player.sendChatAction((player.isInVehicle) ? ((vehicle.Entity == player.vehicle) ? "bloquea el vehículo." : "bloquea el vehículo con control remoto.") : "bloquea el vehículo con control remoto.");
                        API.setVehicleLocked(vehicle.Entity, !isLocked);
                        return;
                    }
                }

                API.sendChatMessageToPlayer(player, "~r~ERRO: ~s~Você está muito longe de seu veículo.");
            }
            else
            {
                API.sendChatMessageToPlayer(player, "~r~ERRO: ~s~Você não tem um veículo.");
            }
        }
    }
}
