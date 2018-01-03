var res = API.getScreenResolutionMaintainRatio();

var lastCheck = 0;
var checkInterval = 500;

var lookingAtEntity = null;
var lookingAtEntityPos = null;

var highlightColor = [26, 188, 156];

function Vector3Lerp(start, end, fraction) {
    return new Vector3(
        (start.X + (end.X - start.X) * fraction),
        (start.Y + (end.Y - start.Y) * fraction),
        (start.Z + (end.Z - start.Z) * fraction)
    );
}

function getLookingAtEntity() {
    var startPoint = API.getGameplayCamPos();
    var aimPoint = API.screenToWorldMaintainRatio(new PointF(res.Width / 2, res.Height / 2));
    startPoint.Add(aimPoint);

    var endPoint = Vector3Lerp(startPoint, aimPoint, 1.1);
    var rayCast = API.createRaycast(startPoint, endPoint, (1 | 16), null);
    if (!rayCast.didHitEntity) return null;

    var hitEntityHandle = rayCast.hitEntity;
    if (API.getEntityPosition(hitEntityHandle).DistanceTo(API.getEntityPosition(API.getLocalPlayer())) > 2) return null;
    if (!API.hasEntitySyncedData(hitEntityHandle, "ItemDropID")) return null;
    return hitEntityHandle;
}

API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode == Keys.E) {
        if (API.isChatOpen() || API.isAnyMenuOpen() || lookingAtEntity == null || !API.hasEntitySyncedData(lookingAtEntity, "ItemDropID")) return;
        API.triggerServerEvent("TakeItem", API.getEntitySyncedData(lookingAtEntity, "ItemDropID"));
    }
});

API.onUpdate.connect(function () {
    if (API.getHudVisible() && !API.isPlayerInAnyVehicle(API.getLocalPlayer()) && !API.isPlayerAiming(API.getLocalPlayer()) && !API.isPlayerShooting(API.getLocalPlayer())) API.drawRectangle(res.Width / 2, res.Height / 2, 4, 4, 255, 255, 255, 255);

    if (!API.isPlayerInAnyVehicle(API.getLocalPlayer()) && API.getGlobalTime() - lastCheck > checkInterval) {
        lookingAtEntity = getLookingAtEntity();
        if (lookingAtEntity != null) lookingAtEntityPos = API.getEntityPosition(lookingAtEntity);

        lastCheck = API.getGlobalTime();
    }

    if (!API.isAnyMenuOpen() && lookingAtEntity != null) {
        API.callNative("_DRAW_LIGHT_WITH_RANGE_AND_SHADOW", lookingAtEntityPos.X, lookingAtEntityPos.Y, lookingAtEntityPos.Z, highlightColor[0], highlightColor[1], highlightColor[2], API.f(1.0), API.f(10.0), API.f(10.0));

        API.callNative("SET_DRAW_ORIGIN", lookingAtEntityPos.X, lookingAtEntityPos.Y, lookingAtEntityPos.Z + 0.5, 0);
        API.drawText("Pressione ~y~E ~w~para pegar.", 0, 0, 0.5, 255, 255, 255, 255, 4, 1, true, true, 500);
        API.callNative("CLEAR_DRAW_ORIGIN");
    }
});