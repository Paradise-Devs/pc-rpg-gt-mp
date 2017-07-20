using GrandTheftMultiplayer.Server.API;
using pcrpg.Database.Models;

namespace pcrpg.Database
{
    class Main : Script
    {
        public Main()
        {
            ContextFactory.SetConnectionParameters("127.0.0.1", "root", "root", "pcrpg");
        }
    }
}
