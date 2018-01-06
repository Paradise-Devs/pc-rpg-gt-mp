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

        Weapon_Pistol = 50,
        Weapon_Pistol50,
        Weapon_Revolver,
        Weapon_MicroSMG,
        Weapon_SMG,
        Weapon_Gusenberg,
        Weapon_AssaultRifle,
        Weapon_BullpupRifle,
        Weapon_SniperRifle,
        Weapon_PumpShotgun,
        Weapon_SawnOffShotgun,
        Weapon_Material
    }

    public class ItemDefinitions
    {
        public static Dictionary<ItemID, BaseItem> ItemDictionary = new Dictionary<ItemID, BaseItem>
        {
            { ItemID.Armor_Tier1, new ArmorItem("Colete Super Leve", "Aumenta seu colete em 20%", 3, 20, "common", new WorldModel("prop_armour_pickup", new Vector3(0.0, 0.0, 0.1), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier2, new ArmorItem("Colete Leve", "Aumenta seu colete em 40%", 2, 40, "common", new WorldModel("prop_bodyarmour_02", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier3, new ArmorItem("Colete Padrão", "Aumenta seu colete em 60%", 2, 60, "uncommon", new WorldModel("prop_bodyarmour_03", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier4, new ArmorItem("Colete Pesado", "Aumenta seu colete em 80%", 1, 80, "uncommon", new WorldModel("prop_bodyarmour_04", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },
            { ItemID.Armor_Tier5, new ArmorItem("Colete Super Pesado", "Aumenta seu colete em 100%", 1, 100, "rare", new WorldModel("prop_bodyarmour_05", new Vector3(0.0, 0.0, 0.125), new Vector3(-90.0, 0.0, 0.0))) },

            { ItemID.Weapon_Pistol, new WeaponItem("Pistola", "Uma pistola compacta, leve e semiautomática", 1, WeaponHash.Pistol, "common", new WorldModel("prop_box_guncase_01a", new Vector3(0.0, 0.0, 0.1), new Vector3())) },
            { ItemID.Weapon_Pistol50, new WeaponItem("Pistola 50mm", "Uma pistola compacta, leve e semiautomática", 1, WeaponHash.Pistol50, "common", new WorldModel("prop_box_guncase_01a", new Vector3(0.0, 0.0, 0.1), new Vector3())) },
            { ItemID.Weapon_Revolver, new WeaponItem("Revolver", "Pistola de repetição", 1, WeaponHash.Revolver, "uncommon", new WorldModel("prop_box_guncase_01a", new Vector3(0.0, 0.0, 0.1), new Vector3())) },
            { ItemID.Weapon_MicroSMG, new WeaponItem("MicroSMG", "Uma metralhadora compacta, leve e semiautomática", 1, WeaponHash.MicroSMG, "common", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_SMG, new WeaponItem("SMG", "Uma metralhadora compacta, leve e semiautomática", 1, WeaponHash.SMG, "uncommon", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_Gusenberg, new WeaponItem("Gusenberg", "Uma metralhadora compacta, leve e semiautomática", 1, WeaponHash.Gusenberg, "uncommon", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_AssaultRifle, new WeaponItem("Assault Rifle", "Uma metralhadora de calibre pesado", 1, WeaponHash.AssaultRifle, "rare", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_BullpupRifle, new WeaponItem("Bullpup Rifle", "Uma metralhadora de calibre pesado", 1, WeaponHash.BullpupRifle, "rare", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_SniperRifle, new WeaponItem("Sniper Rifle", "Rifle de francotirador ideal para situações que requerem precisão a longa distância", 1, WeaponHash.SniperRifle, "legendary", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_PumpShotgun, new WeaponItem("Pump Shotgun", "Escopeta ideal para combate de curto alcance", 1, WeaponHash.PumpShotgun, "common", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },
            { ItemID.Weapon_SawnOffShotgun, new WeaponItem("SawnOff Shotgun", "Escopeta ideal para combate de curto alcance", 1, WeaponHash.SawnoffShotgun, "uncommon", new WorldModel("prop_gun_case_01", new Vector3(0.0, 0.0, 0.15), new Vector3())) },

            { ItemID.Weapon_Material, new MaterialItem("Materiais", "Usado para criar armas.", 999, "common", new WorldModel("prop_cs_cardbox_01", new Vector3(0.0, 0.0, 0.125), new Vector3())) }
        };
    }
}
