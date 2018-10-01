# StarMap
Rendering of stars in the night sky using the HYG database http://astronexus.com/node/34

![unity star map chart rendering](http://i.imgur.com/rEFGI4Sl.png)

It uses a geometry shader, or a baked billboard mesh to render thousands of stars as billboard sprites at little cost. You can use it as a background in your space or sailing game or.. anything else.

## Features
* Automatically creates a renderes on loading the scene, so the star data can be very small.
* If using geometry shader rendering, stars will be resized according to the camera's field of view, so when zooming in you will not see a white blob across your entire screen.
* Zooming in also does not result in seeing awful compression artifacts or pixels (as would if you were using a skybox)
* Realistic star colors depending on the star's temperature
* Since geometry shaders are not supported on all platforms, a static billboard mesh renderer is provided too.
* Mesh rendering method has an option for cubic splitting, which divides the stars into 6 separate meshes. This improves performance since only those sides that are visible are rendered (frustum culling).
* Option to cull all stars below the horizon, if you are making a night game on ground, without dynamic time of day.

## Requirements
You need to download hygdata_v3.csv from https://github.com/astronexus/HYG-Database, and put it in the Project folder.

## How to use
### 1: Building StarData asset
First you need to build a StarData asset. This asset will hold the data about the stars picked from the HYG database, depending on the star magnitude limit (basically, the number of stars), without the need to read the HYG database text every time. Additionally, when the game is built, this data will be packed within your game, and loaded much faster than the entire database.
1. To create the StarData asset, right click anywhere in any folder inside your project and select `Create > StarData`.
2. When you select this asset, click the 'Generate Data' button, it will pick all stars from the HYG database lower than the magnitude limit chosen. The magnitude, is the brightness of the stars and the higher the number, the more stars will be included.
3. When the generation is complete, you will be shown how many stars has been picked in the inspector.

### 2: Chose your rendering method
To render the stars, one of the rendering components needs to be added to the scene:
* StarmapGSRenderer renders the stars with a geometry shader as billboards for each vertex. The benefit of a geometry shader is that it requires less memory, and it can be dinamically resized on the GPU. Although it might not be supported on some platforms. It has not been tested on SRP either.
To use it, attach the Starmapis that the  Attach the StarGS material to it, and once you start the game you should see the stars. All the parameters are configured on the material inspectorlike size and magnitude can currently only be configured inside the GSBillboardFacing shader. 
* StarmapMeshRenderer renders the stars as a mesh, with quads - permanently pointing towards the center. All parameters like magnitude and size can be setup within the component, and have tooltips (if you hover over parameters), which explain what each parameter does. It is recommended to use a Particle additive shader, although, you can use any custom shader that supports vertex color.

## Additional notes
* The StarmapMeshRenderer has an ability to split the stars into 6 sides, 

## Notes
The project is still WIP, not many things are exposed (you need to dig in the code to change stuff), and it's currently hardcoded to only accept hygdata_v3.csv. 

Here is a few things I am working on for future updates:
* Exposed variables
* Billboard resize according to camera's field of view
* Realistic exposure-relative brightness
* Star colors
