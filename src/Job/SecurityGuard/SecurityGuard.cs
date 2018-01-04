using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Flags;
using pcrpg.src.Player.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pcrpg.src.Job.SecurityGuard
{
    class SecurityGuard : Script
    {
        Dictionary<Client, Vehicle> PlayerVehicle = new Dictionary<Client, Vehicle>();
        Dictionary<Client, int> PlayerJobProgress = new Dictionary<Client, int>();

        public SecurityGuard()
        {
            API.onPlayerDisconnected += OnPlayerDisconnected;
            API.onClientEventTrigger += OnClientEvent;
        }

        private void OnPlayerDisconnected(Client player, string reason)
        {
            if (PlayerVehicle.ContainsKey(player))
            {
                if (API.doesEntityExist(PlayerVehicle[player]))
                    API.deleteEntity(PlayerVehicle[player]);

                PlayerVehicle.Remove(player);
            }

            if (PlayerJobProgress.ContainsKey(player))
                PlayerJobProgress.Remove(player);

            if (player.hasData("SECURITY_JOB_OBJ"))
            {
                if (API.doesEntityExist(player.getData("SECURITY_JOB_OBJ")))
                {
                    API.deleteEntity(player.getData("SECURITY_JOB_OBJ"));
                }
            }
        }

        private void OnClientEvent(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "JobService":
                    {
                        if (!sender.hasData("JobMarker_ID")) return;

                        Job job = Main.Jobs.FirstOrDefault(h => h.ID == sender.getData("JobMarker_ID"));
                        if (job == null) return;

                        string serviceName = (string)arguments[0];
                        if (serviceName == "Transporte de dinheiro")
                        {
                            if (job.Vehicles.Count < 1)
                            {
                                sender.sendNotification("ERROR", "Estamos sem veículos no momento.");
                                return;
                            }

                            var atms = Gameplay.Bank.Main.Banks.Where(x => x.Type != 0).ToList();
                            if (atms.Count < 1)
                            {
                                sender.sendNotification("ERROR", "Todos os caixas estão vazios.");
                                return;
                            }

                            if (PlayerVehicle.ContainsKey(sender))
                            {
                                if (API.doesEntityExist(PlayerVehicle[sender]))
                                    API.deleteEntity(PlayerVehicle[sender]);

                                PlayerVehicle.Remove(sender);
                            }

                            Random random = new Random();
                            int i = random.Next(job.Vehicles.Count);
                            var vehicle = API.createVehicle(job.Vehicles[i].Model, job.Vehicles[i].Position, job.Vehicles[i].Rotation, job.Vehicles[i].PrimaryColor, job.Vehicles[i].SecondaryColor);
                            vehicle.engineStatus = false;
                            PlayerVehicle.Add(sender, vehicle);
                            PlayerJobProgress[sender] = 0;

                            sender.sendNotification("", "Pegue o caminhão e vá coletar o dinheiro");
                            sender.triggerEvent("SecurityGuardJobStart", vehicle, atms[random.Next(atms.Count)].Position);
                        }
                        break;
                    }
                case "SecurityGuardGetNextCheckpoint":
                    {
                        PlayerJobProgress[sender]++;
                        if (PlayerJobProgress[sender] < 5)
                        {
                            Random random = new Random();
                            var atms = Gameplay.Bank.Main.Banks.Where(x => x.Type != 0).ToList();
                            sender.sendNotification("", "Vá para o próximo endereço");
                            sender.triggerEvent("SecurityGuardGetNextCheckpoint", atms[random.Next(atms.Count)].Position);
                        }
                        else if (PlayerJobProgress[sender] == 5)
                        {
                            List<Vector3> positions = new List<Vector3>()
                            {
                                new Vector3(-143.3035, -822.2595, 31.32913),
                                new Vector3(-46.75024, -764.0831, 32.82755)
                            };

                            Random rand = new Random();
                            sender.triggerEvent("SecurityGuardGetNextCheckpoint", positions[rand.Next(positions.Count)], true);
                            sender.sendNotification("", "Vá até o banco entregar o dinheiro.");
                        }
                        else
                        {
                            int payment = 2500;
                            sender.sendNotification("", $"Você finalizou o serviço e recebeu ~g~$~w~{payment}.");
                            sender.giveMoney(payment);
                            sender.triggerEvent("SecurityGuardDeleteEntities");
                        }
                        break;
                    }
                case "SecurityGuardCarryAnimPlay":
                    {
                        var obj = API.createObject(289396019, sender.position, new Vector3(0, 0, 0));
                        API.attachEntityToEntity(obj, sender, "SKEL_L_Hand", new Vector3(0.2, 0.05, 0.05), new Vector3(0, 0, 0));
                        sender.setData("SECURITY_JOB_OBJ", obj);
                        sender.playAnimation("anim@heists@box_carry@", "idle", (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl));
                        break;
                    }
                case "SecurityGuardCarryAnimStop":
                    {
                        if (!sender.hasData("SECURITY_JOB_OBJ")) return;

                        var obj = sender.getData("SECURITY_JOB_OBJ");
                        API.deleteEntity(obj);
                        sender.resetData("SECURITY_JOB_OBJ");
                        sender.stopAnimation();
                        break;
                    }
            }
        }
    }
}
