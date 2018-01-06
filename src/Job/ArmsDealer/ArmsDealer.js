"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
let box_id = 0;
let job_started = false;
var box = null;
var blip = null;
var label = null;
let box_positions = [
    { x: 520.1843, y: -1650.249, z: 33.70755 },
    { x: 484.0400, y: -1510.422, z: 37.70727 },
    { x: 481.5940, y: -1473.771, z: 34.93475 },
    { x: 304.5107, y: -1246.784, z: 39.42271 },
    { x: -59.0449, y: -1325.237, z: 33.19511 },
    { x: -145.721, y: -1291.190, z: 47.89811 },
    { x: -344.848, y: -1338.174, z: 37.30556 },
];
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "ArmsDealer_SpawnMaterial") {
        if (job_started) {
            API.sendNotification("~r~ERRO: ~w~Você já iniciou o serviço.");
            return;
        }
        job_started = true;
        box_id = Math.floor((Math.random() * box_positions.length));
        label = API.createTextLabel("~y~Caixa de materiais~n~~w~Pressione ~y~E", new Vector3(box_positions[box_id].x, box_positions[box_id].y, box_positions[box_id].z + 0.5), 15, 0.5);
        box = API.createObject(1302435108, new Vector3(box_positions[box_id].x, box_positions[box_id].y, box_positions[box_id].z - 0.75), new Vector3());
        blip = API.createBlip(new Vector3(box_positions[box_id].x, box_positions[box_id].y, box_positions[box_id].z));
        API.setBlipSprite(blip, 478);
        API.setBlipColor(blip, 50);
        API.showBlipRoute(blip, true);
        API.sendNotification("O fornecedor marcou a localização dos materiais em seu radar.");
    }
});
API.onKeyDown.connect((sender, e) => {
    if (!job_started)
        return;
    var player = API.getLocalPlayer();
    if (e.KeyCode === Keys.E) {
        if (new Vector3(box_positions[box_id].x, box_positions[box_id].y, box_positions[box_id].z).DistanceTo(API.getEntityPosition(player)) < 2) {
            API.deleteEntity(label);
            API.deleteEntity(box);
            API.deleteEntity(blip);
            job_started = false;
            API.triggerServerEvent("ArmsDealer_TakeMaterial");
        }
    }
});
