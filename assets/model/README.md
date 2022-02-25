# Blender Model Guidelines

## Overview

Each blend file contains a single KSP part. The model objects are organized into:
- [export-me](#Empty_object_-_Export-Me)
- [texture-reference-baking](#Empty_object_-_texture_-_reference_-_baking)

### Empty object: export-me
This is a parent empty that holds the final model objects to be exported to a `.mu` file.

Everything require for a KSP part is contained by this parent:
- Collider mesh objects
- Part objects with setup for shaders, animations

These model objects usually do not display properly in Blender. The texture appears too dark.

### Empty object: texture-reference-baking
A parent empty that holds the parametric version of all the model objects. This provides
the ability to easily modify the objects:
- Change polygon count by setting the amount of sections in an object
- Use shader nodes to generate/modify textures
- Modify animations on these objects
- Edit UV maps

When done, delete the corresponding object from `export-me`, then make a copy of the object
in `texture-reference-baking`. Re-parent this object into `export-me`, where you might have to 
apply any modifier modifiers, and set KSP shader again.

The advantages for doing this by proxy:
- Switching KSP shaders will completely reset the shader node setup done previously
- Allows incremental changes from many users onto the same model, without sacrificing 
"backward compatibility"
- Bump/Normal maps can be baked on separate high-poly objects if needed
- 
