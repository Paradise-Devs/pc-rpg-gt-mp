using GrandTheftMultiplayer.Server.Elements;

namespace pcrpg.src.Admin
{
    public static class ClientExtensions
    {
        public static bool IsAdmin(this Client player)
        {
            if (player.hasData("User"))
            {
                if (player.getData("User").Admin > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
