using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using pcrpg.src.Player.Inventory.Classes;
using pcrpg.src.Player.Inventory.Extensions;
using pcrpg.src.Player.Utils;
using System;

namespace pcrpg.src.Job.ArmsDealer
{
    class ArmsDealer : Script
    {
        public ArmsDealer()
        {
            API.onClientEventTrigger += OnClientEvent;
        }

        private void OnClientEvent(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "JobService":
                    {
                        string serviceName = (string)arguments[0];
                        if (serviceName == "Obter materiais")
                        {
                            sender.triggerEvent("ArmsDealer_SpawnMaterial");
                        }
                        else if (serviceName == "Vender materiais")
                        {
                            if (!sender.hasItem(ItemID.Weapon_Material))
                            {
                                sender.sendNotification("", "Você não possui materiais.");
                                return;
                            }

                            int amount = sender.getItemAmount(ItemID.Weapon_Material);
                            int cost = 3 * amount;

                            sender.giveMoney(cost);
                            sender.removeItem(sender.getItem(ItemID.Weapon_Material), amount);

                            sender.sendNotification("", $"- {amount} materiais");
                            sender.sendNotification("", $"+ ~g~$~w~{cost}");
                        }
                        else if (serviceName == "Criar arma")
                        {
                            sender.triggerEvent("ShowWeaponCraftMenu");
                        }
                        break;
                    }
                case "ArmsDealer_TakeMaterial":
                    {
                        int amount = 2;

                        Random random = new Random();
                        amount += random.Next(0, 3);

                        sender.giveItem(ItemID.Weapon_Material, amount);
                        sender.sendNotification("", $"+ {amount} materiais");
                        break;
                    }
                case "CREATE_WEAPON":
                    {
                        WeaponHash weapon = (WeaponHash)arguments[0];
                        int cost = (int)arguments[1];

                        if (!sender.hasItem(ItemID.Weapon_Material))
                        {
                            sender.sendNotification("", "Você não possui materiais.");
                            return;
                        }
                        else if (sender.getItemAmount(ItemID.Weapon_Material) < cost)
                        {
                            sender.sendNotification("", "~r~ERRO: ~w~Você não tem materiais suficiente.");
                            return;
                        }

                        switch (weapon)
                        {
                            case WeaponHash.Pistol:
                                sender.giveItem(ItemID.Weapon_Pistol, 1);
                                break;
                            case WeaponHash.Pistol50:
                                sender.giveItem(ItemID.Weapon_Pistol50, 1);
                                break;
                            case WeaponHash.Revolver:
                                sender.giveItem(ItemID.Weapon_Revolver, 1);
                                break;
                            case WeaponHash.MicroSMG:
                                sender.giveItem(ItemID.Weapon_MicroSMG, 1);
                                break;
                            case WeaponHash.SMG:
                                sender.giveItem(ItemID.Weapon_SMG, 1);
                                break;
                            case WeaponHash.Gusenberg:
                                sender.giveItem(ItemID.Weapon_Gusenberg, 1);
                                break;
                            case WeaponHash.AssaultRifle:
                                sender.giveItem(ItemID.Weapon_AssaultRifle, 1);
                                break;
                            case WeaponHash.BullpupRifle:
                                sender.giveItem(ItemID.Weapon_BullpupRifle, 1);
                                break;
                            case WeaponHash.SniperRifle:
                                sender.giveItem(ItemID.Weapon_SniperRifle, 1);
                                break;
                            case WeaponHash.PumpShotgun:
                                sender.giveItem(ItemID.Weapon_PumpShotgun, 1);
                                break;
                            case WeaponHash.SawnoffShotgun:
                                sender.giveItem(ItemID.Weapon_SawnOffShotgun, 1);
                                break;
                        }

                        int idx = sender.getItem(ItemID.Weapon_Material);
                        sender.removeItem(idx, cost);

                        sender.sendNotification("", $"- {cost} materiais");
                        sender.sendNotification("", $"+ 1 {weapon}");
                        break;
                    }
            }
        }
    }
}
