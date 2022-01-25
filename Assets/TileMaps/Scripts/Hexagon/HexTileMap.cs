﻿// Copyright (C) 2022 Nicholas Maltbie
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

using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using UnityEngine;

namespace nickmaltbie.TileMap.Hexagon
{

    /// <summary>
    /// Fixed size hexagon grid tile map that can contain generic values at each position within the map.
    /// </summary>
    /// <typeparam name="V">Type of values contained within each cell in the grid.</typeparam>
    public class HexTileMap<V> : IBlockableTileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the hexagon grid in tiles.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// Height of the hexagon grid in tiles.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// Values stored at each location in the hexagon grid.
        /// </summary>
        private V[,] values;

        /// <summary>
        /// Tiles within the map that are blocked.
        /// </summary>
        private readonly HashSet<Vector2Int> blocked;

        /// <summary>
        /// Create a fixed size hexagon grid with a given width and height.
        /// </summary>
        /// <param name="width">Width of the hexagon grid.</param>
        /// <param name="height">Height of the hexagon grid.</param>
        public HexTileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.values = new V[width, height];
            this.blocked = new HashSet<Vector2Int>();
        }

        /// <inheritdoc/>
        public V this[Vector2Int loc]
        {
            get => this.values[loc.x, loc.y];
            set => this.values[loc.x, loc.y] = value;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.values = new V[this.width, this.height];
        }

        /// <inheritdoc/>
        public IEnumerator<Vector2Int> GetEnumerator()
        {
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        /// <inheritdoc/>
        public int GetNeighborCount(Vector2Int loc)
        {
            return this.GetNeighbors(loc).Count();
        }

        /// <inheritdoc/>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int loc)
        {
            return HexCoord.GetAdjacent(loc)
                .Where(loc => this.IsInMap(loc))
                .Where(loc => !this.IsBlocked(loc));
        }

        /// <inheritdoc/>
        public bool IsInMap(Vector2Int loc)
        {
            return loc.x >= 0 && loc.x < this.width && loc.y >= 0 && loc.y < this.height;
        }

        /// <inheritdoc/>
        public void Block(Vector2Int loc)
        {
            this.blocked.Add(loc);
        }

        /// <inheritdoc/>
        public void Unblock(Vector2Int loc)
        {
            this.blocked.Remove(loc);
        }

        /// <inheritdoc/>
        public bool IsBlocked(Vector2Int loc)
        {
            return this.blocked.Contains(loc);
        }
    }
}
