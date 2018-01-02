using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using pcrpg.src.Player.Utils;
using System.Timers;

namespace pcrpg.src.Gameplay.Vehicles
{
    class VehicleEngine : Script
    {
        public VehicleEngine()
        {
            API.onResourceStart += OnResourceStart;
            API.onClientEventTrigger += OnClientEvent;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
        }

        private void OnResourceStart()
        {
            Timer statusUpdate = new Timer();
            statusUpdate.Elapsed += new ElapsedEventHandler(OnResourceUpdate);
            statusUpdate.Interval = 60000;
            statusUpdate.Enabled = true;

        }

        private void OnResourceUpdate(object sender, ElapsedEventArgs e)
        {
            foreach (var vehicle in API.getAllVehicles())
            {
                var fuel = API.getVehicleFuelLevel(vehicle);
                if (API.getVehicleEngineStatus(vehicle) && fuel > 0 && !API.isVehicleABicycle((VehicleHash)API.getEntityModel(vehicle)))
                {
                    API.setVehicleFuelLevel(vehicle, (fuel >= 1 && fuel <= 3) ? 0 : fuel - 1);
                }
            }
        }

        private void OnClientEvent(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "on_player_toggle_engine")
            {
                if (!sender.isInVehicle || Dealership.Main.IsADealershipVehicle(sender.vehicle)) return;
                sender.vehicle.engineStatus = !sender.vehicle.engineStatus;
                sender.sendChatAction((sender.vehicle.engineStatus) ? "ligou o motor do veículo" : "desligou o motor do veículo.");
            }
        }

        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle, int seat)
        {
            if (!API.getVehicleEngineStatus(vehicle) && !Dealership.Main.IsADealershipVehicle(vehicle) && seat == -1)
            {
                player.sendNotification("", "O motor deste veículo está desligado, para ligar pressione ~g~Y~s~.");
            }
        }
    }
}
