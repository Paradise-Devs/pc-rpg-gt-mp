/// <reference path='../../../types-gt-mp/index.d.ts' />

var browser = null;

var characters = null;

API.onServerEventTrigger.connect((eventName: string, args: any[]) =>
{
    if (eventName == "UpdateCharactersList")
    {
        characters = args[0];        

        if (characters != null)
            if (JSON.parse(characters).length > 0)
                ApplyCharacterFeatures(0);

        if (browser != null)
            browser.call("updateList", characters);
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

function ApplyCharacterFeatures(i)
{
    var character = JSON.parse(characters);

    API.setPlayerSkin(character[i].Gender ? -1667301416 : 1885233650);

    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character[i].Trait.FaceFirst, character[i].Trait.FaceSecond, 0, character[i].Trait.SkinFirst, character[i].Trait.SkinSecond, 0, character[i].Trait.FaceMix, character[i].Trait.SkinMix, 0, false);

    API.setPlayerClothes(API.getLocalPlayer(), 2, character[i].Trait.HairType, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), character[i].Trait.HairColor, character[i].Trait.HairHighlight);

    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), character[i].Trait.EyeColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, character[i].Trait.Eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, character[i].Trait.EyebrowsColor1, character[i].Trait.EyebrowsColor2);

    if (character[i].Trait.Beard != null)
    {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, character[i].Trait.Beard, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, character[i].Trait.BeardColor, character[i].Trait.BeardColor);
    }

    if (character[i].Trait.Makeup != null)
    {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, character[i].Trait.Makeup, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, character[i].Trait.MakeupColor, character[i].Trait.MakeupColor);
    }

    if (character[i].Trait.Lipstick != null)
    {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, character[i].Trait.Lipstick, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, character[i].Trait.LipstickColor, character[i].Trait.LipstickColor);
    }

    for (var j = 0; j < character[i].Clothes.length; j++)
    {
        API.setPlayerClothes(API.getLocalPlayer(), character[i].Clothes[j].BodyPart, character[i].Clothes[j].Variation, character[i].Clothes[j].Texture);
    }

    API.setEntityRotation(API.getLocalPlayer(), new Vector3(0.0, 0.0, 136.9184));
}

function ShowCharacterSelector()
{
    if (browser == null)
    {
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
}

function CharacterSelectorBrowserReady()
{
    API.triggerServerEvent("RetrieveCharactersList");
}

function SelectCharacterToPlay(id)
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }

    var character = JSON.parse(characters);
    API.triggerServerEvent("SelectCharacter", character[id].Id);

    API.setEntityPosition(API.getLocalPlayer(), new Vector3(-1017.67, -2754.39, 0.8003625));
    API.setEntityRotation(API.getLocalPlayer(), new Vector3(0.0, 0.0, 136.9184));

    API.setActiveCamera(null);
    API.setCanOpenChat(true);
    API.setChatVisible(true);
    API.setHudVisible(true);
    API.showCursor(false);

    resource.Login.canMove = true;
    resource.Login.stopMusic();
}

function SendToCharacterCreator()
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }

    resource.CharacterCreator.ShowCharacterCreator();
}