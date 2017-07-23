"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var browser = null;
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "UpdateCharactersList") {
        if (browser != null)
            browser.call("updateList", args[0]);
    }
});
API.onResourceStop.connect(() => {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});
function ShowCharacterSelector() {
    if (browser == null) {
        var res = API.getScreenResolution();
        browser = API.createCefBrowser(res.Width, res.Height);
        API.setCefBrowserHeadless(browser, true);
        API.setCefBrowserPosition(browser, 0, 0);
        API.waitUntilCefBrowserInit(browser);
        API.loadPageCefBrowser(browser, "res/views/character_selector.html");
        API.waitUntilCefBrowserLoaded(browser);
    }
    API.showCursor(true);
    API.setCanOpenChat(false);
    API.setCefBrowserHeadless(browser, false);
}
function browserReady() {
    API.triggerServerEvent("RetrieveCharactersList");
}
