/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var menu = null;

API.onKeyDown.connect((sender: any, e: System.Windows.Forms.KeyEventArgs) => {
    if (API.isChatOpen() || API.isAnyMenuOpen() || !API.getHudVisible()) return;

    if (e.KeyCode === Keys.M) {
        if (menu == null)
            createInteractionMenu();

        if (menu.Visible)
            menu.Visible = false;
        else {
            if (API.isAnyMenuOpen()) return;
            menu.Visible = true;
        }
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
    if (eventName == "InteractionMenu_SetWaypointPosition") {
        API.setWaypoint(args[0], args[1]);
    }
});

function createInteractionMenu() {
    menu = API.createMenu(API.getPlayerName(API.getLocalPlayer()), "Menu para acesso rápido", 0, 0, 6);

    // GPS
    var list = new List(String);
    list.Add("Nenhum");
    list.Add("Banco");
    list.Add("Concessionária");
    list.Add("Estacionamento");
    var list_item = API.createListItem("GPS Rápido", "Selecione para marcar uma localização no mapa.", list, 0);

    list_item.OnListChanged.connect(function (sender, new_index) {
        switch (new_index) {
            case 0:
                API.removeWaypoint();
                break;
            default:
                API.triggerServerEvent("InteractionMenu_GetWaypointPosition", new_index);
                break
        }
    });

    menu.AddItem(list_item);

    // Call taxi
    var temp_item = API.createMenuItem("Chamar taxi", "Chama um taxi para sua posição atual.");
    temp_item.Activated.connect(function (sender, item) {
        API.triggerServerEvent("InteractionMenu_CallTaxi");
    });
    menu.AddItem(temp_item);

    // Suicide
    temp_item = API.createMenuItem("Cometer suicídio", "Tem certeza que deseja fazer isso?");
    temp_item.Activated.connect(function (sender, item) {
        menu.Visible = false;
        API.triggerServerEvent("InteractionMenu_Suicide");
    });
    menu.AddItem(temp_item);
}
