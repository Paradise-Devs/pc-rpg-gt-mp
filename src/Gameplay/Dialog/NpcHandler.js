"use strict";
/// <reference path='../../../types-gt-mp/index.d.ts' />
var _NPC = [
    {
        name: "James",
        skin: 2010389054,
        position: new Vector3(170.8459, -993.1374, 30.09193),
        heading: 13,
        ped: null,
        label: null,
        last_answer: 0,
        current_conversation: 0,
        dialogs: [
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
        ]
    }
];
API.onResourceStart.connect(() => {
    for (let i = 0; i < _NPC.length; i++) {
        _NPC[i].ped = API.createPed(_NPC[i].skin, _NPC[i].position, _NPC[i].heading);
        _NPC[i].label = API.createTextLabel(_NPC[i].name, new Vector3(_NPC[i].position.X, _NPC[i].position.Y, _NPC[i].position.Z + 1.0), 15, 0.65, false);
    }
});
API.onResourceStop.connect(() => {
    for (let i = 0; i < _NPC.length; i++) {
        if (API.doesEntityExist(_NPC[i].ped))
            API.deleteEntity(_NPC[i].ped);
        if (API.doesEntityExist(_NPC[i].label))
            API.deleteEntity(_NPC[i].label);
    }
});
API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode === Keys.R) {
        var position = API.getEntityPosition(API.getLocalPlayer());
        for (let i = 0; i < _NPC.length; i++) {
            if (_NPC[i].position.DistanceTo(position) > 1.25)
                continue;
            if (API.getCefBrowserHeadless(resource.DialogController.browser))
                API.triggerServerEvent("OnPlayerTalkToNpc", i);
        }
    }
});
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "NpcDialogData") {
        var id = args[0];
        var data = JSON.parse(args[1]);
        if (data != null) {
            _NPC[id].last_answer = data.LastAnswer;
            _NPC[id].current_conversation = data.CurrentConversation;
        }
        var camera = API.createCamera(new Vector3(_NPC[id].position.X + 1.0, _NPC[id].position.Y + 1.0, _NPC[id].position.Z + 1.25), new Vector3());
        API.pointCameraAtPosition(camera, _NPC[id].position);
        API.setActiveCamera(camera);
        resource.DialogController.ShowNpcDialog(id, JSON.stringify(_NPC[id]));
    }
});
