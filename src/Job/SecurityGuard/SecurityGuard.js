"use strict";
/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />
var truck = null;
var truckBlip = null;
var blip = null;
var marker = null;
var label = null;
var checkpoint = null;
var is_carrying = false;
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "SecurityGuardJobStart") {
        if (truckBlip != null)
            API.deleteEntity(truckBlip);
        truck = args[0];
        truckBlip = API.createBlip(API.getEntityPosition(truck));
        API.setBlipSprite(truckBlip, 67);
        API.setBlipName(truckBlip, "Veículo de trabalho");
        API.attachEntity(truckBlip, truck, "", new Vector3(), new Vector3());
        // checkpoints
        checkpoint = args[1];
        createSecurityServiceEntities();
    }
    else if (eventName == "SecurityGuardGetNextCheckpoint") {
        checkpoint = args[0];
        createSecurityServiceEntities();
    }
    else if (eventName == "SecurityGuardDeleteEntities") {
        deleteSecurityServiceEntities();
    }
});
API.onKeyUp.connect((sender, key) => {
    switch (key.KeyCode) {
        case Keys.E:
            if (checkpoint == null)
                return;
            if (API.getEntityPosition(API.getLocalPlayer()).DistanceTo(checkpoint) < 1) {
                deleteSecurityServiceEntities();
                if (!is_carrying) {
                    is_carrying = true;
                    API.triggerServerEvent("SecurityGuardCarryAnimPlay");
                    checkpoint = API.getEntityRearPosition(truck);
                    createSecurityServiceEntities();
                }
                else {
                    is_carrying = false;
                    API.triggerServerEvent("SecurityGuardCarryAnimStop");
                    API.triggerServerEvent("SecurityGuardGetNextCheckpoint");
                }
            }
            break;
    }
});
API.onUpdate.connect(() => {
    if (truck == null)
        return;
    if (!API.doesEntityExist(truck))
        truck = null;
    if (API.getEntityPosition(truck).DistanceTo(API.getEntityPosition(API.getLocalPlayer())) < 5 && truckBlip != null) {
        API.deleteEntity(truckBlip);
        truckBlip = null;
    }
    else if (API.getEntityPosition(truck).DistanceTo(API.getEntityPosition(API.getLocalPlayer())) > 50 && truckBlip == null) {
        truckBlip = API.createBlip(API.getEntityPosition(truck));
        API.setBlipSprite(truckBlip, 67);
        API.setBlipName(truckBlip, "Veículo de trabalho");
        API.attachEntity(truckBlip, truck, "", new Vector3(), new Vector3());
    }
});
function deleteSecurityServiceEntities() {
    if (blip != null) {
        API.deleteEntity(blip);
        blip = null;
    }
    if (label != null) {
        API.deleteEntity(label);
        label = null;
    }
    if (marker != null) {
        API.deleteEntity(marker);
        marker = null;
    }
    checkpoint = null;
}
function createSecurityServiceEntities() {
    blip = API.createBlip(checkpoint);
    API.setBlipName(blip, "Trabalho");
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 1);
    API.showBlipRoute(blip, true);
    marker = API.createMarker(1, new Vector3(checkpoint.X, checkpoint.Y, checkpoint.Z - 1.0), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 0, 0, 255);
    label = API.createTextLabel((is_carrying) ? "~b~Colocar dinheiro~n~~w~Pressione ~b~E" : "~b~Pegar dinheiro~n~~w~Pressione ~b~E", new Vector3(checkpoint.X, checkpoint.Y, checkpoint.Z + 0.5), 15, 0.5);
}
