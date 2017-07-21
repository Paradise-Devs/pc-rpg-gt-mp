﻿using System.Linq;
using pcrpg.Database.Models;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;

namespace pcrpg.src.Player.Auth
{
    class Login : Script
    {
        public Login()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
            API.onPlayerFinishedDownload += OnPlayerDownloaded;
        }

        private void OnPlayerDownloaded(Client player)
        {
            var user = ContextFactory.Instance.Users.FirstOrDefault(up => up.SocialClubName == player.socialClubName);
            if (user != null)
            {
                user.LastIp = player.address;
                API.setEntityData(player, "User", user);
                API.triggerClientEvent(player, "ShowCharacterSelection");

                ContextFactory.Instance.SaveChanges();
            }
            else
            {
                API.triggerClientEvent(player, "ShowLoginForm");
            }
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "LoginAttempt")
            {
                string username = (string)arguments[0];
                string password = (string)arguments[1];

                var user = ContextFactory.Instance.Users.FirstOrDefault(up => up.Username == username && up.Password == password);
                if (user != null)
                {
                    user.SocialClubName = sender.socialClubName;
                    API.setEntityData(sender, "User", user);
                    API.triggerClientEvent(sender, "ShowCharacterSelection");
                }
                else
                {
                    API.triggerClientEvent(sender, "LoginError", "Usuário ou senha incorreta.");
                }
            }
            else if (eventName == "RegisterAttempt")
            {
                string username = (string)arguments[0];
                string password = (string)arguments[1];
                string emailadd = (string)arguments[2];

                var user = ContextFactory.Instance.Users.FirstOrDefault(up => up.Username == username);
                if (user == null)
                {
                    user = new Users { Username = username, Password = password, Email = emailadd, SocialClubName = sender.socialClubName, LastIp = sender.address };
                    ContextFactory.Instance.Users.Add(user);

                    API.setEntityData(sender, "User", user);
                    API.triggerClientEvent(sender, "ShowCharacterSelection");

                    ContextFactory.Instance.SaveChanges();
                }
                else
                {
                    API.triggerClientEvent(sender, "LoginError", "Este usuário já está em uso.");
                }
            }
        }        
    }
}
