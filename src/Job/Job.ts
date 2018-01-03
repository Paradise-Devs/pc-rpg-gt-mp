/// <reference path='../../types-gt-mp/Definitions/index.d.ts' />

var job_text_mode = 0;
var job_menu = null;

API.onServerEventTrigger.connect(function (event_name, args) {
    switch (event_name) {
        case "ShowJobText":
            job_text_mode = args[0];
            if (job_text_mode == 0 && job_menu != null) job_menu.Visible = false;
            break;

        case "JobMenu":
            API.sendNotification("TO DO: show job services");
            break;
    }
});

API.onKeyDown.connect(function (e, key) {
    if (API.isChatOpen()) return;

    switch (key.KeyCode) {
        case Keys.E:
            if (job_text_mode == 1)
                API.triggerServerEvent("JobInteract");
            break;
    }
});

API.onUpdate.connect(function () {
    if (job_text_mode > 0) API.displaySubtitle("Pressione ~y~E ~w~para interagir", 100);
});