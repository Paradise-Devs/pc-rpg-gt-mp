﻿using System;
using System.Linq;
using System.Text;
using pcrpg.src.Database.Models;
using System.Security.Cryptography;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System.Collections.Generic;
using System.Web.Script.Serialization;

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
            using (var ctx = new ContextFactory().Create())
            {
                var user = ctx.Users.FirstOrDefault(up => up.SocialClubName == player.socialClubName);
                if (user != null)
                {
                    user.LastIp = player.address;
                    API.setEntityData(player, "User", user);
                    API.triggerClientEvent(player, "ShowCharacterSelection");

                    ctx.SaveChanges();
                }
                else
                {
                    Dictionary<string, string> playerData = new Dictionary<string, string>()
                    {
                        { "name", player.name},
                        { "socialClub", player.socialClubName}
                    };                
                    API.triggerClientEvent(player, "ShowLoginForm", new JavaScriptSerializer().Serialize(playerData));
                }

                int dimension = Managers.DimensionManager.RequestPrivateDimension(player);
                API.setEntityDimension(player, dimension);
            }
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "LoginAttempt")
            {
                string username = (string)arguments[0];
                string password = Encrypt((string)arguments[1]);
                using (var ctx = new ContextFactory().Create())
                {
                    var user = ctx.Users.FirstOrDefault(up => up.Username == username && up.Password == password);
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
            }
            else if (eventName == "RegisterAttempt")
            {
                string username = (string)arguments[0];
                string password = Encrypt((string)arguments[1]);
                string emailadd = (string)arguments[2];

                using (var ctx = new ContextFactory().Create())
                {
                    var user = ctx.Users.FirstOrDefault(up => up.Username == username);
                    if (user == null)
                    {
                        user = new User { Username = username, Password = password, Email = emailadd, SocialClubName = sender.socialClubName, Admin = 0, LastIp = sender.address };
                        ctx.Users.Add(user);

                        API.setEntityData(sender, "User", user);
                        API.triggerClientEvent(sender, "ShowCharacterSelection");

                        ctx.SaveChanges();
                    }
                    else
                    {
                        API.triggerClientEvent(sender, "LoginError", "Este usuário já está em uso.");
                    }
                }
            }
        }

        public static String Encrypt(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(item => item.ToString("x2")));
            }
        }
    }
}
