"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var player = null;
var browser = null;
API.onResourceStart.connect(() => {
    // Login camera position
    var newCam = API.createCamera(new Vector3(-35.43801, -1122.411, 270.5569), new Vector3());
    API.pointCameraAtPosition(newCam, new Vector3(-80.07943, -840.8312, 310.4772));
    API.setActiveCamera(newCam);
    API.setHudVisible(false);
});
API.onServerEventTrigger.connect((name, args) => {
    if (name == "ShowLoginForm") {
        player = args[0];
        if (browser == null) {
            var res = API.getScreenResolution();
            browser = API.createCefBrowser(res.Width, res.Height);
            API.setCefBrowserPosition(browser, 0, 0);
            API.waitUntilCefBrowserInit(browser);
            API.loadPageCefBrowser(browser, "res/views/login.html");
            API.waitUntilCefBrowserLoaded(browser);
        }
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.setCefBrowserHeadless(browser, false);
    }
    else if (name == "ShowCharacterSelection") {
        if (browser != null) {
            API.destroyCefBrowser(browser);
            browser = null;
        }
        // Move camera position        
        var _camPos = new Vector3(113.7806, -646.5242, 355.2048);
        var _camLookAtPos = new Vector3(-35.43801, -1122.411, 270.5569);
        var _cam = API.createCamera(_camPos, new Vector3());
        API.pointCameraAtPosition(_cam, _camLookAtPos);
        API.interpolateCameras(API.getActiveCamera(), _cam, 15000, false, false);
        resource.CharacterSelector.ShowCharacterSelector();
    }
    else if (name == "LoginError") {
        if (browser != null) {
            browser.call("ShowError", args[0]);
        }
    }
});
API.onResourceStop.connect(() => {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});
function Login(username, password) {
    API.triggerServerEvent("LoginAttempt", username, password);
}
function Register(username, password, email) {
    API.triggerServerEvent("RegisterAttempt", username, password, email);
}
function browserReady() {
    browser.call("update", player);
}
