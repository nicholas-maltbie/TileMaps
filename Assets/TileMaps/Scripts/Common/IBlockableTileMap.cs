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

namespace nickmaltbie.TileMaps.Common
{
    /// <summary>
    /// Tile map that supports blocking spaces within the map. Blocked locations are still within the map but can be
    /// ignored when calculating neighbors of a tile as they do not show up in the neighbors of a tile.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface IBlockableTileMap<K, V> : ITileMap<K, V>
    {
        /// <summary>
        /// Block a given location within the tile map.
        /// </summary>
        /// <param name="loc">Location within the tile map to block.</param>
        void Block(K loc);

        /// <summary>
        /// Unblocks state of a tile within the map.
        /// </summary>
        /// <param name="loc">Location within the tile map to unblock.</param>
        void Unblock(K loc);

        /// <summary>
        /// Check if a location within the tile map is blocked.
        /// </summary>
        /// <param name="loc">Location within the tile map to check.</param>
        /// <returns>True if the tile is blocked, false otherwise.</returns>
        bool IsBlocked(K loc);

        /// <summary>
        /// Reset any blocked tile in the grid.
        /// </summary>
        void ResetBlocks();
    }
}
