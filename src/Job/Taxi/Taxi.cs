﻿using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Player.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pcrpg.src.Job.Taxi
{
    public class Taximeter
    {
        public bool Enabled { get; set; }

        public int Value { get; set; }
    }

    class Taxi : Script
    {
        Dictionary<Client, Vehicle> PlayerVehicle = new Dictionary<Client, Vehicle>();
        Dictionary<Client, Taximeter> PlayerTaximeter = new Dictionary<Client, Taximeter>();

        public Taxi()
        {
            API.onPlayerDisconnected += OnPlayerDisconnected;
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnPlayerDisconnected(Client player, string reason)
        {
            if (PlayerVehicle.ContainsKey(player))
            {
                if (API.doesEntityExist(PlayerVehicle[player]))
                    API.deleteEntity(PlayerVehicle[player]);

                PlayerVehicle.Remove(player);
            }

            if (PlayerTaximeter.ContainsKey(player))
            {
                PlayerTaximeter.Remove(player);
            }
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "JobService":
                    {
                        if (!sender.hasData("JobMarker_ID")) return;

                        Job job = Main.Jobs.FirstOrDefault(h => h.ID == sender.getData("JobMarker_ID"));
                        if (job == null) return;

                        string serviceName = (string)arguments[0];
                        if (serviceName == "Solicitar taxi da empresa")
                        {
                            if (job.Vehicles.Count < 1)
                            {
                                sender.sendNotification("ERROR", "Estamos sem veículos no momento.");
                                return;
                            }

                            if (PlayerVehicle.ContainsKey(sender))
                            {
                                if (API.doesEntityExist(PlayerVehicle[sender]))
                                    API.deleteEntity(PlayerVehicle[sender]);

                                PlayerVehicle.Remove(sender);
                            }

                            Random random = new Random();
                            int i = random.Next(job.Vehicles.Count);
                            var vehicle = API.createVehicle(job.Vehicles[i].Model, job.Vehicles[i].Position, job.Vehicles[i].Rotation, job.Vehicles[i].PrimaryColor, job.Vehicles[i].SecondaryColor);
                            vehicle.engineStatus = false;
                            PlayerVehicle.Add(sender, vehicle);
                        }
                        else if (serviceName == "Devolver taxi da empresa")
                        {
                            if (PlayerVehicle.ContainsKey(sender))
                            {
                                if (API.doesEntityExist(PlayerVehicle[sender]))
                                    API.deleteEntity(PlayerVehicle[sender]);

                                PlayerVehicle.Remove(sender);
                            }
                            else
                            {
                                sender.sendNotification("", "Você não solicitou um veículo.");
                            }
                        }
                        break;
                    }
                case "RequestTaxiMenu":
                    {
                        if (sender.getJob() != (int)JobType.Taxi || !sender.isInVehicle) return;
                        if (sender.vehicle != PlayerVehicle[sender]) return;
                        sender.triggerEvent("ShowTaxiMenu");
                        break;
                    }
                case "ToggleTaximeter":
                    {
                        bool enabled = (bool)arguments[0];
                        if (!PlayerTaximeter.ContainsKey(sender)) PlayerTaximeter.Add(sender, new Taximeter { Enabled = false, Value = 10 });
                        PlayerTaximeter[sender].Enabled = enabled;
                        sender.sendChatAction(enabled ? "ligou o taxímetro." : "desligou o taxímetro.");
                        break;
                    }
                case "TaximeterValue":
                    {
                        int.TryParse((string)arguments[0], out int value);
                        if (!PlayerTaximeter.ContainsKey(sender)) PlayerTaximeter.Add(sender, new Taximeter { Enabled = false, Value = 10 });
                        PlayerTaximeter[sender].Value = value;
                        break;
                    }
            }
        }
    }
}