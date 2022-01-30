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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using UnityEngine;

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Fixed size square grid tile map that can contain generic values at each position within the map.
    /// </summary>
    /// <typeparam name="V">Type of values contained within each cell in the grid.</typeparam>
    public class SquareTileMap<V> : IBlockableTileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the tile map in squares.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// Height of the tile map in squares.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// Type of adjacency for square tiles in this grid.
        /// </summary>
        private readonly Adjacency adjacencyType;

        /// <summary>
        /// Values stored within each square of the tile map.
        /// </summary>
        private V[,] values;

        /// <summary>
        /// Tiles within the map that are blocked.
        /// </summary>
        private readonly HashSet<Vector2Int> blocked;

        /// <summary>
        /// Initialize a tile map with a given width and height.
        /// </summary>
        /// <param name="width">Width of the tile map in number of squares.</param>
        /// <param name="height">Height of the tile map in number of squares.</param>
        /// <param name="adjacencyType">Type of adjacency within the tile map.</param>
        public SquareTileMap(int width, int height, Adjacency adjacencyType)
        {
            this.width = width;
            this.height = height;
            this.values = new V[width, height];
            this.adjacencyType = adjacencyType;
            this.blocked = new HashSet<Vector2Int>();
        }

        /// <inheritdoc/>
        public V this[Vector2Int loc]
        {
            get => this.values[loc.x, loc.y];
            set => this.values[loc.x, loc.y] = value;
        }

        /// <inheritdoc/>
        public int Degree(Vector2Int loc)
        {
            return this.GetNeighbors(loc).Count();
        }

        /// <inheritdoc/>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int loc)
        {
            switch (this.adjacencyType)
            {
                case Adjacency.Full:
                    return SquareCoord.fullAdj
                        .Select(adj => loc + adj)
                        .Where(loc => this.Contains(loc))
                        .Where(loc => !this.IsBlocked(loc));
                case Adjacency.Orthogonal:
                default:
                    return SquareCoord.orthongoalAdj
                        .Select(adj => loc + adj)
                        .Where(loc => this.Contains(loc))
                        .Where(loc => !this.IsBlocked(loc));
            }
        }

        /// <inheritdoc/>
        public bool Contains(Vector2Int loc)
        {
            return loc.x >= 0 && loc.x < this.width && loc.y >= 0 && loc.y < this.height;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.values = new V[this.width, this.height];
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

        public IEnumerator<Vector2Int> GetLocations()
        {
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        /// <summary>
        /// Enumerates the square locations in the grid in an arbitrary order.
        /// </summary>
        /// <returns>An enumerator of the locations within the square grid.</returns>
        public IEnumerator<Vector2Int> GetEnumerator()
        {
            return this.GetLocations();
        }

        /// <summary>
        /// Enumerates the square locations in the grid in an arbitrary order.
        /// </summary>
        /// <returns>An enumerator of the locations within the square grid.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetLocations();
        }
    }
}
