# Developer Readme

## Getting Started

### Local development
This project requires external references found in the KSP folder
- ..\Kerbal Space Program\KSP_x64_Data\Managed\Assembly-CSharp.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\Assembly-CSharp-firstpass.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.CoreModule.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.AnimationModule.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.AudioModule.dll

Place a lightweight KSP install in the parent directory where this project
resides:
- .\dev\Stargate (this project)
- .\dev\Kerbal Space Program

Remove unnecessary parts from the KSP install to decrease load times. You likely
still need wheels, RCS blocks, plate structures to build the stargate craft
and puddle jumpers that move through them.

### Build
Building this project will copy the _"Stargate.dll"_ into the
"_.\dist\Blaarkies\Stargate_" directory, and also attempt to copy that into
"_..\Kerbal Space Program\GameData\Blaarkies\Stargate_".

An after-build command will attempt to run _"KspShortcut.lnk"_, which should
load up KSP to test the changes. Make sure this shortcut points to your
"_KSP_x64.exe_" executable.

The plan is to have a build script that compiles a ready to use package that
then gets copied over to the KSP directory. This package should include all configs
and models, so that any and all changes need only happen in one directory.

## Assets
The `Stargate\assets` folder contains various assets for use in this project. If 
you find alternatives or better assets to use, feel free to add them in the folder
for others to access. Please name/place them properly as to describe their usage.

If such files came with a license, please add that to the `Stargate\docs\licenses`
folder in order to credit the author of the work  as applicable.

The assets directory is a storage place from where developers can copy these files
into the `Stargate\dist` folder as required.

### Sound `Stargate\assets\sound`
This directory contains sound samples in compressed form. KSP loads `.wav` and 
`.ogg` files. It is important to make changes to these files before copying to 
the `dist` folder, so that others may edit this as well without losing the 
previous changes.


### Texture `Stargate\assets\texture`
A simple folder containing some images to be used as textures on the model. These
files likely need to be converted to `.dds` format to use in KSP.

### Model `Stargate\assets\model`
Folder containing `.blend` files for each stargate model used.

## Typical Flow

### Logic / Code
In the `Stargate\Src` folder, go into a `.cs` file and add a line `Debug.Log("Hello world");`

Now build the project from an IDE. It should automatically compile the project into a DLL 
file, it will copy this file into the `dist` folder, then it will copy that folder to
the KSP install, and then run the KSP executable.

This will present you with the modified game. Simply load a save game and verify that
the change works.

### KSP Configs
Go into `Stargate\dist\Blaarkies\Stargate\ResourceNaquadah.cfg` and add a new resource
called _HelloWorldium_. Even though no code changes were made, it is often easier to 
build the project again as this will copy the changes into the KSP install, and startup
the game as well to verify that the config change works as expected.

### Sound
Go to `Stargate\assets\sound`, open one of the files using 
[Audacity](https://www.audacityteam.org/download/). Trim the 
leading silence that often appears in sound files. Save the changes to the **asset** and
then copy & convert the asset to a `.ogg` file to be placed in 
`Stargate\dist\Blaarkies\Stargate\sound`.

Then build the project to automatically copy the changes to the KSP install to verify 
that the changes are correct.

### Texture
Go to `Stargate\assets\texture` and open up a texture with 
[Pain.NET](https://www.getpaint.net/download.html). Write "Hello World", and save the 
**asset**. Then also Save As a `.dds` type in the `dist` folder. This is best done
in conjunction with the model opened to see the effect of the texture file changes.

Now build the project to copy these changes to the KSP install, and verify the texture
in-game.

### Model
See [Design Guidelines](https://github.com/Blaarkies/ksp-mod-stargate/blob/main/assets/model/README.md).

Go to `Stargate\assets\texture` and open up a model with
[Blender](https://www.blender.org/download/). Rotate a few vertexes in edit mode, 
and save the **asset**. Then select the highest ranking parent object (in the tree view)
and export the model as `.mu` (requires the [Blender-mu-importexport-addon](https://forum.kerbalspaceprogram.com/index.php?/topic/40056-12-17-blender-283-mu-importexport-addon/))
in the `dist` folder.

Add descriptive entries into the `CHANGELOG.md` file for that model.

Run the build to have the files copied to the KSP install and test the changes in-game.
The names of animations, textures, and the types on objects in the model have
an influence on how they are handled in code. Some model changes will obviously
need code changes to accompany them.
