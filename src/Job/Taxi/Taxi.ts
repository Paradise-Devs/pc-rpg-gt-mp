/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var taxi_menu = null;

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
    if (eventName == "ShowTaxiMenu") {
        if (taxi_menu == null) {
            taxi_menu = API.createMenu("Taximêtro", "Ajuste seu taximêtro", 0, 0, 6);

            var temp_item = API.createCheckboxItem("Ligado", "Liga/desliga o taximêtro.", false);
            taxi_menu.AddItem(temp_item);

            taxi_menu.OnCheckboxChange.connect(function (menu, item, checked) {
                API.triggerServerEvent("ToggleTaximeter", checked);
            });

            var list = new List(String);
            for (let i = 10; i <= 100; i++) list.Add(i.toString());
            var list_item = API.createListItem("Preço", "Valor cobrado por minuto.", list, 0);
            taxi_menu.AddItem(list_item);

            list_item.OnListChanged.connect(function (sender, new_index) {
                API.triggerServerEvent("TaximeterValue", list[new_index]);
            });
        }
        taxi_menu.Visible = true;
    }
});

API.onKeyUp.connect((sender: any, key: System.Windows.Forms.KeyEventArgs) => {
    switch (key.KeyCode) {
        case Keys.B:
            if (taxi_menu != null) {
                if (taxi_menu.Visible == true) {
                    taxi_menu.Visible = false;
                    return;
                }
            }

            if (!API.isPlayerInAnyVehicle(API.getLocalPlayer())) return;

            API.triggerServerEvent("RequestTaxiMenu");
            break;
    }
});
