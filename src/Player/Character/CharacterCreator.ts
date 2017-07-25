/// <reference path='../../../types-gt-mp/index.d.ts' />

var browser = null;

API.onResourceStop.connect(() =>
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});

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
    API.setPlayerSkin(1885233650);

    API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), 0, 0, 0, 0, 0, 0, 0, 0, 0, false);

    API.setPlayerClothes(API.getLocalPlayer(), 2, 0, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), 0, 0);

    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), 0);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, 0, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, 0, 0);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, 0, API.f(0));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, 0, 0);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 4, 0, API.f(0));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 4, 0, 0, 0);

    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 8, 0, API.f(0));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 8, 2, 0, 0);

    API.setPlayerClothes(API.getLocalPlayer(), 3, 0, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 4, 1, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 6, 1, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 8, 57, 0);
    API.setPlayerClothes(API.getLocalPlayer(), 11, 1, 0);
}