using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;

namespace pcrpg.src.Job
{
    #region JobVehicle Class
    public class JobVehicle
    {
        public int Model { get; }
        public int PrimaryColor { get; }
        public int SecondaryColor { get; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        [JsonIgnore]
        public Vehicle Vehicle { get; private set; }

        public JobVehicle(int model, int colorprimary, int colorsecondary, Vector3 position, Vector3 rotation)
        {
            Model = model;
            PrimaryColor = colorprimary;
            SecondaryColor = colorsecondary;

            Position = position;
            Rotation = rotation;
        }

        public void Create(int dimension = 0)
        {
            Vehicle = API.shared.createVehicle(Model, Position, Rotation, PrimaryColor, SecondaryColor, dimension);
            Vehicle.engineStatus = false;
        }

        public void Destroy()
        {
            if (Vehicle != null) Vehicle.delete();
        }
    }
    #endregion
}
