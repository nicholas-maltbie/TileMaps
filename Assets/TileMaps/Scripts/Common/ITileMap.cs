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

using System.Collections.Generic;

namespace nickmaltbie.TileMap.Common
{
    /// <summary>
    /// A tile map for a given coordinate system that contains specific values.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface ITileMap<K, V>
    {
        /// <summary>
        /// Get the neighbors of a given location on the map
        /// </summary>
        /// <param name="loc">Location on the map to find neighbors of.</param>
        /// <returns>The locations of neighbors on the map</returns>
        IEnumerable<K> GetNeighbors(K loc);

        /// <summary>
        /// Get the number of neighbors at a given location in the map.
        /// </summary>
        /// <param name="loc">Location on the map to compute neighbors for.</param>
        /// <returns>The number of neighbors that a given tile has.</returns>
        int GetNeighborCount(K loc);

        /// <summary>
        /// Check if a given location is within the bounds of the map.
        /// </summary>
        /// <param name="loc">Location to check if it is within bounds.</param>
        /// <returns>True if the location is in the map, false otherwise.</returns>
        bool IsInMap(K loc);

        /// <summary>
        /// Modify the value saved at a specific location within the map.
        /// </summary>
        V this[K loc]
        {
            get;
            set;
        }

        /// <summary>
        /// Clear all values from the tile grid.
        /// </summary>
        void Clear();

        /// <summary>
        /// Get an enumerator of the key elements in this tile map.
        /// </summary>
        IEnumerator<K> GetEnumerator();
    }
}
