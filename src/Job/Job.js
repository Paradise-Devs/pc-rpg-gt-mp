"use strict";
/// <reference path='../../types-gt-mp/Definitions/index.d.ts' />
var job_text_mode = 0;
var job_menu = null;
API.onServerEventTrigger.connect(function (event_name, args) {
    switch (event_name) {
        case "ShowJobText":
            job_text_mode = args[0];
            if (job_text_mode == 0 && job_menu != null)
                job_menu.Visible = false;
            break;
        case "JobMenu":
            var data = JSON.parse(args[0]);
            if (job_menu == null) {
                job_menu = API.createMenu("Emprego", "~b~Opções do emprego", 0, 0, 6);
            }
            job_menu.Clear();
            for (let i = 0; i < data.length; i++) {
                var temp_item = API.createMenuItem(data[i], "");
                job_menu.AddItem(temp_item);
                temp_item.Activated.connect(function (menu, item) {
                    job_menu.Visible = false;
                    API.triggerServerEvent("JobService", data[i]);
                });
            }
            var temp_item = API.createMenuItem("Sair do emprego", "Abandona seu emprego.");
            job_menu.AddItem(temp_item);
            temp_item.Activated.connect(function (menu, item) {
                job_menu.Visible = false;
                API.triggerServerEvent("JobQuit");
            });
            job_menu.Visible = true;
            break;
    }
});
API.onKeyDown.connect(function (e, key) {
    if (API.isChatOpen())
        return;
    switch (key.KeyCode) {
        case Keys.E:
            if (job_text_mode == 1)
                API.triggerServerEvent("JobInteract");
            break;
    }
});
API.onUpdate.connect(function () {
    if (job_text_mode > 0)
        API.displaySubtitle("Pressione ~b~E ~w~para interagir", 100);
});
