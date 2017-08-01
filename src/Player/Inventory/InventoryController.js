"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var browser = null;
API.onResourceStart.connect(() => {
    if (browser == null) {
        var res = API.getScreenResolution();
        browser = API.createCefBrowser(res.Width, res.Height);
        API.setCefBrowserHeadless(browser, true);
        API.setCefBrowserPosition(browser, 0, 0);
        API.loadPageCefBrowser(browser, "res/views/inventory.html");
    }
});
API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode === Keys.I) {
        if (browser == null)
            return;
        if (API.getCefBrowserHeadless(browser)) {
            API.triggerServerEvent("GetCharacterItems");
        }
        else {
            API.showCursor(false);
            API.setCefBrowserHeadless(browser, true);
        }
    }
});
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "UpdateCharacterItems") {
        if (browser == null)
            return;
        API.showCursor(true);
        API.setCefBrowserHeadless(browser, false);
        browser.call("DrawItems", args[0], args[1]);
    }
    else if (eventName == "OnItemDiscarded") {
        browser.call("OnItemDiscarded", args[0]);
    }
});
function onUseItem(id) {
    API.triggerServerEvent("UseItem", id);
}
function onDiscardItem(id) {
    API.triggerServerEvent("DiscardItem", id);
}
