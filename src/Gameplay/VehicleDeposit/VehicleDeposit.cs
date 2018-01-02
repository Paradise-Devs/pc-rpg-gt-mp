using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Gameplay.VehicleDeposit
{
    class VehicleDeposit : Script
    {
        public VehicleDeposit()
        {
            API.onResourceStart += OnResourceStart;
        }

        private void OnResourceStart()
        {
            Blip blip = API.createBlip(new Vector3(100.3687f, -1073.308f, 29.37412f));
            API.setBlipSprite(blip, 50);
            API.setBlipShortRange(blip, true);
            API.setBlipTransparency(blip, 255);
            API.setBlipColor(blip, 47);
            API.setBlipName(blip, "Estacionamento");

            API.createTextLabel($"~o~Estacionamento~s~\nPresione ~o~F3 ~s~para usar\n~g~$~s~100", new Vector3(100.3687f, -1073.308f, 29.37412f + 0.5), 15f, 0.5f);
            API.createMarker(1, new Vector3(100.3687f, -1073.308f, 29.37412f - 1.0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255, 255, 165, 0);
        }
    }
}
