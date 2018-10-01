# StarMap
Rendering of stars in the night sky using the HYG database http://astronexus.com/node/34

![unity star map chart rendering](http://i.imgur.com/rEFGI4Sl.png)

It uses a geometry shader to render thousands of stars as billboard sprites at little cost. You can use it as a background in your space or sailing game or.. anything else.

## Requirements
You need to download hygdata_v3.csv from https://github.com/astronexus/HYG-Database, and put it in the Project folder.

## How to use
1. First you need to build a StarData asset. This asset will hold the data about the stars used in the game without the need to read the HYG database text every time. Additionally, when the game is built, this data will be packed within your game. 
* To create the StarData asset, right click anywhere in any folder inside your project and select `Create > StarData`.
* When you select this asset, click the 'Generate Data' button, it will pick all stars from the HYG database lower than the magnitude limit chosen.
* When the stars have been generated you will be shown how many stars have been found.

2. Chose your rendering method
To render the stars, add one of the renders in the scene and assign your StarData to it.
* StarmapGSRenderer renders the stars with a geometry shader as billboards for each vertex. Attach the StarGS material to it, and once you start the game you should see the stars. All the parameters, like size and magnitude can currently only be configured inside the GSBillboardFacing shader. This is the prefered way of rendering stars, although it might not be supported on some platforms. It has not been tested on SRP.
* StarmapMeshRenderer renders the stars as a mesh, with quads - permanently pointing towards the center. All parameters like magnitude and size can be setup within the component.


## Notes
The project is still WIP, not many things are exposed (you need to dig in the code to change stuff), and it's currently hardcoded to only accept hygdata_v3.csv. 

Here is a few things I am working on for future updates:
* Exposed variables
* Billboard resize according to camera's field of view
* Realistic exposure-relative brightness
* Star colors
