using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;

namespace pcrpg.src.Gameplay.Dealership
{
    #region DealershipVehicle Class
    public class DealershipVehicle
    {
        public int Model { get; }
        public int PrimaryColor { get; }
        public int SecondaryColor { get; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public int Price { get; }

        [JsonIgnore]
        public Vehicle Vehicle { get; private set; }

        [JsonIgnore]
        public TextLabel Label { get; private set; }

        public DealershipVehicle(int model, int color1, int color2, Vector3 position, Vector3 rotation, int price)
        {
            Model = model;
            PrimaryColor = color1;
            SecondaryColor = color2;

            Position = position;
            Rotation = rotation;

            Price = price;
        }

        public void Create(int dimension = 0)
        {
            Vehicle = API.shared.createVehicle(Model, Position, Rotation, PrimaryColor, SecondaryColor, dimension);
            Vehicle.invincible = true;
            Vehicle.engineStatus = false;

            Label = API.shared.createTextLabel($"~p~{Vehicle.displayName}~s~\nPreço: ~g~${Price}", new Vector3(0, 0, 0), 15f, 0.5f, true, dimension);
            API.shared.attachEntityToEntity(Label, Vehicle, "0", new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }

        public void Destroy()
        {
            if (Vehicle != null) Vehicle.delete();
            if (Label != null) Label.delete();
        }
    }
    #endregion
}
