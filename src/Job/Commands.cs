using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using pcrpg.src.Admin;
using System;
using System.IO;
using System.Linq;

namespace pcrpg.src.Job
{
    class Commands : Script
    {
        [Command("jobcmds", Alias = "jcmds")]
        public void CommandsCommand(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Emprego ~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /createjob - /deletejob - /setjobtype - /addjobvehspawn - /clearjobvehspawn");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Emprego ~~~~~~~~~~~~~~~~~");
        }

        [Command("createjob")]
        public void CMD_CreateJob(Client player, JobType type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (type < 0 || (int)type > Enum.GetNames(typeof(JobType)).Length - 1)
            {
                API.sendChatMessageToPlayer(player, $"~r~ERRO: ~s~Tipo inválido, use valores entre 0 e {Enum.GetNames(typeof(JobType)).Length - 1}.");
                return;
            }

            Job new_job = new Job(Main.GetGuid(), player.position, type);
            new_job.Save();

            Main.Jobs.Add(new_job);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você criou o emprego de ${new_job.Name}.");
        }

        [Command("deletejob")]
        public void CMD_RemoveJob(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("JobMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima do checkpoint do emprego que deseja excluir.");
                return;
            }

            Job job = Main.Jobs.FirstOrDefault(h => h.ID == player.getData("JobMarker_ID"));
            if (job == null) return;

            job.Destroy();
            Main.Jobs.Remove(job);

            string job_file = Main.JOB_SAVE_DIR + Path.DirectorySeparatorChar + job.ID + ".json";
            if (File.Exists(job_file)) File.Delete(job_file);

            player.resetData("JobMarker_ID");
            player.triggerEvent("ShowJobText", 0);

            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você deletou este emprego.");
        }

        [Command("setjobtype")]
        public void CMD_SetJobType(Client player, JobType new_type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("JobMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima do checkpoint do emprego que deseja editar.");
                return;
            }

            Job job = Main.Jobs.FirstOrDefault(h => h.ID == player.getData("JobMarker_ID"));
            if (job == null) return;

            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você alterou o tipo do emprego {job.Name} para {Main.GetJobName(new_type)}.");

            job.SetType(new_type);
            job.Save();
        }

        [Command("addjobvehspawn")]
        public void CMD_AddJobVehSpawn(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.isInVehicle)
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não está em um veículo.");
                return;
            }

            Job job = null;
            float distance = 40f;
            foreach (var j in Main.Jobs)
            {
                if (j.Position.DistanceTo(player.position) < distance)
                {
                    job = j;
                    distance = j.Position.DistanceTo(player.position);
                }
            }

            if (job == null)
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não está próximo de um emprego.");
                return;
            }

            JobVehicle vehicle = new JobVehicle(player.vehicle.model, player.vehicle.primaryColor, player.vehicle.secondaryColor, player.vehicle.position, player.vehicle.rotation);
            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você adicionou um spawn de veículo para o emprego {job.Name}.");
            job.Vehicles.Add(vehicle);
            job.Save();
        }

        [Command("clearjobvehspawn")]
        public void CMD_ClearJobVehicles(Client player, JobType new_type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("JobMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima do checkpoint do emprego que deseja editar.");
                return;
            }

            Job job = Main.Jobs.FirstOrDefault(h => h.ID == player.getData("JobMarker_ID"));
            if (job == null) return;

            API.sendChatMessageToPlayer(player, $"~g~SUCESSO: ~s~Você removeu os veículos do emprego {job.Name}.");

            job.Vehicles.Clear();
            job.Save();
        }
    }
}
