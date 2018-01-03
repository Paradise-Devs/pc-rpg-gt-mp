using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;
using pcrpg.src.Player.Utils;

namespace pcrpg.src.Job
{
    public class Main : Script
    {
        // settings
        public static string JOB_SAVE_DIR = "data/Job";
        public static int SAVE_INTERVAL = 120;

        public static List<Job> Jobs = new List<Job>();

        public Main()
        {
            API.onResourceStart += Job_Init;
            API.onClientEventTrigger += Job_ClientEvent;
            API.onResourceStop += Job_Exit;
        }

        #region Methods
        public static Guid GetGuid()
        {
            Guid new_guid;

            do
            {
                new_guid = Guid.NewGuid();
            } while (Jobs.Count(h => h.ID == new_guid) > 0);

            return new_guid;
        }

        public static string GetJobName(JobType job)
        {
            switch (job)
            {
                case JobType.SecurityGuard:
                    return "Segurança de carro forte";
                default:
                    return "Desempregado";
            }
        }
        #endregion

        #region Events
        public void Job_Init()
        {
            // load settings
            if (API.hasSetting("jobDirName")) JOB_SAVE_DIR = API.getSetting<string>("jobDirName");

            JOB_SAVE_DIR = API.getResourceFolder() + Path.DirectorySeparatorChar + JOB_SAVE_DIR;
            if (!Directory.Exists(JOB_SAVE_DIR)) Directory.CreateDirectory(JOB_SAVE_DIR);

            if (API.hasSetting("saveInterval")) SAVE_INTERVAL = API.getSetting<int>("saveInterval");

            API.consoleOutput("-> Save Interval: {0}", TimeSpan.FromSeconds(SAVE_INTERVAL).ToString(@"hh\:mm\:ss"));

            // load jobs
            foreach (string file in Directory.EnumerateFiles(JOB_SAVE_DIR, "*.json"))
            {
                Job job = JsonConvert.DeserializeObject<Job>(File.ReadAllText(file));
                Jobs.Add(job);
            }

            API.consoleOutput("Loaded {0} jobs.", Jobs.Count);
        }        

        public void Job_ClientEvent(Client player, string event_name, params object[] args)
        {
            switch (event_name)
            {
                case "JobInteract":
                    {
                        if (!player.hasData("JobMarker_ID")) return;

                        Job job = Jobs.FirstOrDefault(h => h.ID == player.getData("JobMarker_ID"));
                        if (job == null) return;

                        if (player.getJob() == (int)job.Type)
                        {
                            var services = new List<string>();
                            switch (job.Type)
                            {
                                case JobType.SecurityGuard:
                                    services.Add("Transporte de dinheiro");
                                    break;
                            }
                            player.triggerEvent("JobMenu", API.toJson(services));
                        }
                        else if (player.getJob() == null)
                        {
                            player.setJob((int)job.Type);
                            player.sendNotification("", $"Você se tornou um {job.Name}!");
                        }
                        else
                            player.sendNotification("ERROR", "Você já possui um emprego.");
                        break;
                    }
                case "JobQuit":
                    {
                        if (player.getJob() == null)
                        {
                            player.sendNotification("ERROR", "Você não possui um emprego.");
                            return;
                        }

                        player.setJob(null);
                        player.sendNotification("SUCCESS", "Você abandonou seu emprego.");
                        break;
                    }
            }
        }

        public void Job_Exit()
        {
            foreach (Job job in Jobs)
            {
                job.Save(true);
                job.Destroy();
            }

            Jobs.Clear();
        }
        #endregion
    }
}
