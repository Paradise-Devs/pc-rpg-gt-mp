"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var player = null;
var browser = null;
var music = null;
API.onResourceStart.connect(() => {
    // Login camera position
    var newCam = API.createCamera(new Vector3(-35.43801, -1122.411, 270.5569), new Vector3());
    API.pointCameraAtPosition(newCam, new Vector3(-80.07943, -840.8312, 310.4772));
    API.setActiveCamera(newCam);
    API.setHudVisible(false);
    API.setChatVisible(false);
    resource.CharacterCreator.ResetCharacterCreation();
    music = API.startMusic("res/sounds/music01.ogg", true);
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
        // Move scene position
        API.setEntityPosition(API.getLocalPlayer(), new Vector3(402.9198, -996.5348, -99.00024));
        API.setEntityRotation(API.getLocalPlayer(), new Vector3(0.0, 0.0, 176.8912));
        var startCamPos = new Vector3(400.9627, -1005.109, -99.00404);
        var startCamRot = new Vector3(0.0, 0.0, 176.891);
        var startCamera = API.createCamera(startCamPos, startCamRot);
        var endCamPos = new Vector3(403.6378, -998.5422, -99.00404);
        var endCamRot = new Vector3(0.0, 0.0, 176.891);
        var endCamera = API.createCamera(endCamPos, endCamRot);
        API.pointCameraAtPosition(endCamera, new Vector3(402.9198, -996.5348, -99.00024));
        API.interpolateCameras(startCamera, endCamera, 5000, false, false);
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
    resource.Sounds.PlaySelectSound();
    API.triggerServerEvent("LoginAttempt", username, password);
}
function Register(username, password, email) {
    resource.Sounds.PlaySelectSound();
    API.triggerServerEvent("RegisterAttempt", username, password, email);
}
function LoginBrowserReady() {
    browser.call("update", player);
}
