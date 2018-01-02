/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

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
                texts: ["Olá! Você está bem? O que você estava fazendo deitado alí no chão?",],
                answers: [
                    "Eu não me lembro",
                    "Estava apenas dormindo",
                    "Não te interessa"
                ]
            },
            {
                texts: [
                    "Puts, mas você está bem?",
                    "Ah, haha! Você deve estar todo dolorido, você está bem?",
                    "Nossa! Pra que tanta hostilidade. Você está bem?",
                ],
                answers: [
                    "Sim, estou bem",
                    "Sim, estou apenas dolorido",
                    "Você é bastante intrometido"
                ]
            },
            {
                texts: [
                    "Que bom! Precisa de alguma ajuda?",
                    "Ja já passa. Você precisa de ajuda?",
                    "E você é bastante ignorante, você não merece, mas vou perguntar, precisa de ajuda?",
                ],
                answers: [
                    "Sim, por favor",
                    "Sim",
                    "Sim...",
                ]
            },
            {
                texts: [
                    "Você é novo na cidade, estou certo?",
                    "Você não me é familiar, é novo na cidade?",
                    "Agora precisa de ajuda... Você é novo na cidade, né?",
                ],
                answers: [
                    "Sim",
                ]
            },
            {
                texts: [
                    "A primeira coisa a se fazer é obter habilitação para dirigir, procure por este ícone no mapa, não esqueça de levar dinheiro...",
                ],
                answers: [
                    "Hmm...",
                ]
            },
            {
                texts: [
                    "Utilize o banco a frente caso precisar, depois procure pela agencia de emprego através deste ícone no mapa.",
                ],
                answers: [
                    "Certo, obrigado!",
                    "Certo!",
                    "Tá, otário.",
                ]
            },
            {
                texts: [
                    "Bem-vindo a Los Santos, amigo!",
                    "Bem-vindo a Los Santos!",
                    "Vá embora.",
                ],
                answers: [
                ]
            }
        ]
    }
];

API.onResourceStart.connect(() =>
{
    for (let i = 0; i < _NPC.length; i++)
    {
        _NPC[i].ped = API.createPed(_NPC[i].skin, _NPC[i].position, _NPC[i].heading);
        _NPC[i].label = API.createTextLabel(_NPC[i].name, new Vector3(_NPC[i].position.X, _NPC[i].position.Y, _NPC[i].position.Z + 1.0), 15, 0.65, false);
    }
});

API.onResourceStop.connect(() =>
{
    for (let i = 0; i < _NPC.length; i++)
    {
        if (API.doesEntityExist(_NPC[i].ped))
            API.deleteEntity(_NPC[i].ped);

        if (API.doesEntityExist(_NPC[i].label))
            API.deleteEntity(_NPC[i].label);
    }
});

API.onKeyUp.connect(function (sender, e)
{
    if (e.KeyCode === Keys.E)
    {
        if (!API.getCefBrowserHeadless(resource.DialogController.dlgBrowser)) return;

        var position = API.getEntityPosition(API.getLocalPlayer());
        for (let i = 0; i < _NPC.length; i++)
        {
            if (_NPC[i].position.DistanceTo(position) > 1.25)
                continue;
                    
            API.triggerServerEvent("OnPlayerTalkToNpc", i);
        }
    }
});

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    if (eventName == "NpcDialogData")
    {
        var id = args[0];        
        var data = JSON.parse(args[1]);

        if (data != null)
        {
            _NPC[id].last_answer = data.LastAnswer;
            _NPC[id].current_conversation = data.CurrentConversation;
        }

        var camera = API.createCamera(new Vector3(_NPC[id].position.X + 1.0, _NPC[id].position.Y + 1.0, _NPC[id].position.Z + 0.7), new Vector3());
        API.pointCameraAtPosition(camera, new Vector3(_NPC[id].position.X, _NPC[id].position.Y, _NPC[id].position.Z + 0.7));
        API.setActiveCamera(camera);
        
        resource.DialogController.ShowNpcDialog(id, JSON.stringify(_NPC[id]));
    }
});