﻿/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var player = null;
var browser = null;
var canMove = false;

API.onResourceStart.connect(() =>
{
    // Login camera position
    API.setEntityPosition(API.getLocalPlayer(), new Vector3(-498.3053, 4348.135, 66.42624));

    var startCam = API.createCamera(new Vector3(-734.4893, 4441.397, 15.45909), new Vector3());
    API.pointCameraAtPosition(startCam, new Vector3(-625.5039, 4449.758, 18.23818));

    var endCam = API.createCamera(new Vector3(-625.5039, 4447.758, 18.23818), new Vector3());
    API.pointCameraAtPosition(endCam, new Vector3(-528.4624, 4419.629, 30.04474));

    API.interpolateCameras(startCam, endCam, 30000, false, false);

    API.setHudVisible(false);
    API.setChatVisible(false);
    resource.CharacterCreator.ResetCharacterCreation();
    API.startMusic("res/sounds/music01.mp3", true);    
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    if (eventName == "ShowLoginForm")
    {
        player = args[0];
        if (browser == null)
        {
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
    else if (eventName == "ShowCharacterSelection")
    {
        if (browser != null)
        {
            API.destroyCefBrowser(browser);
            browser = null;
        }        

        resource.CharacterSelector.ShowCharacterSelector();
    }
    else if (eventName == "LoginError")
    {
        if (browser != null)
        {            
            browser.call("ShowError", args[0]);
        }
    }
});

API.onResourceStop.connect(() =>
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});

API.onUpdate.connect(() =>
{
    if (!canMove)
    {
        API.disableAllControlsThisFrame();
    }
});

function stopMusic()
{
    API.stopMusic();
}

function Login(username: string, password: string)
{
    resource.Sounds.PlaySelectSound();
    API.triggerServerEvent("LoginAttempt", username, password);
}

function Register(username: string, password: string, email: string)
{
    resource.Sounds.PlaySelectSound();
    API.triggerServerEvent("RegisterAttempt", username, password, email);
}

function LoginBrowserReady()
{
    browser.call("update", player);
}