using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using pcrpg.src.Player.Utils;
using System.Collections.Generic;

namespace pcrpg.src.Job.Taxi
{
    public class Taximeter
    {
        public bool Enabled { get; set; }

        public int Fare { get; set; }

        public Taximeter()
        {
            Fare = 10;
            Enabled = false;
        }
    }

    class Taxi : Script
    {
        Dictionary<Client, int> PlayerTaxiFare = new Dictionary<Client, int>();
        Dictionary<Client, Taximeter> PlayerTaximeter = new Dictionary<Client, Taximeter>();

        public Taxi()
        {
            API.onResourceStart += OnResourceStart;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "RequestTaxiMenu":
                    {
                        if (!sender.isInVehicle) return;
                        else if (sender.vehicle.model != (int)VehicleHash.Taxi) return;
                        else if (sender.getJob() != (int)JobType.Taxi) return;

                        sender.triggerEvent("ShowTaxiMenu");
                        break;
                    }
                case "ToggleTaximeter":
                    {
                        bool enabled = (bool)arguments[0];
                        PlayerTaximeter[sender].Enabled = enabled;
                        sender.sendChatAction(enabled ? "ligou o taxímetrô." : "desligou o taxímetrô.");

                        if (enabled)
                        {
                            foreach (var passenger in API.getAllPlayers())
                            {
                                if (passenger.vehicle != sender.vehicle || sender == passenger) continue;
                                PlayerTaxiFare.Add(passenger, 0);
                            }
                        }
                        else
                        {
                            if (!PlayerTaximeter.ContainsKey(sender)) return;
                            else if (!PlayerTaximeter[sender].Enabled) return;

                            int total = 0;
                            var vehicle = sender.vehicle;
                            foreach (var passenger in API.getAllPlayers())
                            {
                                if (!passenger.isInVehicle) continue;
                                else if (passenger.vehicle != vehicle) continue;
                                else if (passenger == sender) continue;
                                else if (!PlayerTaxiFare.ContainsKey(passenger)) continue;

                                var money = PlayerTaxiFare[passenger];
                                total += money;

                                passenger.giveMoney(-money);
                                passenger.sendNotification("", $"- ${money}");

                                PlayerTaxiFare.Remove(passenger);
                            }

                            sender.giveMoney(total);
                            sender.sendNotification("", $"+ ${total}");

                            PlayerTaximeter.Remove(sender);
                        }
                        
                        break;
                    }
                case "TaximeterValue":
                    {
                        int value = (int)arguments[0];
                        sender.sendChatAction(value > PlayerTaximeter[sender].Fare ? "aumentou o preço do taxímetrô." : "abaixou o preço do taxímetrô.");
                        PlayerTaximeter[sender].Fare = value;
                        break;
                    }
            }
        }

        private void OnResourceStart()
        {
            API.startTimer(15000, true, () =>
            {
                foreach (var player in API.getAllPlayers())
                {
                    if (!player.isInVehicle) continue;
                    else if (!PlayerTaxiFare.ContainsKey(player)) continue;

                    var driver = API.getVehicleDriver(player.vehicle);
                    if (driver == null) continue;
                    else if (!PlayerTaximeter.ContainsKey(driver)) continue;

                    PlayerTaxiFare[player] += PlayerTaximeter[driver].Fare;
                }
            });
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (API.getEntityModel(vehicle) != (int)VehicleHash.Taxi) return;

            if (seat != -1)
            {
                var driver = API.getVehicleDriver(vehicle);

                if (driver == null) return;
                else if (!PlayerTaximeter.ContainsKey(driver)) return;
                else if (!PlayerTaximeter[driver].Enabled) return;

                PlayerTaxiFare.Add(player, 0);
            }
            else if (player.getJob() == (int)JobType.Taxi)
            {
                if (!PlayerTaximeter.ContainsKey(player)) PlayerTaximeter.Add(player, new Taximeter());
                if (!PlayerTaximeter[player].Enabled) player.sendNotification("", "Seu taxímetro está desligado, aperte ~y~B~w~.");
            }
        }

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (API.getEntityModel(vehicle) != (int)VehicleHash.Taxi) return;

            if (seat != -1)
            {
                var driver = API.getVehicleDriver(vehicle);

                if (driver == null) return;
                else if (!PlayerTaxiFare.ContainsKey(player)) return;

                var money = PlayerTaxiFare[player];
                player.sendNotification("", $"- ${money}");
                driver.sendNotification("", $"+ ${money}");

                player.giveMoney(-money);
                driver.giveMoney(money);

                PlayerTaxiFare.Remove(player);
            }
            else if (player.getJob() == (int)JobType.Taxi)
            {
                if (!PlayerTaximeter.ContainsKey(player)) return;
                else if (!PlayerTaximeter[player].Enabled) return;

                int total = 0;
                foreach (var passenger in API.getAllPlayers())
                {
                    if (!passenger.isInVehicle) continue;
                    else if (passenger.vehicle != vehicle) continue;
                    else if (passenger == player) continue;
                    else if (!PlayerTaxiFare.ContainsKey(passenger)) continue;

                    var money = PlayerTaxiFare[passenger];
                    total += money;

                    passenger.giveMoney(-money);
                    passenger.sendNotification("", $"- ${money}");

                    PlayerTaxiFare.Remove(passenger);
                }

                player.giveMoney(total);
                player.sendNotification("", $"+ ${total}");

                PlayerTaximeter.Remove(player);
            }
        }
    }
}
