# StarMap
Rendering of stars in the night sky using the HYG database http://astronexus.com/node/34

![unity star map chart rendering](http://i.imgur.com/rEFGI4Sl.png)

It uses a geometry shader to render thousands of stars as billboard sprites at little cost. You can use it as a background in your space or sailing game or.. anything else.

## Requirements
You need to download hygdata_v3.csv from https://github.com/astronexus/HYG-Database, and put it in the Project folder.

## Notes
The project is still WIP, not many things are exposed (you need to dig in the code to change stuff), and it's currently hardcoded to only accept hygdata_v3.csv. 

Here is a few things I am working on for future updates:
* Exposed variables
* Billboard resize according to camera's field of view
* Realistic exposure-relative brightness
* Star colors
