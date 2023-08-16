# DigitigradeSetup
A utility for Unity that adds digitigrade movement to avatars.

REQUIREMENTS

Your avatar needs to have it's bones named appropriately. 
Left and right bones will need prefixes like _l or .r to be found.

Plantigrade bones require a "planti" or "plantigrade" prefix.

INSTALLATION

If you are using the Unitypackage from the releases, simply drag it to your project.

If you downloaded the code, put DigitigradeSetup.cs in Assets/Editor/ .
If there is no Editor folder, simply create it.

If the installation went well, you should be access the utility from tools/Digitigrade Setup

USAGE

Make sure to backup everything before using this script!

If you did not add planti bones to your avatar using 3D software, use the "Helper Rig" option.

WARNING: The prefab will need to be unpacked and bones will be renamed and reparented, so make sure this is the last step after you have finished everything else on your model! Making a duplicate is also a good idea!

For avatars that have both planti and digi bones, use the Auto Rig option. It will simply create the needed constraints and it will not upack or do anything to the asset.

CREDITS

DragonSkyRunner for creating the original method.
https://www.furaffinity.net/view/44035707/
