﻿/// <reference path='../../../types-gt-mp/index.d.ts' />

var mainChat = null;
var mainBrowser = null;

API.onResourceStart.connect(() =>
{
    var res = API.getScreenResolution();
    mainBrowser = API.createCefBrowser(res.Width, res.Height);
    API.waitUntilCefBrowserInit(mainBrowser);
    API.setCefBrowserPosition(mainBrowser, 0, 0);
    API.loadPageCefBrowser(mainBrowser, "res/views/chat.html");

    mainChat = API.registerChatOverride();

    mainChat.onTick.connect(chatTick);
    mainChat.onKeyDown.connect(chatKeyDown);
    mainChat.onAddMessageRequest.connect(addMessage);
    mainChat.onChatHideRequest.connect(onChatHide);
    mainChat.onFocusChange.connect(onFocusChange);    

    mainChat.SanitationLevel = 2;
});

API.onResourceStop.connect(function ()
{
    if (mainBrowser != null)
    {
        var localCopy = mainBrowser;
        mainBrowser = null;
        API.destroyCefBrowser(localCopy);
    }
});

function addMessage(msg)
{
    if (mainBrowser != null)
    {        
        mainBrowser.call("addMessage", msg);
    }
}

function onChatHide(hide)
{
    if (mainBrowser != null)
    {
        API.setCefBrowserHeadless(mainBrowser, hide);
    }
}

function onFocusChange(focus)
{
    if (mainBrowser != null)
    {
        mainBrowser.call("setFocus", focus);
    }

    API.showCursor(focus);
}

function commitMessage(msg)
{
    mainChat.sendMessage(msg);
}

function chatTick()
{

}

var devToolsShown = false;
function chatKeyDown(sender, args)
{

}