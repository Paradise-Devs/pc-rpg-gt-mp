"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
var invBrowser = null;
API.onResourceStart.connect(() => {
    var res = API.getScreenResolution();
    invBrowser = API.createCefBrowser(res.Width, res.Height);
    API.waitUntilCefBrowserInit(invBrowser);
    API.setCefBrowserPosition(invBrowser, 0, 0);
    API.loadPageCefBrowser(invBrowser, "res/views/inventory.html");
    API.setCefBrowserHeadless(invBrowser, true);
});
API.onResourceStop.connect(() => {
    if (invBrowser != null) {
        API.destroyCefBrowser(invBrowser);
        invBrowser = null;
    }
});
API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode === Keys.I) {
        if (API.getCefBrowserHeadless(invBrowser)) {
            API.triggerServerEvent("GetCharacterItems");
        }
        else {
            API.showCursor(false);
            API.startAudio("res/sounds/inventory/close.wav", false);
            API.setCefBrowserHeadless(invBrowser, true);
        }
    }
});
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "UpdateCharacterItems") {
        if (invBrowser == null)
            return;
        API.showCursor(true);
        API.startAudio("res/sounds/inventory/open.wav", false);
        API.setCefBrowserHeadless(invBrowser, false);
        invBrowser.call("DrawItems", args[0], args[1]);
    }
    else if (eventName == "OnItemDiscarded") {
        invBrowser.call("OnItemDiscarded", args[0]);
    }
});
function onUseItem(id) {
    API.triggerServerEvent("UseItem", id);
}
function onDiscardItem(id) {
    API.triggerServerEvent("DiscardItem", id);
}
