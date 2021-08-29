using System.Collections.Generic;
using TileMap.Common;
using UnityEngine;

namespace TileMap.Square
{
    /// <summary>
    /// Fixed size square grid tile map that can contain generic values at each position within the map.
    /// </summary>
    /// <typeparam name="V">Type of values contained within each cell in the grid.</typeparam>
    public class SquareTileMap<V> : ITileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the tile map in squares.
        /// </summary>
        private int width;
        
        /// <summary>
        /// Height of the tile map in squares.
        /// </summary>
        private int height;

        /// <summary>
        /// Values stored within each square of the tile map
        /// </summary>
        private V[,] values;
        
        /// <summary>
        /// Initialize a tile map with a given width and height.
        /// </summary>
        /// <param name="width">Width of </param>
        /// <param name="height"></param>
        public SquareTileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <inheritdoc/>
        public V this[Vector2Int loc]
        {
            get => values[loc.x, loc.y];
            set => values[loc.x, loc.y] = value;
        }

        /// <inheritdoc/>
        public int GetNeighborCount(Vector2Int loc)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int loc)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsInMap(Vector2Int loc)
        {
            throw new System.NotImplementedException();
        }
    }
}
