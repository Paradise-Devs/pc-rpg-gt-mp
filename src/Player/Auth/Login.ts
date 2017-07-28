/// <reference path='../../../types-gt-mp/index.d.ts' />

var player = null;
var browser = null;
var canMove = false;

API.onResourceStart.connect(() =>
{
    // Login camera position
    var newCam = API.createCamera(new Vector3(-35.43801, -1122.411, 270.5569), new Vector3());
    API.pointCameraAtPosition(newCam, new Vector3(-80.07943, -840.8312, 310.4772));
    API.setActiveCamera(newCam);

    API.setHudVisible(false);
    API.setChatVisible(false);
    resource.CharacterCreator.ResetCharacterCreation();
    API.startMusic("res/sounds/music01.ogg", true);
    API.setMusicVolume(0.25);    
});

API.onServerEventTrigger.connect((name: string, args: any[]) =>
{
    if (name == "ShowLoginForm")
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
    else if (name == "ShowCharacterSelection")
    {
        if (browser != null)
        {
            API.destroyCefBrowser(browser);
            browser = null;
        }        

        resource.CharacterSelector.ShowCharacterSelector();
    }
    else if (name == "LoginError")
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