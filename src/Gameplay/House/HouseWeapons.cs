using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace pcrpg.src.Gameplay.House
{
    #region HouseWeapon Class
    public class HouseWeapon
    {
        public WeaponHash Hash { get; }
        public int Ammo { get; }

        // customization
        public WeaponTint Tint { get; }
        public WeaponComponent[] Components { get; }

        public HouseWeapon(WeaponHash hash, int ammo, WeaponTint tint, WeaponComponent[] components)
        {
            Hash = hash;
            Ammo = ammo;

            Tint = tint;
            Components = components;
        }
    }
    #endregion

    public class HouseWeapons : Script
    {
        public static List<WeaponHash> WeaponBlacklist = new List<WeaponHash>
        {
            WeaponHash.Unarmed,
            WeaponHash.Snowball
        };

        public HouseWeapons()
        {
            API.onClientEventTrigger += HouseWeapons_EventTrigger;
        }

        #region Events
        public void HouseWeapons_EventTrigger(Client player, string event_name, params object[] args)
        {
            switch (event_name)
            {
                case "HousePutGun":
                    {
                        if (!player.hasData("InsideHouse_ID")) return;

                        House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("InsideHouse_ID"));
                        if (house == null) return;

                        if (house.Owner != player.name)
                        {
                            player.sendNotification("Error", "~r~Apenas o dono pode fazer isto.");
                            return;
                        }

                        WeaponHash weapon = player.currentWeapon;
                        if (WeaponBlacklist.Contains(weapon))
                        {
                            player.sendNotification("Error", "~r~Não pode armazenar esta arma.");
                            return;
                        }

                        if (Main.HOUSE_WEAPON_LIMIT > 0 && house.Weapons.Count >= Main.HOUSE_WEAPON_LIMIT)
                        {
                            player.sendNotification("Error", "~r~Limite de armas atingido.");
                            return;
                        }

                        house.Weapons.Add(new HouseWeapon(weapon, player.getWeaponAmmo(weapon), player.getWeaponTint(weapon), player.GetAllWeaponComponents(weapon)));
                        house.Save();

                        player.sendNotification("Success", string.Format("~g~Guardou um(a) {0} com {1:n0} balas.", weapon, player.getWeaponAmmo(weapon)));
                        player.removeWeapon(weapon);

                        player.triggerEvent("HouseUpdateWeapons", API.toJson(house.Weapons));
                        break;
                    }

                case "HouseTakeGun":
                    {
                        if (!player.hasData("InsideHouse_ID") || args.Length < 1) return;

                        House house = Main.Houses.FirstOrDefault(h => h.ID == player.getData("InsideHouse_ID"));
                        if (house == null) return;

                        if (house.Owner != player.name)
                        {
                            player.sendNotification("Error", "~r~Apenas o dono pode fazer isto.");
                            return;
                        }

                        int idx = Convert.ToInt32(args[0]);
                        if (idx < 0 || idx >= house.Weapons.Count) return;

                        HouseWeapon weapon = house.Weapons[idx];
                        house.Weapons.RemoveAt(idx);
                        house.Save();

                        player.giveWeapon(weapon.Hash, weapon.Ammo, true, false);
                        foreach (WeaponComponent comp in weapon.Components) player.setWeaponComponent(weapon.Hash, comp);
                        player.setWeaponTint(weapon.Hash, weapon.Tint);

                        player.sendNotification("Success", string.Format("~g~Retirou um(a) {0} com {1:n0} balas.", weapon.Hash, weapon.Ammo));

                        player.triggerEvent("HouseUpdateWeapons", API.toJson(house.Weapons));
                        break;
                    }
            }
        }
        #endregion
    }
}
