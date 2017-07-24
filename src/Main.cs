using System.Timers;
using GrandTheftMultiplayer.Server.API;
using pcrpg.Database.Models;
using System.Linq;

namespace pcrpg
{
    public class Main : Script
    {
        public Main()
        {
            API.onResourceStart += OnResourceStart;
        }

        private void OnResourceStart()
        {
            API.setGamemodeName("pcrpg v0.2.0");

            Timer timer = new Timer(600000);
            timer.Elapsed += OnSaveChanges;
            timer.Enabled = true;

            ContextFactory.Instance.Characters.Where(up => up.Id == 1);
            ContextFactory.Instance.SaveChanges();
        }

        private void OnSaveChanges(object sender, ElapsedEventArgs e)
        {
            ContextFactory.Instance.SaveChanges();
        }
    }
}
