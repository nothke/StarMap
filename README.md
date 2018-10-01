# StarMap
Rendering of stars in the night sky using the HYG database http://astronexus.com/node/34

![unity star map chart rendering](http://i.imgur.com/rEFGI4Sl.png)

It uses a geometry shader (or optionally a baked billboard mesh) to render thousands of stars as billboard sprites at little cost. You can use it as a background in your space or sailing game or.. anything else.

## Features
* Automatically creates renderers on loading the scene, so the star data can be very small.
* Star's size is based on magnitude and can be limited according to the camera's field of view. When zooming in you will not see a white blob across your entire screen, nor will the stars completely disappear if the fov is too high. This only works if using geometry shader rendering.
* Zooming in also does not result in seeing awful compression artifacts or pixels, as would if you were using a skybox.
* Realistic star colors depending on the star's temperature.
* Since geometry shaders are not supported on all platforms, a static billboard mesh renderer is provided too.
* Mesh rendering method has an option for cubic splitting, which divides the star field into 6 separate meshes. This improves performance since only those sides that are visible are rendered (frustum culling).
* Option to cull all stars below the horizon, for example if you are making a night game on ground, without dynamic time of day.

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
#### StarmapGSRenderer
The StarmapGSRenderer renders the stars with a geometry shader as billboards for each vertex. The benefit of a geometry shader is that it requires less memory, and it can be dinamically resized on the GPU. Although it might not be supported on some platforms. It has not been tested on SRP either.
#### StarmapMeshRenderer 
The StarmapMeshRenerer renders the stars as a non-dynamic mesh, with quads - permanently pointing towards the center. All parameters like magnitude and size can be setup within the component, and have tooltips (if you hover over parameters), which explain what each parameter does. It is recommended to use a Particle additive shader material, although, you can use any custom shader that supports vertex color.

### 3: Now add your renderer to the scene
1. Add your chosen renderer to a GameObject centered at 0,0,0.
2. Add the StarData you made in step 1.
3. Assign the material
  * For StarmapGSRenderer, you need to use the material with one of the Geometry/Star shaders. It is recommended to use the 'Star - Screen Aware' shader as it will dynamically resize with the field of view.
  * For StarmapMeshRenderer, you should use the particle additive shader, or a custom transparent additive shader that has a vertex color support
4. The distance parameter is how far from the center the stars will be rendered at. Note that they will not be rendered if they are beyond the cameras far plane.
5. Edit parameters
  * For the StarmapGSRenderer, most parameters are in the material inspector.
  * For StarmapMeshRenderer, parameters are in the inspector.
6. When you start the game, the stars should now be generated!

## Notes
The project is currently hardcoded to only accept hygdata_v3.csv, but see the Starmap class to plug your own custom data.
To make the stars always be visible 'in infinity' a good solution is to make a script that will make the transform of the starmap follow the camera all the time.
