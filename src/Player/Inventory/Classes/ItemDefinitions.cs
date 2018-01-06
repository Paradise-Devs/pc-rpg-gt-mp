using System.Collections.Generic;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Player.Inventory.ItemTypes;

namespace pcrpg.src.Player.Inventory.Classes
{
    public enum ItemID
    {
        Armor_Tier1 = 0,
        Armor_Tier2,
        Armor_Tier3,
        Armor_Tier4,
        Armor_Tier5,

        Weapon_CombatPistol = 10,
        Weapon_PumpShotgun,
        Weapon_CarbineRifle,
        Weapon_SniperRifle
    }

    public class ItemDefinitions
    {
        public static Dictionary<ItemID, BaseItem> ItemDictionary = new Dictionary<ItemID, BaseItem>
        {
            { ItemID.Armor_Tier1, new ArmorItem("Colete Super Leve", "Aumenta seu colete em 20%.", 3, 20, "common", new WorldModel("prop_armour_pickup", new Vector3(0.0, 0.0, 0.1), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier2, new ArmorItem("Colete Leve", "Aumenta seu colete em 40%.", 2, 40, "common", new WorldModel("prop_bodyarmour_02", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier3, new ArmorItem("Colete Padrão", "Aumenta seu colete em 60%.", 2, 60, "uncommon", new WorldModel("prop_bodyarmour_03", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier4, new ArmorItem("Colete Pesado", "Aumenta seu colete em 80%.", 1, 80, "uncommon", new WorldModel("prop_bodyarmour_04", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier5, new ArmorItem("Colete Super Pesado", "Aumenta seu colete em 100%.", 1, 100, "rare", new WorldModel("prop_bodyarmour_05", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },

            { ItemID.Weapon_CombatPistol, new WeaponItem("Combat Pistol", "A compact, lightweight, semi-automatic pistol.", 1, WeaponHash.CombatPistol, "common", new WorldModel("w_pi_combatpistol", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_PumpShotgun, new WeaponItem("Pump Shotgun", "Standard shotgun ideal for short-range combat.", 1, WeaponHash.PumpShotgun, "common", new WorldModel("w_sg_pumpshotgun", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_CarbineRifle, new WeaponItem("Carbine Rifle", "Combining long distance accuracy with a high-capacity magazine, the carbine rifle can be relied on to make the hit.", 1, WeaponHash.CarbineRifle, "uncommon", new WorldModel("w_ar_carbinerifle", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_SniperRifle, new WeaponItem("Sniper Rifle", "Standard sniper rifle. Ideal for situations that require accuracy at long range. Limitations include slow reload speed and very low rate of fire.", 1, WeaponHash.SniperRifle, "legendary", new WorldModel("w_sr_sniperrifle", new Vector3(0.0, 0.0, 0.15), new Vector3())) }
        };
    }
}
