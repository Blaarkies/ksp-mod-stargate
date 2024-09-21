# S T A R G Å T E

#### A Kerbal Space Program mod that adds portals to the game

(in progress demo)[https://youtu.be/unWgcGZV8Hk]

### What is it about?
In [Stargate](https://en.wikipedia.org/wiki/Stargate), kerbals uncovered
an ancient ring structure near the Desert Pyramids. The origins of this device 
is unknown, and it took quite a few experts to debate and confirm that it was 
in fact not a "skinny donut". Kerbals soon figured out how to powerup the device,
and learned that it can open up wormholes to other Stargates in the galaxy, 
through which kerbals and vessels could instantly travel.

<br><br>

---
## ⨂ This plugin is currently in pre-alpha ⨂
_Do not install this on you main savegame, it contains the bare minimum 
functionality as a proof of concept. Feedback is greatly appreciated, 
please see the bottom of this file._

<br>

---


### How does it work?
- Players get a new "Stargate" part under the _Utilities_ category
- Place this part on a new craft
- Move this craft next to the KSC runway, and place another 
craft (that also contains a stargate) on the KSC Launch Pad (or the Mun)
- Now launch a 3rd craft, a simple rover will suffice
- Drive up to the runway stargate
- Without changing focus, right-click on the stargate
  - The PAW menu allows targeting other stargates
  - Target the other stargate
  - Then click "Open Wormhole"
- While the wormhole is open, drive through the stargate
- The rover will be instantly transported, driving out of the other stargate

### Details

#### New resource - Naquadah (unit: NQ)
- A rare, super-dense mineral (atomic weight of 98.6389) with properties
that amplify energy. Some Konnoisseurs are convinced that it is petrified dark chocolate, because of the bitter taste.
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
- This starts the dialing sequence that takes ~90 seconds
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
- Kerbals can walk through one gate and out the other, rovers can drive through seamlessly, and spacecraft
can drift through one gate and out the other
  - Any wormhole with a surface destination will rotate the kerbal/vessel to have it's feet downwards,
or at least have the same relative orientation at the destination as they had at the origin gate

<br>

---

## Mod Development

### [Developer Readme](https://github.com/Blaarkies/ksp-mod-stargate/blob/main/docs/developer-readme.md)

### New features and bug fixes
This repo's **github issue tracker** is used to organize new features and bugfixes.

Feel free to add feature request there, and as much detail as possible, 
including external links, images, videos, etc.

A lot of the details mentioned [here](#details) does not yet exist, and are planned features
in the issue tracker.

### Pull requests
Help and contributions are welcome (and even encouraged). Pull requests will be 
reviewed and squashed-committed to origin



