using GrandTheftMultiplayer.Server.API;

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
        }
    }
}
