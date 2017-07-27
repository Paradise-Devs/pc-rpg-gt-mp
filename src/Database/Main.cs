using GrandTheftMultiplayer.Server.API;
using pcrpg.src.Database.Models;

namespace pcrpg.src.Database
{
    class Main : Script
    {
        public Main()
        {
            ContextFactory.SetConnectionParameters("127.0.0.1", "root", "root", "pcrpg");
        }
    }
}
