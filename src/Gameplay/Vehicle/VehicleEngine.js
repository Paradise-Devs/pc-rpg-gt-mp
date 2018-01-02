"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
API.onKeyDown.connect(function (sender, arg) {
    if (arg.KeyCode == Keys.Y && !API.isChatOpen() && API.getPlayerVehicleSeat(API.getLocalPlayer()) == -1) {
        API.triggerServerEvent("on_player_toggle_engine");
    }
});
