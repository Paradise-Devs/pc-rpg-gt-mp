/// <reference path='../../../types-gt-mp/index.d.ts' />

var browser = null;

// Character's Data
var characterData = {
    gender: 0,
    faceFirst: 0,
    faceSecond: 0,
    faceMix: 0.0,
    skinFirst: 0,
    skinSecond: 0,
    skinMix: 0.0,
    hairType: 0,
    hairColor: 0,
    hairHighlight: 0,
    eyeColor: 0,
    eyebrows: 0,
    eyebrowsColor1: 0,
    eyebrowsColor2: 0,
    beard: null,
    beardColor: 0,
    makeup: null,
    makeupColor: 0,
    lipstick: null,
    lipstickColor: 0,
    torso: 0,
    legs: 1,
    feet: 1,
    undershirt: 57,
    topshirt: 1,
};

API.onResourceStop.connect(() =>
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});

function ChangeCharacterGender(id)
{
    characterData.gender = id;
    ResetCharacterCreation();
}

function ShowCharacterCreator()
{
    if (browser == null)
    {
        var res = API.getScreenResolution();
        browser = API.createCefBrowser(res.Width, res.Height);
        API.setCefBrowserHeadless(browser, true);
        API.setCefBrowserPosition(browser, 0, 0);
        API.waitUntilCefBrowserInit(browser);
        API.loadPageCefBrowser(browser, "res/views/character_creator.html");
        API.waitUntilCefBrowserLoaded(browser);
    }
    API.showCursor(true);
    API.setCanOpenChat(false);
    API.setCefBrowserHeadless(browser, false);

    ResetCharacterCreation();
}

function ResetCharacterCreation()
{
    characterData.torso = characterData.gender ? 5 : 0;
    characterData.undershirt = characterData.gender ? 95 : 57;
    characterData.hairType = characterData.gender ? 4 : 0;    

    API.setPlayerSkin(characterData.gender ? -1667301416 : 1885233650);

    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), characterData.faceFirst, characterData.faceSecond, 0, characterData.skinFirst, characterData.skinSecond, 0, characterData.faceMix, characterData.skinMix, 0, false);

    API.setPlayerClothes(API.getLocalPlayer(), 2, characterData.hairType, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), characterData.hairColor, characterData.hairHighlight);

    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), characterData.eyeColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, characterData.eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, characterData.eyebrowsColor1, characterData.eyebrowsColor2);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, characterData.beard == null ? 0 : characterData.beard, API.f(characterData.beard == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, characterData.beardColor, characterData.beardColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, characterData.makeup == null ? 0 : characterData.makeup, API.f(characterData.makeup == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, characterData.makeupColor, characterData.makeupColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, characterData.lipstick == null ? 0 : characterData.lipstick, API.f(characterData.lipstick == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, characterData.lipstickColor, characterData.lipstickColor);

    API.setPlayerClothes(API.getLocalPlayer(), 3, characterData.torso, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 4, characterData.legs, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 6, characterData.feet, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 8, characterData.undershirt, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 11, characterData.topshirt, 0);
}

function MoveCameraPosition(pos)
{
    switch (pos)
    {
        case 0:
            {
                // Head
                var camPos = new Vector3(402.9378, -997.0, -98.35);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -98.35));
                API.interpolateCameras(API.getActiveCamera(), camera, 2000, false, false);
                break;
            }
        case 1:
            {
                // Torso
                var camPos = new Vector3(402.9378, -997.5, -98.90);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -98.90));
                API.interpolateCameras(API.getActiveCamera(), camera, 2000, false, false);
                break;
            }
        case 2:
            {
                // Legs
                var camPos = new Vector3(402.9378, -997.5, -99.40);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -99.40));
                API.interpolateCameras(API.getActiveCamera(), camera, 2000, false, false);
                break;
            }
        case 3:
            {
                // Feet
                var camPos = new Vector3(402.9378, -997.5, -99.85);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -99.85));
                API.interpolateCameras(API.getActiveCamera(), camera, 2000, false, false);
                break;
            }
        default:
            {
                // Default
                var camPos = new Vector3(403.6378, -998.5422, -99.00404);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -99.00024));
                API.interpolateCameras(API.getActiveCamera(), camera, 2000, false, false);
                break;
            }
    }
}

function GoBackToSelection()
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }
    MoveCameraPosition(-1);
    resource.CharacterSelector.ShowCharacterSelector();
}