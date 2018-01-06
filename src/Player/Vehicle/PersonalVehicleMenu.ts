/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var menu = null;
var blip = null;

API.onKeyDown.connect(function (sender, arg)
{
    if (arg.KeyCode == Keys.F3 && !API.isChatOpen())
    {
        if (menu != null)
            if (menu.Visible)
                return;

        API.triggerServerEvent("on_player_request_vehicle_list");
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    if (eventName === "retrieve_vehicle_list")
    {
        var vehicles = JSON.parse(args[0]);
        if(vehicles.length > 0)
        {
            menu = API.createMenu("Meus veículos", "Selecione um veículo", 0, 0, 6);
            for (var i = 0; i < vehicles.length; i++)
            {
                menu.AddItem(API.createMenuItem(API.getVehicleDisplayName(vehicles[i].Model), "Localização: " + API.getZoneName(new Vector3(vehicles[i].PositionX, vehicles[i].PositionY, vehicles[i].PositionZ))));
            }

            menu.OnItemSelect.connect(function (sender, item, index)
            {
                menu.Visible = false;
                API.triggerServerEvent("on_player_select_owned_vehicle", vehicles[index].Id);
            });

            menu.Visible = true;
        }
        else
        {
            API.sendNotification("Você não tem veículos.");
        }
    }
    else if (eventName === "show_owned_vehicle_blip")
    {
        if (blip != null)
        {
            API.deleteEntity(blip);
            blip = null;
        }

        var vehicle = args[0];

        blip = API.createBlip(API.getEntityPosition(vehicle));
        API.setBlipColor(blip, 43);
        API.setBlipSprite(blip, 225);
        API.setBlipName(blip, "Meu veículo");
        API.attachEntity(blip, vehicle, "", new Vector3(), new Vector3());
    }
    else if (eventName === "on_player_enter_owned_vehicle")
    {
        if(blip != null)
        {
            API.deleteEntity(blip);
            blip = null;
        }
    }
    else if (eventName === "hide_owned_vehicle_blip")
    {
        if (blip != null)
        {
            API.deleteEntity(blip);
            blip = null;
        }
    }
});