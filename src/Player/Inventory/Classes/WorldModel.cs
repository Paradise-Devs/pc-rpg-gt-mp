using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Player.Inventory.Classes
{
    public class WorldModel
    {
        public string ModelName { get; private set; }
        public Vector3 Offset { get; private set; }
        public Vector3 Rotation { get; private set; }

        public WorldModel(string modelName, Vector3 offset, Vector3 rotation)
        {
            ModelName = modelName;
            Offset = offset;
            Rotation = rotation;
        }
    }
}
