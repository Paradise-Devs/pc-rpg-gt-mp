/// <reference path='../../../types-gt-mp/index.d.ts' />

var browser = null;

API.onResourceStart.connect(() =>
{
    if (browser == null)
    {
        InitializeDialogBrowser();
    }
});

function ShowNpcDialog(npc, dialog, data)
{
    if (browser == null || !API.isCefBrowserInitialized(browser))
        InitializeDialogBrowser();

    API.showCursor(true);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(browser, false);
    browser.call("DrawDialog", npc, dialog, data);
}

function HideNpcDialog()
{
    API.showCursor(false);
    API.setActiveCamera(null);
    resource.Sounds.PlaySelectSound();
    API.setCefBrowserHeadless(browser, true);
}

API.onResourceStop.connect(() =>
{
    if (browser != null)
    {
        API.destroyCefBrowser(browser);
        browser = null;
    }
});

function InitializeDialogBrowser()
{
    var res = API.getScreenResolution();
    browser = API.createCefBrowser(res.Width, res.Height);
    API.setCefBrowserHeadless(browser, true);
    API.setCefBrowserPosition(browser, 0, 0);
    API.loadPageCefBrowser(browser, "res/views/dialog.html");
}

function SelectedAnswer(npcid, id)
{
    API.triggerServerEvent("OnSelectAnswer", npcid, id);
}