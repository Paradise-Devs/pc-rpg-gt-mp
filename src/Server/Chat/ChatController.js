"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
var mainChat = null;
var chatBrowser = null;
var currentTab = "Local";
API.onResourceStart.connect(() => {
    var res = API.getScreenResolution();
    chatBrowser = API.createCefBrowser(res.Width, res.Height);
    API.waitUntilCefBrowserInit(chatBrowser);
    API.setCefBrowserPosition(chatBrowser, 0, 0);
    API.loadPageCefBrowser(chatBrowser, "res/views/chat.html");
    mainChat = API.registerChatOverride();
    mainChat.onTick.connect(chatTick);
    mainChat.onKeyDown.connect(chatKeyDown);
    mainChat.onAddMessageRequest.connect(addMessage);
    mainChat.onChatHideRequest.connect(onChatHide);
    mainChat.onFocusChange.connect(onFocusChange);
    mainChat.SanitationLevel = 2;
});
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "OnCommitChatMessage") {
        var message = args[0];
        var tab = args[1];
        if (chatBrowser != null)
            chatBrowser.call("addMessage", message, tab);
    }
    else if (eventName == "ClearChat") {
        if (chatBrowser != null)
            chatBrowser.call("clearChat");
    }
});
API.onResourceStop.connect(function () {
    if (chatBrowser != null) {
        var localCopy = chatBrowser;
        chatBrowser = null;
        API.destroyCefBrowser(localCopy);
    }
});
function onChatHide(hide) {
    if (chatBrowser != null) {
        API.showCursor(!hide);
        API.setCefBrowserHeadless(chatBrowser, hide);
    }
}
function onFocusChange(focus) {
    if (chatBrowser != null) {
        API.showCursor(focus);
        chatBrowser.call("setFocus", focus);
    }
}
function commitMessage(msg, chat) {
    currentTab = chat;
    mainChat.sendMessage(msg);
    if (msg.replace(/\s/g, '').length && msg.charAt(0) != '/') {
        API.triggerServerEvent("OnSendChatMessage", msg, chat);
    }
}
function addMessage(msg, hasColor, r, g, b) {
    if (chatBrowser != null) {
        if (currentTab != "System")
            chatBrowser.call("addMessage", msg, currentTab);
        chatBrowser.call("addMessage", msg, "System");
    }
}
function chatTick() {
}
var devToolsShown = false;
function chatKeyDown(sender, args) {
}
