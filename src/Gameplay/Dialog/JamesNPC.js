"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var _NPC = {
    id: 0,
    name: "James",
    skin: 2010389054,
    position: new Vector3(170.8459, -993.1374, 30.09193),
    heading: 13,
    ped: null,
    label: null
};
var _Dialog = [
    {
        texts: ["Opa! Precisa de ajuda?",],
        answers: [
            "Sim, por favor",
            "Sim",
            "Se eu te chamei é porque sim, né"
        ]
    },
    {
        texts: [
            "Amigo, eu amo esta praça...",
            "Eu amo esta praça...",
            "Ah, você denovo... ",
        ],
        answers: []
    }
];
API.onResourceStart.connect(() => {
    _NPC.ped = API.createPed(_NPC.skin, _NPC.position, _NPC.heading);
    _NPC.label = API.createTextLabel(_NPC.name, new Vector3(_NPC.position.X, _NPC.position.Y, _NPC.position.Z + 1.0), 15, 0.65, false);
});
API.onResourceStop.connect(() => {
    if (API.doesEntityExist(_NPC.ped))
        API.deleteEntity(_NPC.ped);
    if (API.doesEntityExist(_NPC.label))
        API.deleteEntity(_NPC.label);
});
API.onKeyUp.connect(function (sender, e) {
    var position = API.getEntityPosition(API.getLocalPlayer());
    if (_NPC.position.DistanceTo(position) > 1.25)
        return;
    if (e.KeyCode === Keys.R) {
        if (API.getCefBrowserHeadless(resource.DialogController.browser)) {
            API.triggerServerEvent("OnPlayerTalkToNpc", _NPC.id);
        }
    }
});
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "NpcDialogData") {
        if (args[0] != _NPC.id)
            return;
        var dialog_id = 0;
        var data = JSON.parse(args[1]);
        if (data != null)
            dialog_id = data.CurrentConversation;
        var camera = API.createCamera(new Vector3(_NPC.position.X + 1.0, _NPC.position.Y + 1.0, _NPC.position.Z + 1.25), new Vector3());
        API.pointCameraAtPosition(camera, _NPC.position);
        API.setActiveCamera(camera);
        resource.DialogController.ShowNpcDialog(JSON.stringify(_NPC), JSON.stringify(_Dialog[dialog_id]), JSON.stringify(data));
    }
});
