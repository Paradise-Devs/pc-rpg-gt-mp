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
    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), character[i].traits.FaceFirst, character[i].traits.FaceSecond, 0, character[i].traits.SkinFirst, character[i].traits.SkinSecond, 0, character[i].traits.FaceMix, character[i].traits.SkinMix, 0, false);
    API.setPlayerClothes(API.getLocalPlayer(), 2, character[i].traits.HairType, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), character[i].traits.HairColor, character[i].traits.HairHighlight);
    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), character[i].traits.EyeColor);
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, character[i].traits.Eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, character[i].traits.EyebrowsColor1, character[i].traits.EyebrowsColor2);
    if (character[i].traits.Beard != null) {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, character[i].traits.Beard, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, character[i].traits.BeardColor, character[i].traits.BeardColor);
    }
    if (character[i].traits.Makeup != null) {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, character[i].traits.Makeup, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, character[i].traits.MakeupColor, character[i].traits.MakeupColor);
    }
    if (character[i].traits.Lipstick != null) {
        API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, character[i].traits.Lipstick, API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, character[i].traits.LipstickColor, character[i].traits.LipstickColor);
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
