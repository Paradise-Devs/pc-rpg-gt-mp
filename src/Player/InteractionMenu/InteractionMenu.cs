using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using pcrpg.src.Player.Utils;

namespace pcrpg.src.Player.InteractionMenu
{
    class InteractionMenu : Script
    {
        public InteractionMenu()
        {
            API.onClientEventTrigger += OnClientEvent;
        }

        private void OnClientEvent(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "InteractionMenu_GetWaypointPosition":
                    int index = (int)arguments[0];
                    Vector3 position = null;
                    switch (index)
                    {
                        case 1:
                            {
                                float distance = -1f;
                                foreach (var bank in Gameplay.Bank.Main.Banks)
                                {
                                    if (bank.Position.DistanceTo(sender.position) < distance || distance < 0f)
                                    {
                                        position = bank.Position;
                                        distance = bank.Position.DistanceTo(sender.position);
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                float distance = -1f;
                                foreach (var dealership in Gameplay.Dealership.Main.Dealerships)
                                {
                                    if (dealership.Position.DistanceTo(sender.position) < distance || distance < 0f)
                                    {
                                        position = dealership.Position;
                                        distance = dealership.Position.DistanceTo(sender.position);
                                    }
                                }
                                break;
                            }
                        case 3:
                            {
                                float distance = -1f;
                                foreach (var parkinglot in Gameplay.Parkinglot.Main.Parkinglots)
                                {
                                    if (parkinglot.Position.DistanceTo(sender.position) < distance || distance < 0f)
                                    {
                                        position = parkinglot.Position;
                                        distance = parkinglot.Position.DistanceTo(sender.position);
                                    }
                                }
                                break;
                            }
                    }

                    if (position != null) sender.triggerEvent("InteractionMenu_SetWaypointPosition", position.X, position.Y);
                    break;
                case "InteractionMenu_Suicide":
                    sender.sendChatAction("cometeu suicídio.");
                    sender.kill();
                    break;
            }
        }
    }
}
