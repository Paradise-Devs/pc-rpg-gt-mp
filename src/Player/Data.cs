using System;
using System.Linq;
using pcrpg.src.Database.Models;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;
using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Player
{
    class Data : Script
    {
        public static Dictionary<Client, Character> Character = new Dictionary<Client, Character>();

        public Data()
        {
            API.onResourceStop += OnResourceStop;
            API.onPlayerDisconnected += OnPlayerDisconnect;
        }        

        private void OnPlayerDisconnect(Client player, string reason)
        {
            SaveAccount(player);

            if (Character.ContainsKey(player)) Character.Remove(player);
        }

        private void OnResourceStop()
        {
            var players = API.getAllPlayers();
            foreach (var player in players)
            {
                SaveAccount(player);

                if (Character.ContainsKey(player)) Character.Remove(player);
            }
        }

        public static void LoadVehicles(Character character)
        {
            foreach (var vehicle in character.Vehicles)
            {
                var veh = API.shared.createVehicle(vehicle.Model, new Vector3(vehicle.PositionX, vehicle.PositionY, vehicle.PositionZ), new Vector3(vehicle.RotationX, vehicle.RotationY, vehicle.RotationZ), vehicle.Color1, vehicle.Color2);
                vehicle.Vehicle = veh;

                API.shared.setVehicleEngineStatus(veh, false);
                API.shared.setVehicleFuelLevel(veh, (vehicle.Fuel >= 1 && vehicle.Fuel <= 3) ? 4 : vehicle.Fuel);
            }
        }

        public static void SaveAccount(Client player)
        {
            if (!Character.ContainsKey(player)) return;

            using (var ctx = new ContextFactory().Create())
            {
                int characterId = Character[player].Id;
                Character character = ctx.Characters.First(x => x.Id == characterId);

                character.Name = player.name;

                character.Gender = Character[player].Gender;
                character.Job = Character[player].Job;
                character.Money = Character[player].Money;
                character.Bank = Character[player].Bank;
                character.Level = Character[player].Level;
                character.Experience = Character[player].Experience;

                character.PositionX = player.position.X;
                character.PositionY = player.position.Y;
                character.PositionZ = player.position.Z;
                character.RotationZ = player.rotation.Z;

                character.PlayedTime += (DateTime.Now - Character[player].LastLogin).TotalSeconds;
                character.LastLogin = DateTime.Now;

                foreach (var vehicle in Character[player].Vehicles.ToList())
                {
                    // Getting vehicle data
                    float fuel = API.shared.getVehicleFuelLevel(vehicle.Vehicle);
                    var position = API.shared.getEntityPosition(vehicle.Vehicle);
                    var rotation = API.shared.getEntityRotation(vehicle.Vehicle);

                    int vehicleId = vehicle.Id;
                    CharacterVehicle characterVehicle = ctx.CharacterVehicle.FirstOrDefault(x => x.Id == vehicleId);

                    if (characterVehicle == null)
                    {
                        characterVehicle = new CharacterVehicle { Model = vehicle.Model, PositionX = vehicle.PositionX, PositionY = vehicle.PositionY, PositionZ = vehicle.PositionZ, RotationX = vehicle.RotationX, RotationY = vehicle.RotationY, RotationZ = vehicle.RotationZ, Color1 = vehicle.Color1, Color2 = vehicle.Color2, Price = vehicle.Price, Fuel = vehicle.Fuel, CreatedAt = DateTime.Now };
                        character.Vehicles.Add(characterVehicle);
                    }

                    characterVehicle.PositionX = position.X;
                    characterVehicle.PositionY = position.Y;
                    characterVehicle.PositionZ = position.Z;
                    characterVehicle.RotationX = rotation.X;
                    characterVehicle.RotationY = rotation.Y;
                    characterVehicle.RotationZ = rotation.Z;

                    characterVehicle.Fuel = (int)fuel;

                    if (API.shared.doesEntityExist(vehicle.Vehicle)) API.shared.deleteEntity(vehicle.Vehicle);
                }

                ctx.SaveChangesAsync();
            }
        }
    }
}
