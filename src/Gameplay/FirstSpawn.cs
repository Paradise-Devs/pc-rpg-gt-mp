using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using pcrpg.src.Flags;

namespace pcrpg.src.Gameplay
{
    class FirstSpawn : Script
    {
        public FirstSpawn()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "SpawnPlayerForTheFirstTime")
            {
                API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop), "dead", "dead_d");
                API.delay(5000, true, () =>
                {
                    API.playPlayerAnimation(sender, 0, "get_up@standard", "front");
                    API.triggerClientEvent(sender, "PlayerWakeUpForTheFirstTime");
                });
            }
        }
    }
}
