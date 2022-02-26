# Tile Maps

This project will generate various types of tile maps including square and hexagon maps and allow for spawning in game
objects based off of these patterns. Includes various features such as path finding.

[![Sample pathfinding demo with astar.](Demo/sample-pathfinding.gif)](https://youtu.be/1OOXEmKqmxc)

The Tile Maps project is hosted on GitHub at
[https://github.com/nicholas-maltbie/TileMaps](https://github.com/nicholas-maltbie/TileMaps)

An interactive version demonstrating the TileMaps project is hosted at
[https://nickmaltbie.com/TileMaps/](https://nickmaltbie.com/TileMaps/).

Series of videos describing how pathfinding works
* [How Pathfinding Works - Introduction](https://youtu.be/1OOXEmKqmxc)

# Development

If you want to help with the project, feel free to make some changes and submit a PR to the repo.

This library was developed as part of the Falling Parkour Project here -
[https://github.com/nicholas-maltbie/FallingParkour](https://github.com/nicholas-maltbie/FallingParkour)

This project is developed using Unity Release [2021.1.19f1](https://unity3d.com/unity/whats-new/2021.1.19). Install this
version of Unity from Unity Hub using this unity hub link unityhub://2021.1.19f1/d0d1bb862f9d.

When working with the project, make sure to setup the `.githooks` if you want to edit the code in the project. In order to
do this, use the following command to reconfigure the `core.hooksPath` for your repository 

```
git config --local core.hooksPath .githooks
```

# Documentation

Documentation on the project and scripting API is found at
[https://nickmaltbie.com/TileMaps/docs/](https://nickmaltbie.com/TileMaps/docs/) for the latest version of the codebase.

To view the documentation from a local build of the project install [DocFX](https://dotnet.github.io/docfx/), use the
following command from the root of the repo.
```
docfx Documentation/docfx.json --serve
```

The documentation for the project is stored in the folder `/Documentation` and can be modified and changed to update
with the project.

_This documentation project is inspired by the project by Norman Erwan's
[DocFxForUnity](https://github.com/NormandErwan/DocFxForUnity)_

# License

This is an open source project licensed under a [MIT License](LICENSE.txt). Feel free to use a build of the project for
your own work. If you see an error in the project or have any suggestions, write an issue or make a pull request, I'll
happy include any suggestions or ideas into the project.
