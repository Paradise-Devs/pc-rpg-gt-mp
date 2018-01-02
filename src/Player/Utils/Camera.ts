/// <reference path='../../../types-gt-mp/Definitions/index.d.ts' />

API.onServerEventTrigger.connect((eventName: string, args: System.Array<any>) =>
{
    if (eventName == "CreateCamera")
    {
		var newCam = API.createCamera(args[0], new Vector3());
		API.pointCameraAtPosition(newCam, args[1]);
		API.setActiveCamera(newCam);
	}
    else if (eventName == "ResetCamera")
    {
		API.setActiveCamera(null);
	}
});
