/// <reference path='../../../types-gt-mp/index.d.ts' />

var browser = null;

// Character's Data
var gender = 0;
var faceFirst = 0;
var faceSecond = 0;
var faceMix = 0.0;
var skinFirst = 0;
var skinSecond = 0;
var skinMix = 0.0;
var hairType = 0;
var hairColor = 0;
var hairHighlight = 0;
var eyeColor = 0;
var eyebrows = 0;
var eyebrowsColor1 = 0;
var eyebrowsColor2 = 0;
var beard = null;
var beardColor = 0;
var makeup = null;
var makeupColor = 0;
var lipstick = null;
var lipstickColor = 0;
var torso = 0;
var legs = 1;
var feet = 1;
var undershirt = 57;
var topshirt = 1;

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
    gender = id;
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
    API.setPlayerSkin(gender ? -1667301416 : 1885233650);

    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceFirst, faceSecond, 0, skinFirst, skinSecond, 0, faceMix, skinMix, 0, false);

    API.setPlayerClothes(API.getLocalPlayer(), 2, hairType, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), hairColor, hairHighlight);

    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), eyeColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, eyebrowsColor1, eyebrowsColor2);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, beard == null ? 0 : beard, API.f(beard == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, beardColor, beardColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, makeup == null ? 0 : makeup, API.f(makeup == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, makeupColor, makeupColor);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, lipstick == null ? 0 : lipstick, API.f(lipstick == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, lipstickColor, lipstickColor);

    API.setPlayerClothes(API.getLocalPlayer(), 3, torso, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 4, legs, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 6, feet, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 8, undershirt, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 11, topshirt, 0);
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