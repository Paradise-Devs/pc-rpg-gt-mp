using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;

namespace pcrpg.src.Gameplay.House
{
    #region HouseType Class
    public class HouseType
    {
        public string Name { get; }
        public Vector3 Position { get; }

        private Marker Marker;
        private ColShape ColShape;
        private TextLabel Label;

        public HouseType(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }

        public void Create()
        {
            Marker = API.shared.createMarker(1, Position - new Vector3(0.0, 0.0, 1.0), new Vector3(), new Vector3(), new Vector3(1.0, 1.0, 0.5), 150, 64, 196, 255);

            ColShape = API.shared.createCylinderColShape(Position, 0.85f, 0.85f);
            ColShape.onEntityEnterColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.triggerEvent("ShowHouseText", 2);
                }
            };

            ColShape.onEntityExitColShape += (s, ent) =>
            {
                Client player;

                if ((player = API.shared.getPlayerFromHandle(ent)) != null)
                {
                    player.triggerEvent("ShowHouseText", 0);
                }
            };

            Label = API.shared.createTextLabel("Sair da casa", Position, 10f, 0.65f);
        }

        public void Destroy()
        {
            Marker.delete();
            API.shared.deleteColShape(ColShape);
            Label.delete();
        }
    }
    #endregion

    public class HouseTypes : Script
    {
        public static List<HouseType> HouseTypeList = new List<HouseType>
        {
            // name, position
            new HouseType("Low End", new Vector3(266.0969, -1007.33, -101.0086)),
            new HouseType("Medium End", new Vector3(346.55, -1012.159, -99.19622)),
            new HouseType("Modern Apartment", new Vector3(-786.8663, 315.7642, 217.6385)),
            /*new HouseType("Mody Apartment", new Vector3(-787.0749, 315.8198, 217.6386)),
            new HouseType("Vibrant Apartment", new Vector3(-786.6245, 315.6175, 217.6385)),
            new HouseType("Sharp Apartment", new Vector3(-787.0902, 315.7039, 217.6384)),
            new HouseType("Monochrome Apartment", new Vector3(-786.9887, 315.7393, 217.6386)),
            new HouseType("Seductive Apartment", new Vector3(-787.1423, 315.6943, 217.6384)),
            new HouseType("Regal Apartment", new Vector3(-787.029, 315.7113, 217.6385)),
            new HouseType("Aqua Apartment", new Vector3(-786.9469, 315.5655, 217.6383)),*/
            new HouseType("Lux Apartment", new Vector3(-31.05241, -595.2823, 80.03093))
        };

        public HouseTypes()
        {
            API.onResourceStart += HouseTypes_Init;
            API.onResourceStop += HouseTypes_Exit;
        }

        #region Events
        public void HouseTypes_Init()
        {
            foreach (HouseType house_type in HouseTypeList) house_type.Create();
        }

        public void HouseTypes_Exit()
        {
            foreach (HouseType house_type in HouseTypeList) house_type.Destroy();
        }
        #endregion
    }
}
