"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
var is_on_vehicle = false;
var dealership_purchase_menu = null;
var colors = [
    { id: 0, name: "Black" },
    { id: 4, name: "Silver" },
    { id: 27, name: "Red" },
    { id: 36, name: "Orange" },
    { id: 37, name: "Gold" },
    { id: 49, name: "Green" },
    { id: 64, name: "Blue" },
    { id: 88, name: "Yellow" },
    { id: 120, name: "Chrome" },
    { id: 134, name: "White" }
];
var color1 = 0;
var color2 = 0;
function d_numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "OnEnterDealershipVehicle") {
        is_on_vehicle = true;
    }
    else if (eventName == "OnExitDealershipVehicle") {
        is_on_vehicle = false;
    }
    else if (eventName == "Dealership_PurchaseMenu") {
        var data = JSON.parse(args[0]);
        if (dealership_purchase_menu == null) {
            dealership_purchase_menu = API.createMenu("Concessionária", "~g~Este veículo está a venda!", 0, 0, 6);
            dealership_purchase_menu.OnItemSelect.connect(function (menu, item, index) {
                if (index == 3)
                    API.triggerServerEvent("VehiclePurchase", colors[color1].id, colors[color2].id);
                dealership_purchase_menu.Visible = false;
            });
        }
        dealership_purchase_menu.Clear();
        var temp_item = API.createMenuItem("Preço", "Preço do veículo.");
        temp_item.SetRightLabel("$" + d_numberWithCommas(data.Price));
        dealership_purchase_menu.AddItem(temp_item);
        var list = new List(String);
        for (var i = 0, len = colors.length; i < len; i++)
            list.Add(colors[i].name);
        var list1 = API.createListItem("Cor primária", "Selecione a cor do veículo", list, 0);
        dealership_purchase_menu.AddItem(list1);
        var list2 = API.createListItem("Cor secundária", "Selecione a cor do veículo", list, 0);
        dealership_purchase_menu.AddItem(list2);
        dealership_purchase_menu.OnListChange.connect(function (menu, item, newIndex) {
            if (item == list1) {
                color1 = newIndex;
            }
            else if (item == list2) {
                color2 = newIndex;
            }
        });
        temp_item = API.createColoredItem("Comprar", "Selecione esta opção para comprar.", "#4caf50", "#ffffff");
        dealership_purchase_menu.AddItem(temp_item);
        dealership_purchase_menu.Visible = true;
    }
});
API.onKeyDown.connect(function (e, key) {
    if (API.isChatOpen())
        return;
    else if (!is_on_vehicle)
        return;
    else if (API.isAnyMenuOpen())
        return;
    switch (key.KeyCode) {
        case Keys.E:
            API.triggerServerEvent("RequestDealershipBuyMenu");
            break;
    }
});
