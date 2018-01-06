/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var weapon_menus = [];

var weapon_craft_menu = API.createMenu("Criar arma", "Selecione uma categoria", 0, 0, 6);
API.setMenuBannerSprite(weapon_craft_menu, "shopui_title_gr_gunmod", "shopui_title_gr_gunmod");
weapon_menus.push(weapon_craft_menu);

var weaponClassNames = [
    "Pistolas",
    "Metralladoras",
    "Rifles de assalto",
    "Francotirador",
    "Escopetas"
];

var weaponHashes = [
    { class: 0, cost: 5, hash: 453432689 }, // Pistol
    { class: 0, cost: 5, hash: -1716589765 }, // Pistol50
    { class: 0, cost: 5, hash: -1045183535 }, // Revolver
    { class: 1, cost: 10, hash: 324215364 }, // MicroSMG
    { class: 1, cost: 25, hash: 736523883 }, // SMG
    { class: 1, cost: 10, hash: 1627465347 }, // Gusenberg
    { class: 2, cost: 50, hash: -1074790547 }, // AssaltRifle
    { class: 2, cost: 50, hash: 2132975508 },// BullpupRifle
    { class: 3, cost: 500, hash: 100416529 },// SniperRifle
    { class: 4, cost: 75, hash: 487013001 },// PumpShotgun
    { class: 4, cost: 100, hash: 2017895192 }// SawnOffShotgun
];

API.onResourceStart.connect(() => {
    var groupedWeapons = [];

    for (var i = 0; i < weaponHashes.length; i++) {
        var weaponHash = weaponHashes[i].hash;
        var weaponCost = weaponHashes[i].cost;
        var weaponClass = weaponClassNames[weaponHashes[i].class];

        if (groupedWeapons[weaponClass] == undefined) {
            groupedWeapons[weaponClass] = [];
        }

        groupedWeapons[weaponClass].push({ hash: weaponHash, cost: weaponCost, name: API.getWeaponName(weaponHash) });
    }

    for (var group in groupedWeapons) {
        if (!groupedWeapons.hasOwnProperty(group)) continue;

        var groupName = group;
        var weapons = groupedWeapons[group];
        var categoryMenu = createWeaponCategory(groupName);

        for (var i = 0; i < weapons.length; i++) {
            var weapon = weapons[i];
            createSpawnWeaponItem(weapon.name, weapon.hash, weapon.cost, categoryMenu);
        }
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) => {
    switch (eventName) {
        case "ShowWeaponCraftMenu":
            weapon_craft_menu.Visible = true;
            break;
    }
});

function createWeaponCategory(name) {
    var weaponCategoryMenu = API.createMenu("Criar arma", 0, 0, 6);
    API.setMenuBannerSprite(weaponCategoryMenu, "shopui_title_gr_gunmod", "shopui_title_gr_gunmod");
    weapon_menus.push(weaponCategoryMenu);

    var weaponCategoryItem = API.createMenuItem(name, "");

    weapon_craft_menu.AddItem(weaponCategoryItem);
    weapon_craft_menu.BindMenuToItem(weaponCategoryMenu, weaponCategoryItem);

    return weaponCategoryMenu;
}

function createSpawnWeaponItem(name, hash, cost, parentMenu) {
    var menuItem = API.createMenuItem(name, "");
    menuItem.SetRightLabel(cost.toString());
    menuItem.Activated.connect(function (menu, item) {
        API.triggerServerEvent("CREATE_WEAPON", hash, cost);

        for (var i = 0; i < weapon_menus.length; i++) {
            if (weapon_menus[i].Visible) {
                weapon_menus[i].Visible = false;
            }
        }
    });
    parentMenu.AddItem(menuItem);
}