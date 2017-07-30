"use strict";
/// <reference path='../../types-gt-mp/index.d.ts' />
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "PlayerWakeUpForTheFirstTime") {
        API.setActiveCamera(null);
        API.setCanOpenChat(true);
        API.setChatVisible(true);
        API.setHudVisible(true);
        API.sendNotification("Vá falar com chester");
    }
});
