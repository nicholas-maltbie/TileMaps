// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;

namespace nickmaltbie.TileMaps.Common
{
    /// <summary>
    /// World grid to control and load a TileMap into world space.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface IWorldGrid<K, V>
    {
        /// <summary>
        /// Gets the tile map associated with this world grid.
        /// </summary>
        /// <returns></returns>
        ITileMap<K, V> GetTileMap();

        /// <summary>
        /// Gets the world position of a given coordinate within the tile map.
        /// </summary>
        /// <param name="loc">Location within the tile map to lookup.</param>
        /// <returns>Position in world space of this location in the tile map.</returns>
        Vector3 GetWorldPosition(K loc);

        /// <summary>
        /// Gets the world rotation of a given coordinate within the tile map.
        /// </summary>
        /// <param name="loc">Rotation within the tile map to lookup.</param>
        /// <returns>Rotation in world space of this location in the tile map.</returns>
        Quaternion GetWorldRotation(K loc);
    }
}
