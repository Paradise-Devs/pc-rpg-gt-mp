"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var browser = null;
var selected_character = null;
var characters = null;
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "UpdateCharactersList") {
        characters = args[0];
        if (JSON.parse(characters).length > 0)
            ApplyCharacterFeatures(0);
        if (browser != null)
            browser.call("updateList", characters);
    }
});
API.onResourceStop.connect(() => {
    if (browser != null) {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});
function ApplyCharacterFeatures(i) {
    var character = JSON.parse(characters);
    API.setPlayerSkin(character[i].Gender ? -1667301416 : 1885233650);
    for (var j = 0; j < character[i].Traits.length; j++) {
        API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character[i].Traits[j].FaceFirst, character[i].Traits[j].FaceSecond, 0, character[i].Traits[j].SkinFirst, character[i].Traits[j].SkinSecond, 0, character[i].Traits[j].FaceMix, character[i].Traits[j].SkinMix, 0, false);
        API.setPlayerClothes(API.getLocalPlayer(), 2, character[i].Traits[j].HairType, 0);
        API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), character[i].Traits[j].HairColor, character[i].Traits[j].HairHighlight);
        API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), character[i].Traits[j].EyeColor);
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, character[i].Traits[j].Eyebrows, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, character[i].Traits[j].EyebrowsColor1, character[i].Traits[j].EyebrowsColor2);
        if (character[i].Traits[j].Beard != null) {
            API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, character[i].Traits[j].Beard, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, character[i].Traits[j].BeardColor, character[i].Traits[j].BeardColor);
        }
        if (character[i].Traits[j].Makeup != null) {
            API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, character[i].Traits[j].Makeup, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, character[i].Traits[j].MakeupColor, character[i].Traits[j].MakeupColor);
        }
        if (character[i].Traits[j].Lipstick != null) {
            API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, character[i].Traits[j].Lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, character[i].Traits[j].LipstickColor, character[i].Traits[j].LipstickColor);
        }
    }
    for (var j = 0; j < character[i].Clothes.length; j++) {
        if (character[i].Clothes[j].Torso != null)
            API.setPlayerClothes(API.getLocalPlayer(), 3, character[i].Clothes[j].Torso, 0);
        API.setPlayerClothes(API.getLocalPlayer(), character[i].Clothes[j].BodyPart, character[i].Clothes[j].Variation, 0);
    }
}
function ShowCharacterSelector() {
    if (browser == null) {
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
}
function CharacterSelectorBrowserReady() {
    API.triggerServerEvent("RetrieveCharactersList");
}
