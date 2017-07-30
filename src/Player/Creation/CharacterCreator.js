"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var browser = null;
// Character's Data
var character_data = {
    name: "",
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
    topshirtTexture: 0,
    accessory: 0,
};
API.onResourceStop.connect(() => {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});
API.onServerEventTrigger.connect((eventName, _arguments) => {
    if (eventName == "closeCharacterCreationBrowser") {
        if (browser != null) {
            API.destroyCefBrowser(browser);
            browser = null;
        }
        API.setEntityPosition(API.getLocalPlayer(), new Vector3(169.3792, -967.8402, 29.98808));
        API.setEntityRotation(API.getLocalPlayer(), new Vector3(0, 0, 138.6666));
        API.setEntityPositionFrozen(API.getLocalPlayer(), false);
        var startCam = API.createCamera(new Vector3(169.3792, -965.8402, 31.98808), new Vector3());
        API.pointCameraAtPosition(startCam, new Vector3(169.3792, -967.8402, 29.98808));
        var endCam = API.createCamera(new Vector3(167.3792, -965.8402, 31.98808), new Vector3());
        API.pointCameraAtPosition(endCam, new Vector3(169.3792, -967.8402, 29.98808));
        API.interpolateCameras(startCam, endCam, 10000, false, false);
        API.showCursor(false);
        API.triggerServerEvent("SpawnPlayerForTheFirstTime");
        resource.Login.stopMusic();
        resource.Login.canMove = true;
    }
    else if (eventName == "CharacterNameAlreadyTaken") {
        if (browser != null) {
            browser.call("showError", "Este nome já em uso.", "#charname");
        }
    }
});
function ChangeCharacterGender(id) {
    character_data.gender = id;
    character_data.faceFirst = 0;
    character_data.faceSecond = 0;
    character_data.faceMix = 0.0;
    character_data.skinFirst = 0;
    character_data.skinSecond = 0;
    character_data.skinMix = 0.0;
    character_data.hairType = 0;
    character_data.hairColor = 0;
    character_data.hairHighlight = 0;
    character_data.eyeColor = 0;
    character_data.eyebrows = 0;
    character_data.eyebrowsColor1 = 0;
    character_data.eyebrowsColor2 = 0;
    character_data.beard = null;
    character_data.beardColor = 0;
    character_data.makeup = null;
    character_data.makeupColor = 0;
    character_data.lipstick = null;
    character_data.lipstickColor = 0;
    character_data.torso = 0;
    character_data.legs = 1;
    character_data.feet = 1;
    character_data.undershirt = 57;
    character_data.topshirt = 1;
    character_data.topshirtTexture = 0;
    character_data.accessory = 0;
    ResetCharacterCreation();
}
function ShowCharacterCreator() {
    if (browser == null) {
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
function ResetCharacterCreation() {
    character_data.torso = character_data.gender ? 5 : 0;
    character_data.undershirt = character_data.gender ? 95 : 57;
    character_data.hairType = character_data.gender ? 4 : 0;
    API.setPlayerSkin(character_data.gender ? -1667301416 : 1885233650);
    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character_data.faceFirst, character_data.faceSecond, 0, character_data.skinFirst, character_data.skinSecond, 0, character_data.faceMix, character_data.skinMix, 0, false);
    API.setPlayerClothes(API.getLocalPlayer(), 2, character_data.hairType, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), character_data.hairColor, character_data.hairHighlight);
    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), character_data.eyeColor);
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, character_data.eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, character_data.eyebrowsColor1, character_data.eyebrowsColor2);
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, character_data.beard == null ? 0 : character_data.beard, API.f(character_data.beard == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, character_data.beardColor, character_data.beardColor);
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, character_data.makeup == null ? 0 : character_data.makeup, API.f(character_data.makeup == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, character_data.makeupColor, character_data.makeupColor);
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, character_data.lipstick == null ? 0 : character_data.lipstick, API.f(character_data.lipstick == null ? 0 : 1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, character_data.lipstickColor, character_data.lipstickColor);
    API.setPlayerClothes(API.getLocalPlayer(), 3, character_data.torso, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 4, character_data.legs, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 6, character_data.feet, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 7, character_data.accessory, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 8, character_data.undershirt, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 11, character_data.topshirt, 0);
}
function MoveCameraPosition(pos) {
    switch (pos) {
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
                var camPos = new Vector3(402.9378, -997.5, -98.60);
                var camRot = new Vector3(0.0, 0.0, 176.891);
                var camera = API.createCamera(camPos, camRot);
                API.pointCameraAtPosition(camera, new Vector3(402.9198, -996.5348, -98.60));
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
function GoBackToSelection() {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
    MoveCameraPosition(-1);
    resource.CharacterSelector.ShowCharacterSelector();
}
function SelectCharacterClothes(data) {
    data = JSON.parse(data);
    if (data.torso != undefined) {
        character_data.torso = data.torso;
        API.setPlayerClothes(API.getLocalPlayer(), 3, data.torso, 0);
    }
    if (data.undershirt != undefined) {
        character_data.undershirt = data.undershirt;
        API.setPlayerClothes(API.getLocalPlayer(), 8, data.undershirt, 0);
    }
    if (data.slot == 4)
        character_data.legs = data.variation;
    else if (data.slot == 6)
        character_data.feet = data.variation;
    else if (data.slot == 7)
        character_data.accessory = data.variation;
    else if (data.slot == 8)
        character_data.undershirt = data.variation;
    else if (data.slot == 11) {
        character_data.topshirt = data.variation;
        character_data.topshirtTexture = data.texture;
    }
    API.setPlayerClothes(API.getLocalPlayer(), data.slot, data.variation, data.texture);
}
function SelectCharacterComponent(data) {
    var mix_data = [
        0.0,
        0.1,
        0.2,
        0.3,
        0.4,
        0.5,
        0.6,
        0.7,
        0.8,
        0.9
    ];
    data = JSON.parse(data);
    switch (data.type) {
        // Face
        case 13:
            {
                character_data.faceFirst = data.config1;
                character_data.faceSecond = data.config2;
                character_data.faceMix = mix_data[data.config3];
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character_data.faceFirst, character_data.faceSecond, 0, character_data.skinFirst, character_data.skinSecond, 0, character_data.faceMix, character_data.skinMix, 0, false);
                break;
            }
        // Eyes
        case 14:
            {
                character_data.eyeColor = data.config1;
                API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), character_data.eyeColor);
                break;
            }
        // Hair
        case 15:
            {
                character_data.hairType = data.config1;
                character_data.hairColor = data.config2;
                character_data.hairHighlight = data.config3;
                API.setPlayerClothes(API.getLocalPlayer(), 2, character_data.hairType, 0);
                API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), character_data.hairColor, character_data.hairHighlight);
                break;
            }
        // Beard or Makeup
        case 16:
            {
                if (!character_data.gender) {
                    if (data.config1 < 1) {
                        character_data.beard = null;
                        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, data.config1, API.f(0));
                    }
                    else {
                        character_data.beard = data.config1 - 1;
                        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, character_data.beard, API.f(1));
                    }
                    character_data.beardColor = data.config2;
                    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, character_data.beardColor, character_data.beardColor);
                }
                else {
                    if (data.config1 < 1) {
                        character_data.lipstick = null;
                        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, data.config1, API.f(0));
                    }
                    else {
                        character_data.lipstick = data.config1 - 1;
                        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, character_data.lipstick, API.f(1));
                    }
                    character_data.lipstickColor = data.config2;
                    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, character_data.lipstickColor, character_data.lipstickColor);
                }
                break;
            }
        // Skin color
        case 17:
            {
                character_data.skinFirst = data.config1;
                character_data.skinSecond = data.config2;
                character_data.skinMix = mix_data[data.config3];
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character_data.faceFirst, character_data.faceSecond, 0, character_data.skinFirst, character_data.skinSecond, 0, character_data.faceMix, character_data.skinMix, 0, false);
                break;
            }
        // Eyebrow
        case 18:
            {
                character_data.eyebrows = data.config1;
                character_data.eyebrowsColor1 = data.config2;
                character_data.eyebrowsColor2 = data.config2;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, character_data.eyebrows, API.f(1));
                API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, character_data.eyebrowsColor1, character_data.eyebrowsColor2);
                break;
            }
    }
}
function finishCharacterCreation(character_name) {
    character_data.name = character_name;
    API.triggerServerEvent("FinishCharacterCreation", API.toJson(character_data));
}
