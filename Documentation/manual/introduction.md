# Introduction to Tile Maps

The Tile Maps project is a basic example and proof of concept to create various types of tile maps that can be
manipulated in a generic manner. A Tile Map in this project refers to a collection of objects that can be collected,
referenced, and searched in some organized manner. 

The basic assumption of any tile map is that it saves values based on a given key lookup and that each key will have a
set of neighbors that are adjacent to it. An example of this would be a square grid where each cell in the grid has a
coordinate linked to it and neighbors defined by the squares sharing a side.

This project by default includes:
* Square Grids
* Hexagon Grids
* Generic Path Finding
* Animated Path Finding Demo

## Organization of Project

The project is organized into a few namespaces:
* `nickmaltbie.TileMaps.Common` for common code for managing any tile map
* `nickmaltbie.TileMaps.Example` for example code used in the demo
* `nickmaltbie.TileMaps.Hexagon` for hexagon based tile maps
* `nickmatlbie.TileMaps.Pathfinding` for any path finding code
* `nickmaltbie.TileMaps.Square` for square based tile maps.
