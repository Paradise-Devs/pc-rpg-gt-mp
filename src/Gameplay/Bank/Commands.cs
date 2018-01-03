using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using pcrpg.src.Admin;
using System.IO;
using System.Linq;

namespace pcrpg.src.Gameplay.Bank
{
    class Commands : Script
    {
        [Command("bankcmds", Alias = "bcmds")]
        public void CommandsCommand(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Banco ~~~~~~~~~~~~~~~~~");
            API.sendChatMessageToPlayer(player, "* /createbank - /deletebank - /setbanktype");
            API.sendChatMessageToPlayer(player, "~p~~~~~~~~~~~~~~~~~~ Comandos Banco ~~~~~~~~~~~~~~~~~");
        }

        [Command("createbank")]
        public void CMD_CreateBank(Client player, int type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }
            
            else if (type < 0 || type > 1)
            {
                player.sendChatMessage("~r~ERRO: ~w~Use valores entre 0 e 1 para o tipo.");
                return;
            }

            Bank new_bank = new Bank(Main.GetGuid(), player.position, type);
            new_bank.Save();

            Main.Banks.Add(new_bank);
            API.sendChatMessageToPlayer(player, (type == 0) ? "~g~SUCESSO: ~s~Você adicionou um banco em sua posição." : "~g~SUCESSO: ~s~Você adicionou um caixa eletrônico em sua posição.");
        }

        [Command("deletebank")]
        public void CMD_RemoveBank(Client player)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("BankMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima da posição do banco que deseja excluir.");
                return;
            }

            Bank bank = Main.Banks.FirstOrDefault(h => h.ID == player.getData("BankMarker_ID"));
            if (bank == null) return;

            bank.Destroy();
            Main.Banks.Remove(bank);

            string bank_file = Main.BANK_SAVE_DIR + Path.DirectorySeparatorChar + bank.ID + ".json";
            if (File.Exists(bank_file)) File.Delete(bank_file);
            API.sendChatMessageToPlayer(player, (bank.Type == 0) ? "~g~SUCESSO: ~s~Você deletou este banco." : "~g~SUCESSO: ~s~Você deletou este caixa eletrônico.");
        }

        [Command("setbanktype")]
        public void CMD_SetBankType(Client player, int new_type)
        {
            if (!player.IsAdmin())
            {
                player.sendChatMessage("~r~ERRO: ~w~Você não tem permissão.");
                return;
            }

            if (!player.hasData("BankMarker_ID"))
            {
                player.sendChatMessage("~r~ERRO: ~w~Fique em cima da posição do banco que deseja editar.");
                return;
            }

            Bank bank = Main.Banks.FirstOrDefault(h => h.ID == player.getData("BankMarker_ID"));
            if (bank == null) return;

            bank.SetType(new_type);
            bank.Save();

            API.sendChatMessageToPlayer(player, (new_type == 0) ? "~g~SUCESSO: ~s~Você alterou o tipo para banco." : "~g~SUCESSO: ~s~Você alterou o tipo para caixa eletrônico.");
        }
    }
}