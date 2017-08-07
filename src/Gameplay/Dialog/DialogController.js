"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var dlgBrowser = null;
API.onResourceStart.connect(() => {
    var res = API.getScreenResolution();
    dlgBrowser = API.createCefBrowser(res.Width, res.Height);
    API.waitUntilCefBrowserInit(dlgBrowser);
    API.setCefBrowserPosition(dlgBrowser, 0, 0);
    API.loadPageCefBrowser(dlgBrowser, "res/views/dialog.html");
    API.setCefBrowserHeadless(dlgBrowser, true);
});
function ShowNpcDialog(id, npc) {
    API.showCursor(true);
    API.setHudVisible(false);
    API.setCanOpenChat(false);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(dlgBrowser, false);
    dlgBrowser.call("DrawDialog", id, npc);
}
function HideNpcDialog() {
    API.showCursor(false);
    API.setHudVisible(true);
    API.setCanOpenChat(true);
    API.setActiveCamera(null);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(dlgBrowser, true);
}
API.onResourceStop.connect(() => {
    if (dlgBrowser != null) {
        API.destroyCefBrowser(dlgBrowser);
        dlgBrowser = null;
    }
});
function SelectedAnswer(id, answerid) {
    API.triggerServerEvent("OnSelectAnswer", id, answerid);
}
