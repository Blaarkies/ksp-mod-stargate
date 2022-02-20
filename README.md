# S T A R G Å T E

#### Kerbal Space Program mod that adds portals to the game

### What is it about?
In [Stargate](https://en.wikipedia.org/wiki/Stargate), kerbals discovered
an ancient ring structure 6.7 m in diameter. The origins of this device 
is unknown, but kerbals figured out how to start up the device. They 
soon learned that it can open up wormholes to other Stargates in the 
galaxy, through which kerbals and vessels could travel.

<br><br>

---
## ⨂ This plugin is currently in pre-alpha ⨂
_Do not install this on you main savegame, it contains the bare minimum 
functionality as a proof of concept. Feedback is greatly appreciated, 
please see the bottom of this file._

<br>

---


### How does it work?
- Players get a new "Stargate" part under the utilities category
- Use this as part of a new craft
- Place this craft next to the KSC runway, and place another 
craft (that also contains a stargate) on the Mun
- Now launch a 3rd craft, a simple rover will suffice
- Drive up to the runway stargate
- Without changing focus, right-click on the stargate
  - The PAW menu allows targeting other stargates
  - Target the Mun stargate
  - Then click "Open Wormhole"
- While the wormhole is open, drive through the stargate's event horizon
- The rover will be instantly transported, driving out of the Mun stargate

### Details

#### New resource - Naquadah (unit: NQ)
- A rare, super-dense mineral (atomic weight of 98.6389) with properties
that amplify energy
- Stargates require Naquadah in order to function:
  - 0.01 NQ per second to keep the wormhole open
  - 1 NQ per ton of transported craft
- Naquadah cannot be bought at the KSC, it can only be mined from asteroids,
comets, Kerbin's pyramid temple, and Vallhenge
- New container parts hold Naquadah, stargates require these to be on the
same vessel

### New parts - Stargate
- A ring device that initiates wormhole travel
- After selecting another stargate as a target, the "Open Wormhole" button can be clicked
  - A cancel button will appear in the menu
- This starts the dialing sequence that takes 10-15 seconds
  - Parts of the stargate will spin to "dial" the other gate address
  - Chevrons emit light when locking in address combinations
  - When the final chevron is locked: 
    - A water-blue colored event horizon appears
    - An [Unstable vortex](https://stargate.fandom.com/wiki/Unstable_vortex) forms (a.k.a "kawoosh")
that destroys any parts of nearby ships inside the vortex space
  - A calm blue event horizon remains, sapping a constant drain of NQ for up to 5 minutes,
at which point the gate automatically shuts down
  - Any ship whose CoM moves from one side of the gate, through to the other gets teleported to the
target gate
    - The destination gate starts by also having a blue event horizon open, 
that shortly closes after the vessel's arrival
- Kerbals can walk through one gate and out the other, rover can drive through seamlessly, and spacecraft
can drift through one gate and out the other
  - Any wormhole with a surface destination will rotate the kerbal/vessel to have it's feet downwards,
or at least have the same relative orientation at the destination as the had at the origin gate

<br>

---

## Mod Development

### New features and bug fixes
This repo's **github issue tracker** is used to organize new features and bugfixes.

Feel free to add feature request there, and as much detail as possible, 
including external links, images, videos, etc.

A lot of the details mentioned [here](#details) does not yet exist, and are planned features
in the issue tracker.

### Pull requests
Help and contributions are welcome (and even encouraged). Pull requests will be 
reviewed and squashed-committed to origin

### Local development
This project requires external references found in the KSP folder
- ..\Kerbal Space Program\KSP_x64_Data\Managed\Assembly-CSharp.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\Assembly-CSharp-firstpass.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.CoreModule.dll
- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.AnimationModule.dll

[comment]: <> (- ..\Kerbal Space Program\KSP_x64_Data\Managed\UnityEngine.AudioModule.dll)

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



