## stargate milkyway.blend

### 2022.03.21
- Change `texture-reference-baking`
  - Animations 
    - Add `RingSpin`, `Kawoosh`, `EventHorizon` animations
    - Add `ChevronLight1`-`ChevronLight6` animations
    - Add `ChevronOrigin`(...) animations
  - Geometry
    - Add `chevron-bools` to describe chevron decorations (the non-moving parts)
    - Add `chevron-origin` that has 2 separate moving parts + lights
    - Add `chevron-other` that generates 9 base chevrons around the gate
      - These have no moving parts, and only singular lights
      - Remember to delete the base chevron generated over the origin when converting to mesh
  - Materials
    - Add many materials for debugging, please replace with proper material setups
    - Keep the `texture-reference-baking` materials separate from the `export-me` materials
      - Changing a KSP shader overwrites anything in the currently selected material
- Add `export-me` parent object
  - Copy `texture-reference-baking` objects over to `export-me` and convert to mesh (finalize modifiers)
  - Setup some basic KSP shader for debugging the model in game

### 2022.02.23
- Setup parent empties
- Add `texture-reference-baking` parent object
  - Add `body` with Screw modifier
  - Add `ring-glyphs` with Screw modifier

