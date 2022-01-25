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
using nickmaltbie.TileMap.Square;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Example square grid of spawned prefabs for testing purposes.
    /// </summary>
    public class ExampleSquareGrid : AbstractExampleGrid
    {
        /// <summary>
        /// Width of the tile map in number of squares.
        /// </summary>
        [Tooltip("Width of the tile map in number of squares.")]
        [SerializeField]
        private int width = 8;

        /// <summary>
        /// Height of the tile map in number of squares.
        /// </summary>
        [Tooltip("Height of the tile map in number of squares.")]
        [SerializeField]
        private int height = 8;

        /// <summary>
        /// Size (distance) between spawned tiles in the world grid.
        /// </summary>
        [Tooltip("Size (distance) between spawned tiles in the world grid.")]
        [SerializeField]
        private float tileSize = 1.0f;

        /// <summary>
        /// Adjacency type used for this square grid.
        /// </summary>
        [Tooltip("Adjacency type for square grid.")]
        [SerializeField]
        private Adjacency adjacency = Adjacency.Orthogonal;

        protected override (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap()
        {
            IBlockableTileMap<Vector2Int, GameObject> tileMap = new SquareTileMap<GameObject>(
                width, height, adjacency);

            return (
                new SquareWorldGrid<GameObject>(tileMap, tileSize, base.transform),
                tileMap
            );
        }
    }
}
