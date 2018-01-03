using System.IO;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using pcrpg.src.Admin;

namespace pcrpg.src.Gameplay.House
{
    class Commands : Script
    {
        [Command("housecmds", Alias = "hcmds")]
        public void AdminCommands(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~~~~~~ Comandos Casa ~~~~~~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /createhouse - /sethousename - /sethousetype - /sethouseprice - /deletehouse");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~~~~~~ Comandos Casa ~~~~~~~~~~~~~~~~~~~~~~");
        }

        [Command("createhouse")]
        public void CMD_CreateHouse(Client player, int type, int price)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            if (type < 0 || type >= HouseTypes.HouseTypeList.Count)
            {
                player.sendChatMessage("~r~ERROR: ~w~ID do tipo inválido.");
                return;
            }

            House new_house = new House(Main.GetGuid(), string.Empty, type, player.position, price, false);
            new_house.Dimension = Main.DimensionID;
            new_house.Save();

            Main.Houses.Add(new_house);
            Main.DimensionID++;
        }

        [Command("sethousename", GreedyArg = true)]
        public void CMD_HouseName(Client player, string new_name)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("HouseMarker_ID"))
            {
                player.sendChatMessage("~r~ERROR: ~w~Fique no checkpoint da entrada  da casa que deseja editar.");
                return;
            }

            House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("HouseMarker_ID"));
            if (house == null) return;

            house.SetName(new_name);
            player.sendChatMessage(string.Format("~g~SUCESSO: ~w~Nome alterado para ~y~\"{0}\".", new_name));
        }

        [Command("sethousetype")]
        public void CMD_HouseType(Client player, int new_type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("HouseMarker_ID"))
            {
                player.sendChatMessage("~r~ERROR: ~w~Fique no checkpoint da entrada  da casa que deseja editar.");
                return;
            }

            if (new_type < 0 || new_type >= HouseTypes.HouseTypeList.Count)
            {
                player.sendChatMessage("~r~ERROR: ~w~ID do tipo inválido.");
                return;
            }

            House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("HouseMarker_ID"));
            if (house == null) return;

            house.SetType(new_type);
            player.sendChatMessage(string.Format("~g~SUCESSO: ~w~Tipo alterado para ~y~{0}.", HouseTypes.HouseTypeList[new_type].Name));
        }

        [Command("sethouseprice")]
        public void CMD_HousePrice(Client player, int new_price)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("HouseMarker_ID"))
            {
                player.sendChatMessage("~r~ERROR: ~w~Fique no checkpoint da entrada  da casa que deseja editar.");
                return;
            }

            House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("HouseMarker_ID"));
            if (house == null) return;

            house.SetPrice(new_price);
            player.sendChatMessage(string.Format("~g~SUCESSO: ~w~Preço alterado para ~g~${0:n0}.", new_price));
        }

        [Command("deletehouse")]
        public void CMD_RemoveHouse(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERROR: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("HouseMarker_ID"))
            {
                player.sendChatMessage("~r~ERROR: ~w~Fique no checkpoint da entrada  da casa que deseja apagar.");
                return;
            }

            House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("HouseMarker_ID"));
            if (house == null) return;

            house.Destroy();
            Main.Houses.Remove(house);

            string house_file = Main.HOUSE_SAVE_DIR + Path.DirectorySeparatorChar + house.ID + ".json";
            if (File.Exists(house_file)) File.Delete(house_file);
        }
    }
}
