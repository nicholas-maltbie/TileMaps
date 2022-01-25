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

using nickmaltbie.TileMap.Common;
using UnityEngine;

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Grid for loading a square grid map into the unity game scene.
    /// </summary>
    public class SquareWorldGrid<V> : IWorldGrid<Vector2Int, V>
    {
        /// <summary>
        /// Size of each tile within the grid.
        /// </summary>
        private float tileSize;

        /// <summary>
        /// Base position for this tile map.
        /// </summary>
        private Transform basePosition;

        /// <summary>
        /// Square tile map for this world grid.
        /// </summary>
        private ITileMap<Vector2Int, V> squareTileMap;

        /// <summary>
        /// Create a square grid with a given tile map.
        /// </summary>
        /// <param name="tileMap">Tile map that this world grid represents.</param>
        /// <param name="tileSize">Size of each tile, the length of each edge in the square.</param>
        /// <param name="basePosition">Base position of the square grid.</param>
        public SquareWorldGrid(ITileMap<Vector2Int, V> tileMap, float tileSize, Transform basePosition)
        {
            this.squareTileMap = tileMap;
            this.basePosition = basePosition;
            this.tileSize = tileSize;
        }

        /// <inheritdoc/>
        public ITileMap<Vector2Int, V> GetTileMap() => this.squareTileMap;

        /// <inheritdoc/>
        public Vector3 GetWorldPosition(Vector2Int loc) =>
            this.basePosition.position + this.basePosition.TransformVector(new Vector3(loc.x, 0, loc.y) * this.tileSize);

        /// <inheritdoc/>
        public Quaternion GetWorldRotation(Vector2Int loc) => this.basePosition.rotation;
    }
}