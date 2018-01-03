"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
var house_text_mode = 0;
var remove_selected_furniture = false;
var house_purchase_menu = null;
var house_menu = null;
var house_safe_menu = null;
var house_guns_main_menu = null;
var house_guns_locker_menu = null;
var house_furnitures_main_menu = null;
var house_furnitures_buy_menu = null;
var house_furnitures_menu = null;
var house_menus = [];
function h_numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function hideMenus() {
    for (var i = 0; i < house_menus.length; i++)
        house_menus[i].Visible = false;
}
API.onServerEventTrigger.connect(function (event_name, args) {
    switch (event_name) {
        case "ShowHouseText":
            house_text_mode = args[0];
            if (house_text_mode == 0)
                hideMenus();
            break;
        case "House_PurchaseMenu":
            var data = JSON.parse(args[0]);
            if (house_purchase_menu == null) {
                house_purchase_menu = API.createMenu("Casa", "~g~Esta casa está a venda!", 0, 0, 6);
                house_menus.push(house_purchase_menu);
                house_purchase_menu.OnItemSelect.connect(function (menu, item, index) {
                    if (index == 2)
                        API.triggerServerEvent("HousePurchase");
                });
            }
            house_purchase_menu.Clear();
            var temp_item = API.createMenuItem("Interior", "Interior da casa.");
            temp_item.SetRightLabel(data.Interior);
            house_purchase_menu.AddItem(temp_item);
            temp_item = API.createMenuItem("Preço", "Preço da casa.");
            temp_item.SetRightLabel("$" + h_numberWithCommas(data.Price));
            house_purchase_menu.AddItem(temp_item);
            temp_item = API.createColoredItem("Comprar", "Comprar a casa.", "#4caf50", "#ffffff");
            house_purchase_menu.AddItem(temp_item);
            house_purchase_menu.Visible = true;
            break;
        case "HouseMenu":
            var data = JSON.parse(args[0]);
            if (house_menu == null) {
                house_menu = API.createMenu("Casa", "~b~Controles da casa", 0, 0, 6);
                house_menus.push(house_menu);
                house_menu.OnCheckboxChange.connect(function (menu, item, checked) {
                    API.triggerServerEvent("HouseSetLock", checked);
                });
                house_menu.OnItemSelect.connect(function (menu, item, index) {
                    switch (index) {
                        case 0:
                            var name = API.getUserInput("", 31);
                            if (name.length > 0)
                                API.triggerServerEvent("HouseSetName", name);
                            break;
                        case 5:
                            API.triggerServerEvent("HouseSell");
                            break;
                    }
                });
            }
            if (house_safe_menu == null) {
                house_safe_menu = API.createMenu("Casa", "~b~Cofre", 0, 0, 6);
                house_menus.push(house_safe_menu);
                house_safe_menu.ParentMenu = house_menu;
                var temp_safe_item = API.createMenuItem("Depositar", "Coloca dinheiro na casa.");
                house_safe_menu.AddItem(temp_safe_item);
                temp_safe_item = API.createMenuItem("Sacar", "Retira dinheiro da casa.");
                house_safe_menu.AddItem(temp_safe_item);
                house_safe_menu.OnItemSelect.connect(function (menu, item, index) {
                    var amount = API.getUserInput("", 9);
                    if (parseInt(amount) > 1)
                        API.triggerServerEvent("HouseSafe", index, amount);
                });
            }
            if (house_guns_main_menu == null) {
                house_guns_main_menu = API.createMenu("Casa", "~b~Armário de armas", 0, 0, 6);
                house_menus.push(house_guns_main_menu);
                house_guns_locker_menu = API.createMenu("Casa", "~b~Armário de armas (Armazenado)", 0, 0, 6);
                house_menus.push(house_guns_locker_menu);
                house_guns_main_menu.ParentMenu = house_menu;
                house_guns_locker_menu.ParentMenu = house_guns_main_menu;
                var temp_guns_item = API.createMenuItem("Colocar arma", "Coloca sua arma atual no armário.");
                house_guns_main_menu.AddItem(temp_guns_item);
                temp_guns_item = API.createMenuItem("Retirar arma", "Retira uma arma do armário.");
                house_guns_main_menu.AddItem(temp_guns_item);
                house_guns_main_menu.BindMenuToItem(house_guns_locker_menu, temp_guns_item);
                house_guns_main_menu.OnItemSelect.connect(function (menu, item, index) {
                    if (index == 0)
                        API.triggerServerEvent("HousePutGun");
                });
                house_guns_locker_menu.OnItemSelect.connect(function (menu, item, index) {
                    API.triggerServerEvent("HouseTakeGun", index);
                });
            }
            if (house_furnitures_main_menu == null) {
                house_furnitures_main_menu = API.createMenu("Casa", "~b~Móveis", 0, 0, 6);
                house_menus.push(house_furnitures_main_menu);
                house_furnitures_buy_menu = API.createMenu("Casa", "~b~Catálogo de móveis", 0, 0, 6);
                house_menus.push(house_furnitures_buy_menu);
                house_furnitures_menu = API.createMenu("Casa", "~b~Móveis", 0, 0, 6);
                house_menus.push(house_furnitures_menu);
                house_furnitures_main_menu.ParentMenu = house_menu;
                house_furnitures_buy_menu.ParentMenu = house_furnitures_main_menu;
                house_furnitures_menu.ParentMenu = house_furnitures_main_menu;
                var temp_furnitures_item = API.createMenuItem("Comprar móveis", "Compra móveis.");
                house_furnitures_main_menu.AddItem(temp_furnitures_item);
                house_furnitures_main_menu.BindMenuToItem(house_furnitures_buy_menu, temp_furnitures_item);
                temp_furnitures_item = API.createMenuItem("Editar móvel", "Seleciona um móvel para editar.");
                house_furnitures_main_menu.AddItem(temp_furnitures_item);
                house_furnitures_main_menu.BindMenuToItem(house_furnitures_menu, temp_furnitures_item);
                temp_furnitures_item = API.createMenuItem("Vender móvel", "Seleciona um móvel para vender.");
                house_furnitures_main_menu.AddItem(temp_furnitures_item);
                house_furnitures_main_menu.BindMenuToItem(house_furnitures_menu, temp_furnitures_item);
                house_furnitures_main_menu.OnItemSelect.connect(function (menu, item, index) {
                    switch (index) {
                        case 0:
                            API.triggerServerEvent("HouseFurnitureCatalogue");
                            break;
                        case 1:
                            remove_selected_furniture = false;
                            break;
                        case 2:
                            remove_selected_furniture = true;
                            break;
                    }
                });
                house_furnitures_buy_menu.OnItemSelect.connect(function (menu, item, index) {
                    API.triggerServerEvent("HouseBuyFurniture", index);
                });
                house_furnitures_menu.OnItemSelect.connect(function (menu, item, index) {
                    API.triggerServerEvent(((remove_selected_furniture) ? "HouseSellFurniture" : "HouseEditFurniture"), index);
                });
            }
            // house main menu
            house_menu.Clear();
            var temp_item = API.createMenuItem("Nome da casa", "Altera o nome de sua casa.");
            house_menu.AddItem(temp_item);
            temp_item = API.createCheckboxItem("Trancada", "Tranca/destranca a sua casa.", data.Locked);
            house_menu.AddItem(temp_item);
            temp_item = API.createMenuItem("Cofre", "Opções do cofre.");
            house_menu.AddItem(temp_item);
            house_menu.BindMenuToItem(house_safe_menu, temp_item);
            temp_item = API.createMenuItem("Armário de armas", "Opções do armário de armas.");
            house_menu.AddItem(temp_item);
            house_menu.BindMenuToItem(house_guns_main_menu, temp_item);
            temp_item = API.createMenuItem("Móveis", "Opções dos móveis.");
            house_menu.AddItem(temp_item);
            house_menu.BindMenuToItem(house_furnitures_main_menu, temp_item);
            temp_item = API.createColoredItem("Vender casa", "Vende a casa.", "#4caf50", "#ffffff");
            house_menu.AddItem(temp_item);
            // house safe menu
            API.setMenuSubtitle(house_safe_menu, "~b~Cofre ~g~($" + h_numberWithCommas(data.Money) + ")");
            // house guns menu
            house_guns_locker_menu.Clear();
            for (var i = 0; i < data.Weapons.length; i++) {
                var temp_locker_item = API.createMenuItem(API.getWeaponName(data.Weapons[i].Hash), "");
                temp_locker_item.SetRightLabel("Munição: " + h_numberWithCommas(data.Weapons[i].Ammo));
                temp_locker_item.Description = ((data.Weapons[i].Components.length > 0) ? "Tem componentes." : "Não tem componentes.");
                if (data.Weapons[i].Tint > 0)
                    temp_locker_item.Description += " (Tingida)";
                house_guns_locker_menu.AddItem(temp_locker_item);
            }
            // house furnitures menu
            house_furnitures_menu.Clear();
            for (var i = 0; i < data.Furnitures.length; i++) {
                var temp_furniture_item = API.createMenuItem(data.Furnitures[i].Name, "");
                temp_furniture_item.SetRightLabel("$" + h_numberWithCommas(Math.round(data.Furnitures[i].Price * 0.8)));
                house_furnitures_menu.AddItem(temp_furniture_item);
            }
            house_menu.Visible = true;
            break;
        case "HouseUpdateSafe":
            if (house_safe_menu != null) {
                var data = JSON.parse(args[0]);
                API.setMenuSubtitle(house_safe_menu, "~b~Cofre ~g~($" + h_numberWithCommas(data.Money) + ")");
            }
            break;
        case "HouseUpdateWeapons":
            if (house_guns_locker_menu != null) {
                var data = JSON.parse(args[0]);
                house_guns_locker_menu.Clear();
                for (var i = 0; i < data.length; i++) {
                    var temp_locker_item = API.createMenuItem(API.getWeaponName(data[i].Hash), "");
                    temp_locker_item.SetRightLabel("Munição: " + h_numberWithCommas(data[i].Ammo));
                    temp_locker_item.Description = ((data[i].Components.length > 0) ? "Tem componentes." : "Não tem componentes.");
                    if (data[i].Tint > 0)
                        temp_locker_item.Description += " (Tingida)";
                    house_guns_locker_menu.AddItem(temp_locker_item);
                }
            }
            break;
        case "HouseFurnitureCatalogue":
            if (house_furnitures_buy_menu != null) {
                var data = JSON.parse(args[0]);
                house_furnitures_buy_menu.Clear();
                for (var i = 0; i < data.length; i++) {
                    var temp_furniture_item = API.createMenuItem(data[i].Name, "");
                    temp_furniture_item.SetRightLabel("$" + h_numberWithCommas(data[i].Price));
                    house_furnitures_buy_menu.AddItem(temp_furniture_item);
                }
            }
            break;
        case "HouseUpdateFurnitures":
            if (house_furnitures_menu != null) {
                var data = JSON.parse(args[0]);
                house_furnitures_menu.Clear();
                for (var i = 0; i < data.length; i++) {
                    var temp_furniture_item = API.createMenuItem(data[i].Name, "");
                    temp_furniture_item.SetRightLabel("$" + h_numberWithCommas(Math.round(data[i].Price * 0.8)));
                    house_furnitures_menu.AddItem(temp_furniture_item);
                }
            }
            break;
        case "UpdateHouseBlip":
            API.setBlipColor(args[0], 69);
            API.setBlipShortRange(args[0], false);
            break;
        case "ResetHouseBlip":
            API.setBlipColor(args[0], 0);
            API.setBlipShortRange(args[0], true);
            break;
    }
});
API.onKeyDown.connect(function (e, key) {
    if (API.isChatOpen())
        return;
    switch (key.KeyCode) {
        case Keys.E:
            if (resource.FurnitureEditor.editing_handle != null)
                return;
            if (house_text_mode == 1) {
                API.triggerServerEvent("HouseInteract");
            }
            else if (house_text_mode == 2) {
                API.triggerServerEvent("HouseLeave");
            }
            break;
        case Keys.M:
            if (resource.FurnitureEditor.editing_handle != null)
                return;
            hideMenus();
            API.triggerServerEvent("HouseMenu");
            break;
    }
});
API.onEntityStreamIn.connect(function (ent, entType) {
    if (entType === 3) {
        var count = API.getEntitySyncedData(ent, "PlayersInside");
        if (count < 1) {
            API.callNative("HIDE_NUMBER_ON_BLIP", ent);
        }
        else {
            API.callNative("SHOW_NUMBER_ON_BLIP", ent, count);
        }
    }
});
API.onUpdate.connect(function () {
    if (house_text_mode > 0)
        API.displaySubtitle(((house_text_mode == 1) ? "Pressione ~y~E ~w~para interagir." : "Pressione ~y~E ~w~para sair."), 100);
});
