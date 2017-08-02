/// <reference path='../../types-gt-mp/index.d.ts' />

API.onServerEventTrigger.connect((eventName: string, args: any[]) =>
{
    if (eventName == "PlayerWakeUpForTheFirstTime")
    {
        API.setActiveCamera(null);
        API.setCanOpenChat(true);
        API.setChatVisible(true);
        API.setHudVisible(true);

        API.sendNotification("Pressione ~r~O ~w~para ver suas quests");
    }
});