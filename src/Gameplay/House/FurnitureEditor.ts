/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

var screenMR = API.getScreenResolutionMaintainRatio();
var mapMarginLeft = screenMR.Width / 90;
var mapMarginBottom = screenMR.Height / 86;
var mapWidth = screenMR.Width / 7.11;
var mapHeight = screenMR.Height / 5.71;
var resX = mapMarginLeft + mapWidth + mapMarginLeft;
var resY = screenMR.Height - mapHeight - mapMarginBottom;

var moving_speeds = [0.001, 0.01, 0.1, 1.0, 5.0, 10.0];
var moving_speed_idx = 0;

var editing_types = ["Pos X", "Pos Y", "Pos Z", "Rot X", "Rot Y", "Rot Z"];
var editing_type_idx = 0;

var editing_handle = null;

API.onServerEventTrigger.connect(function (event_name, args) {
    switch (event_name) {
        case "SetEditingHandle":
            editing_handle = args[0];
            editing_type_idx = 0;
            moving_speed_idx = 0;

            if (editing_handle != null) API.callNative("SET_ENTITY_COLLISION", editing_handle, false, true);
            resource.House.hideMenus();
            break;

        case "ResetEntityPosition":
            API.setEntityPosition(editing_handle, args[0]);
            API.setEntityRotation(editing_handle, args[1]);

            editing_handle = null;
            break;
    }
});

API.onKeyDown.connect(function (e, key) {
    if (editing_handle == null || API.isChatOpen()) return;

    switch (key.KeyCode) {
        // Increase value
        case Keys.Up:
            switch (editing_type_idx) {
                // pos x
                case 0:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(moving_speeds[moving_speed_idx], 0.0, 0.0));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // pos y
                case 1:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(0.0, moving_speeds[moving_speed_idx], 0.0));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // pos z
                case 2:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(0.0, 0.0, moving_speeds[moving_speed_idx]));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // rot x
                case 3:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X + moving_speeds[moving_speed_idx], rot.Y, rot.Z));
                    break;

                // rot y
                case 4:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X, rot.Y + moving_speeds[moving_speed_idx], rot.Z));
                    break;

                // rot z
                case 5:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X, rot.Y, rot.Z + moving_speeds[moving_speed_idx]));
                    break;
            }

            break;

        // Decrease value
        case Keys.Down:
            switch (editing_type_idx) {
                // pos x
                case 0:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(-moving_speeds[moving_speed_idx], 0.0, 0.0));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // pos y
                case 1:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(0.0, -moving_speeds[moving_speed_idx], 0.0));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // pos z
                case 2:
                    var pos = API.getOffsetInWorldCoords(editing_handle, new Vector3(0.0, 0.0, -moving_speeds[moving_speed_idx]));
                    API.setEntityPosition(editing_handle, pos);
                    break;

                // rot x
                case 3:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X - moving_speeds[moving_speed_idx], rot.Y, rot.Z));
                    break;

                // rot y
                case 4:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X, rot.Y - moving_speeds[moving_speed_idx], rot.Z));
                    break;

                // rot z
                case 5:
                    var rot = API.getEntityRotation(editing_handle);
                    API.setEntityRotation(editing_handle, new Vector3(rot.X, rot.Y, rot.Z - moving_speeds[moving_speed_idx]));
                    break;
            }

            break;

        // Change editing type - increase
        case Keys.Right:
            editing_type_idx++;
            if (editing_type_idx >= editing_types.length) editing_type_idx = 0;

            API.displaySubtitle("Edit type: ~y~" + editing_types[editing_type_idx], 2000);
            break;

        // Change editing type - decrease
        case Keys.Left:
            editing_type_idx--;
            if (editing_type_idx < 0) editing_type_idx = editing_types.length - 1;

            API.displaySubtitle("Edit type: ~y~" + editing_types[editing_type_idx], 2000);
            break;

        // Moving speed - increase
        case Keys.Add:
            moving_speed_idx++;
            if (moving_speed_idx >= moving_speeds.length) moving_speed_idx = 0;

            API.displaySubtitle("Moving speed: ~y~" + moving_speeds[moving_speed_idx], 2000);
            break;

        // Moving speed - decrease
        case Keys.Subtract:
            moving_speed_idx--;
            if (moving_speed_idx < 0) moving_speed_idx = moving_speeds.length - 1;

            API.displaySubtitle("Moving speed: ~y~" + moving_speeds[moving_speed_idx], 2000);
            break;

        // Finish editing
        case Keys.Y:
            API.callNative("SET_ENTITY_COLLISION", editing_handle, true, true);

            var pos = API.getEntityPosition(editing_handle);
            var rot = API.getEntityRotation(editing_handle);
            API.triggerServerEvent("HouseSaveFurniture", pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z);

            editing_handle = null;

            API.displaySubtitle("~g~Saved.", 2000);
            break;

        // Cancel editing
        case Keys.N:
            API.callNative("SET_ENTITY_COLLISION", editing_handle, true, true);
            API.triggerServerEvent("HouseResetFurniture");

            API.displaySubtitle("~r~Cancelled.", 2000);
            break;
    }
});

API.onUpdate.connect(function () {
    if (editing_handle == null) return;
    API.drawText("Editing: ~y~" + editing_types[editing_type_idx], resX, resY - 25, 0.55, 255, 255, 255, 255, 4, 0, true, true, 0);
    API.drawText("Speed: ~y~" + moving_speeds[moving_speed_idx], resX, resY + 5, 0.55, 255, 255, 255, 255, 4, 0, true, true, 0);

    API.drawText("~b~Up/Down arrows to move the object.", resX, resY + 60, 0.45, 255, 255, 255, 255, 4, 0, true, true, 0);
    API.drawText("~b~Left/Right arrows to change edit type.", resX, resY + 90, 0.45, 255, 255, 255, 255, 4, 0, true, true, 0);
    API.drawText("~b~+/- to change speed.", resX, resY + 120, 0.45, 255, 255, 255, 255, 4, 0, true, true, 0);
    API.drawText("~b~Y/N to save/cancel editing.", resX, resY + 150, 0.45, 255, 255, 255, 255, 4, 0, true, true, 0);
});