/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var invBrowser = null;
var inventoryData = [];

API.onResourceStart.connect(() =>
{
    var res = API.getScreenResolution();
    invBrowser = API.createCefBrowser(res.Width, res.Height);
    API.waitUntilCefBrowserInit(invBrowser);
    API.setCefBrowserPosition(invBrowser, 0, 0);
    API.loadPageCefBrowser(invBrowser, "res/views/inventory-vue.html");
    API.setCefBrowserHeadless(invBrowser, true);
});

API.onResourceStop.connect(() =>
{
    if (invBrowser != null)
    {
        API.destroyCefBrowser(invBrowser);
        invBrowser = null;
    }
});

API.onKeyUp.connect(function (sender, e)
{
    if (e.KeyCode === Keys.I)
    {
        if (API.isChatOpen() || API.isAnyMenuOpen() || !API.getHudVisible()) return;

        if (API.getCefBrowserHeadless(invBrowser))
        {            
            API.triggerServerEvent("RequestInventory");
        }
        else
        {
            API.showCursor(false);
            API.startAudio("res/sounds/inventory/close.wav", false);
            API.setCefBrowserHeadless(invBrowser, true);
        }
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    switch (eventName) {
        case "ReceiveInventory":
            // load data
            inventoryData = JSON.parse(args[0]);
            API.showCursor(true);
            API.startAudio("res/sounds/inventory/open.wav", false);
            API.setCefBrowserHeadless(invBrowser, false);
            invBrowser.call("AddItem", args[0]);
            break;
    }
    /*if (eventName == "UpdateCharacterItems")
    {
        if (invBrowser == null)
            return;

        API.showCursor(true);
        API.startAudio("res/sounds/inventory/open.wav", false);
        API.setCefBrowserHeadless(invBrowser, false);
        invBrowser.call("DrawItems", args[0], args[1]);
    }
    else if (eventName == "OnItemDiscarded")
    {
        invBrowser.call("OnItemDiscarded", args[0]);
    }*/
});

function onUseItem(id)
{
    API.triggerServerEvent("UseItem", id);
}

function onDiscardItem(selectedIdx)
{
    var dropAmount = 1; // VIR DO FRONT ISSO
    if (!inventoryData[selectedIdx].IsDroppable) return;
    var amount = (inventoryData[selectedIdx].Quantity > 1) ? dropAmount : 1;
    if (amount < 1) amount = 1;

    var pos = API.getEntityFrontPosition(API.getLocalPlayer());
    var ground = API.getGroundHeight(pos);
    API.triggerServerEvent("DropItem", selectedIdx, amount, new Vector3(pos.X, pos.Y, ground));
}