"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var browser = null;
API.onResourceStart.connect(() => {
    InitializeDialogBrowser();
});
function ShowNpcDialog(id, npc) {
    if (browser == null || !API.isCefBrowserInitialized(browser))
        InitializeDialogBrowser();
    API.showCursor(true);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(browser, false);
    browser.call("DrawDialog", id, npc);
}
function HideNpcDialog() {
    API.showCursor(false);
    API.setActiveCamera(null);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(browser, true);
}
API.onResourceStop.connect(() => {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});
function InitializeDialogBrowser() {
    var res = API.getScreenResolution();
    browser = API.createCefBrowser(res.Width, res.Height);
    API.setCefBrowserHeadless(browser, true);
    API.setCefBrowserPosition(browser, 0, 0);
    API.loadPageCefBrowser(browser, "res/views/dialog.html");
}
function SelectedAnswer(id, answerid) {
    API.triggerServerEvent("OnSelectAnswer", id, answerid);
}
