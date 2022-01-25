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

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Types of adjacency for square coordinate grid.
    /// </summary>
    public enum Adjacency
    {
        Orthogonal,
        Full
    }

    /// <summary>
    /// Utility functions for square coordinate grid. 
    /// </summary>
    public static class SquareCoord
    {
        /// <summary>
        /// Up coordinate (0, 1)
        /// </summary>
        public static Vector2Int Up = Vector2Int.up;

        /// <summary>
        /// Right coordinate (0, -1)
        /// </summary>
        public static Vector2Int Down = Vector2Int.down;

        /// <summary>
        /// Left coordinate (-1, 0)
        /// </summary>
        public static Vector2Int Left = Vector2Int.left;

        /// <summary>
        /// Right Coordinate (1, 0)
        /// </summary>
        public static Vector2Int Right = Vector2Int.right;

        /// <summary>
        /// Offset of all orthogonally adjacent tiles enumerated in counter
        /// clockwise order starting with 0 radians at (1, 0).
        /// </summary>
        public static readonly Vector2Int[] orthongoalAdj = new Vector2Int[]
        {
            Right,
            Up,
            Left,
            Down
        };

        /// <summary>
        /// Offset of all full adjacent tiles (orthogonal + diagonals) enumerated in counter
        /// clockwise order starting with 0 radians at (1, 0).
        /// </summary>
        public static readonly Vector2Int[] fullAdj = new Vector2Int[]
        {
            Right,
            Right + Up,
            Up,
            Left + Up,
            Left,
            Left + Down,
            Down,
            Right + Down,
        };
    }
}