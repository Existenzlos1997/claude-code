# Free Aircraft Models for Game Development

This document provides a curated list of free aircraft models (Flugzeugmodelle) available for use in game development and other projects. All models listed here are from reputable sources and available at no cost, though some may have specific licensing requirements.

## Table of Contents
- [3D Model Repositories](#3d-model-repositories)
- [Open Source Aircraft Projects](#open-source-aircraft-projects)
- [Free Models by Category](#free-models-by-category)
- [License Information](#license-information)
- [Usage Guidelines](#usage-guidelines)

---

## 3D Model Repositories

### Sketchfab
**Website:** https://sketchfab.com/
**Free Models:** https://sketchfab.com/search?features=downloadable&licenses=322a749bcfa841b29dff1e8a1bb74b0b&licenses=b9ddc40b93e34cdca1fc152f39b9f375&licenses=72360ff1740d419791934298b8b6d270&licenses=bbfe3f7dbcdd4122b966b85b9786a989&q=aircraft&sort_by=-pertinence&type=models

**Description:** Community platform with thousands of free downloadable aircraft models
**License Types:** CC0, CC-BY, CC-BY-SA
**Popular Models:**
- Commercial aircraft (Boeing 737, Airbus A320, etc.)
- Military jets (F-16, MiG-29, Su-27)
- General aviation aircraft
- Helicopters

### Free3D
**Website:** https://free3d.com/
**Aircraft Section:** https://free3d.com/3d-models/aircraft

**Description:** Large collection of free 3D models
**Formats:** .obj, .fbx, .blend, .3ds, .max
**Popular Categories:**
- Commercial airliners
- Fighter jets
- Helicopters
- Historical aircraft

### TurboSquid (Free Section)
**Website:** https://www.turbosquid.com/
**Free Aircraft:** https://www.turbosquid.com/Search/3D-Models/free/aircraft

**Description:** Professional 3D marketplace with free section
**Formats:** Various (check individual models)
**Quality:** Variable, some professional-grade models available

### CGTrader (Free Models)
**Website:** https://www.cgtrader.com/
**Free Aircraft:** https://www.cgtrader.com/free-3d-models/aircraft

**Description:** Marketplace with both free and paid models
**Formats:** .fbx, .obj, .blend, .max, .c4d
**Notable:** Often includes PBR textures

### Poly Pizza (formerly Google Poly)
**Website:** https://poly.pizza/
**Search:** https://poly.pizza/search?q=aircraft

**Description:** Archive of Google Poly models
**License:** Mostly CC-BY
**Formats:** .gltf, .fbx, .obj
**Note:** Low-poly models, great for mobile/web games

---

## Open Source Aircraft Projects

### FlightGear Aircraft
**Website:** https://www.flightgear.org/
**Aircraft Directory:** https://wiki.flightgear.org/Category:Aircraft
**Download:** https://github.com/FlightGear-Contrib

**Description:** Open-source flight simulator with hundreds of aircraft models
**License:** GPL and various open-source licenses
**Formats:** .xml, .ac (can be converted)
**Models Include:**
- Cessna 172
- Boeing 747-400
- Airbus A320
- Various military aircraft
- Helicopters and gliders

### OpenVSP Models
**Website:** https://openvsp.org/
**GitHub:** https://github.com/OpenVSP/OpenVSP

**Description:** Parametric aircraft geometry tool with sample models
**License:** NASA Open Source Agreement
**Use Case:** More technical/engineering focused, but models can be exported

---

## Free Models by Category

### Commercial Aircraft

1. **Boeing 737**
   - Source: Sketchfab, Free3D
   - License: Various (check individual models)
   - Download: Search on respective platforms

2. **Airbus A320**
   - Source: Sketchfab, CGTrader
   - License: CC-BY or similar
   - Download: Search on respective platforms

3. **Boeing 747**
   - Source: Free3D, TurboSquid free section
   - License: Varies
   - Download: Search on respective platforms

### Military Aircraft

1. **F-16 Fighting Falcon**
   - Source: Sketchfab, Free3D
   - License: CC-BY recommended
   - Download: https://sketchfab.com/search?q=f-16&type=models

2. **MiG-29**
   - Source: Free3D, CGTrader
   - License: Varies
   - Download: Search on respective platforms

3. **F-22 Raptor**
   - Source: Sketchfab, TurboSquid
   - License: CC-BY or similar
   - Download: Search on respective platforms

### General Aviation

1. **Cessna 172**
   - Source: FlightGear, Sketchfab
   - License: GPL/CC-BY
   - Download: FlightGear aircraft repository

2. **Piper Cub**
   - Source: Free3D, Poly Pizza
   - License: CC-BY
   - Download: Search on respective platforms

### Helicopters

1. **Bell UH-1 (Huey)**
   - Source: Sketchfab, Free3D
   - License: CC-BY
   - Download: Search on respective platforms

2. **Apache Helicopter**
   - Source: Free3D, CGTrader
   - License: Varies
   - Download: Search on respective platforms

### Historical Aircraft

1. **Spitfire**
   - Source: Sketchfab, Free3D
   - License: CC-BY
   - Download: Search on respective platforms

2. **P-51 Mustang**
   - Source: Sketchfab, FlightGear
   - License: CC-BY/GPL
   - Download: Search on respective platforms

---

## License Information

When using free 3D models, always check and comply with the license:

### Common License Types

1. **CC0 (Public Domain)**
   - ✅ Commercial use allowed
   - ✅ No attribution required
   - ✅ Can modify freely

2. **CC-BY (Creative Commons Attribution)**
   - ✅ Commercial use allowed
   - ⚠️ Attribution required
   - ✅ Can modify freely

3. **CC-BY-SA (Share Alike)**
   - ✅ Commercial use allowed
   - ⚠️ Attribution required
   - ⚠️ Derivatives must use same license

4. **GPL (General Public License)**
   - ✅ Free to use and modify
   - ⚠️ Source code/model must be made available
   - ⚠️ Derivatives must use GPL

### Important Notes
- Always read the specific license for each model
- Some free models are only for personal/non-commercial use
- Attribution requirements vary by license
- Some models may require you to share your modifications

---

## Usage Guidelines

### Before Using a Model

1. **Check the License**
   - Read and understand the license terms
   - Note any attribution requirements
   - Verify commercial use is permitted (if applicable)

2. **Verify File Format**
   - Ensure the format is compatible with your game engine
   - Common game formats: .fbx, .obj, .gltf, .blend

3. **Check Polygon Count**
   - High-poly models may need optimization for games
   - Consider LOD (Level of Detail) requirements

### Model Preparation

1. **Optimization**
   - Reduce polygon count if needed
   - Optimize textures for game performance
   - Create LOD versions if necessary

2. **Texturing**
   - Check if textures are included
   - May need to create/modify materials for your engine
   - Consider PBR workflow compatibility

3. **Rigging**
   - Check if model is rigged (if animation needed)
   - May need to add armature/bones
   - Verify compatibility with your animation system

### Attribution Template

If the license requires attribution, use this template:

```
Aircraft Model: [Model Name]
Author: [Creator Name]
Source: [URL]
License: [License Type]
Modifications: [List any changes made]
```

---

## Additional Resources

### Texture Resources
- **Textures.com** - Free textures (with account): https://www.textures.com/
- **Poly Haven** - Free PBR textures: https://polyhaven.com/textures
- **CC0 Textures** - Public domain textures: https://cc0textures.com/

### Model Conversion Tools
- **Blender** (Free): https://www.blender.org/
  - Can convert between most 3D formats
  - Free modeling and optimization tools
  
- **FBX Converter** (Free): https://www.autodesk.com/developer-network/platform-technologies/fbx-converter-archives

### Game Engine Documentation
- **Unity 3D Models**: https://docs.unity3d.com/Manual/3D-formats.html
- **Unreal Engine**: https://docs.unrealengine.com/en-US/WorkingWithContent/Importing/
- **Godot**: https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/

---

## Quick Start Checklist

- [ ] Choose your aircraft models from the repositories above
- [ ] Download models in compatible format
- [ ] Check and comply with license requirements
- [ ] Import into Blender or your game engine
- [ ] Optimize polygon count if needed
- [ ] Set up materials and textures
- [ ] Test in your game environment
- [ ] Add attribution if required by license

---

## Disclaimer

This document is provided as a resource guide. Always verify:
- License terms directly from the source
- Model quality and compatibility with your project
- Legal compliance for your specific use case

The availability and licensing of models may change over time. Always check the current terms before using any model in your project.

---

**Last Updated:** December 2025

**Contributions:** To suggest additional resources or corrections, please open an issue or pull request in this repository.
