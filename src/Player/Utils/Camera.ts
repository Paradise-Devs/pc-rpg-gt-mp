/// <reference path='../../../types-gt-mp/index.d.ts' />

API.onServerEventTrigger.connect((name: string, args: any[]) =>
{
    if (name == "CreateCamera")
    {
		var newCam = API.createCamera(args[0], new Vector3());
		API.pointCameraAtPosition(newCam, args[1]);
		API.setActiveCamera(newCam);
	}
    else if (name == "ResetCamera")
    {
		API.setActiveCamera(null);
	}
});
