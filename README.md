# Zone Tools

A Cities: Skylines II mod that gives direct control over road zoning: **both sides**, **left only**, **right only**, or **none**.  
Works for **new roads** and for **updating existing roads**.

This is the older Zone Tools style with a **movable floating panel**.  
For a different UI style with static in-panel icons, see **Easy Zoning**.

## How to use

- Open the Zone Tools panel from the **top-left button** or press **Shift+X** by default
- Drag the panel by its **title bar**
- Pick a zoning mode: **Both / Left / Right / None**

### New roads

- Select a mode, then draw a road
- Vanilla road zoning side still depends on road draw direction, so Zone Tools lets that be chosen up front instead of leaving it to chance

### Update existing roads

- Open the panel
- Enable **Update Road**
- Pick the zoning mode to apply
- **Left-click** roads to apply the selected mode
- Supports **click-and-drag** to update many roads in one pass
- **Right-click** cycles the 4 zoning modes while the Update Road tool is active
- Roads that would actually change get a **hover highlight / blue outline**

### Contour / Topography

- Optional **Contour** button in the Zone Tools panel
- Toggles terrain contour lines for convenience
- Syncs with the vanilla Road Tools **Topography** toggle
- If the Contour button is disabled in Options, contour is still available while **Update Road** is ON

## Options UI Settings

- ✅ Protect occupied cells (buildings)
- ✅ Protect zoned-but-empty cells
- ✅ Show or hide the Contour button
- ✅ Choose panel style:
  - **Glass** = clearer translucent panel
  - **Vanilla gray** = darker vanilla-style panel
- ✅ Change keybind: default **Shift+X** (rebindable in the Options menu)

## Why use it

- Fine control over where buildings will spawn
- Easier placement of nearby roads without unwanted overlapping zoning
- Existing roads can be updated without rebuilding them
- Movable panel can be parked where it is least annoying

## Compatibility

- No Harmony patches
- Uses native CS2 systems
- Safe to remove: cities still load without the mod, and roads keep the zoning edits already applied
- Improved compatibility with other mods
- Should work with Platter

## Languages

English, French, German, Spanish, Italian, Japanese, Korean, Polish, Portuguese (Brazil), Simplified Chinese, Traditional Chinese.

## Credits

- River-mochi — current author and maintainer
- Original ZoningToolKit by zeeshanabid94 (retired)
- Thanks to yenyang for technical support
- Thanks to =Noel= for Chinese locale review

## License

Copyright 2025 River-Mochi

Licensed under the Apache License, Version 2.0 (the "License");  
this file may not be used except in compliance with the License.  
A copy of the License may be obtained at:

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software  
distributed under the License is distributed on an "AS IS" BASIS,  
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  
See the License for the specific language governing permissions and  
limitations under the License.
