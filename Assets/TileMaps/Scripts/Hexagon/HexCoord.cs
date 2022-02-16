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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nickmaltbie.TileMaps.Hexagon
{
    /// <summary>
    /// Directions relative to a hexagon. These are expressed as degrees relative
    /// to zero where zero is directly to the right. This is assuming a hexagon grid is
    /// stacked evenly in an offset grid where moving at a different multiples of 60 degrees
    /// (in a counter clockwise direction at angles of 0, 60, 120, 180, 240, 300)
    /// will lead directly towards another hexagon. Each of
    /// the hexagon directions has a numeric value of the degrees of turning to
    /// move in that direction.
    /// </summary>
    public enum HexDir
    {
        Right = 0,
        TopRight = 60,
        TopLeft = 120,
        Left = 180,
        BottomRight = 240,
        BottomLeft = 300,
    }

    /// <summary>
    /// Utility functions for hexagon coordinate grid. This set of coordinates assumes that a row is
    /// represented in the y coordinate of a hexagon and the column is represented in the x coordinate of a hexagon.
    /// Moving in the positive x direction will increase column and moving in the positive y direction will increase the
    /// row value.
    /// </summary>
    public static class HexCoord
    {
        /// <summary> Change in coordinate of a hexagon to the right of a location. </summary>
        private static readonly Vector2Int offsetRight = new Vector2Int(1, 0);

        /// <summary> Change in coordinate of a hexagon to the left of a location. </summary>
        private static readonly Vector2Int offsetLeft = new Vector2Int(-1, 0);

        /// <summary> Change in coordinate of a hexagon to the top right of a location in an even row. </summary>
        private static readonly Vector2Int offsetTopRightEven = new Vector2Int(0, 1);

        /// <summary> Change in coordinate of a hexagon to the top right of a location in an odd row. </summary>
        private static readonly Vector2Int offsetTopRightOdd = new Vector2Int(1, 1);

        /// <summary> Change in coordinate of a hexagon to the top left of a location in an even row. </summary>
        private static readonly Vector2Int offsetTopLeftEven = new Vector2Int(-1, 1);

        /// <summary> Change in coordinate of a hexagon to the top left of a location in an odd row. </summary>
        private static readonly Vector2Int offsetTopLeftOdd = new Vector2Int(0, 1);

        /// <summary> Change in coordinate of a hexagon to the bottom right of a location in an even row. </summary>
        private static readonly Vector2Int offsetBottomRightEven = new Vector2Int(0, -1);

        /// <summary> Change in coordinate of a hexagon to the bottom right of a location in an odd row. </summary>
        private static readonly Vector2Int offsetBottomRightOdd = new Vector2Int(1, -1);

        /// <summary> Change in coordinate of a hexagon to the bottom left of a location in an even row. </summary>
        private static readonly Vector2Int offsetBottomLeftEven = new Vector2Int(-1, -1);

        /// <summary> Change in coordinate of a hexagon to the bottom left of a location in an odd row. </summary>
        private static readonly Vector2Int offsetBottomLeftOdd = new Vector2Int(0, -1);

        /// <summary>
        /// Array of all hexagon directions in counter clockwise order starting with right
        /// </summary>
        private static readonly HexDir[] adjacentDir = {
            HexDir.Right,
            HexDir.TopRight,
            HexDir.TopLeft,
            HexDir.Left,
            HexDir.BottomLeft,
            HexDir.BottomRight
        };

        /// <summary>
        /// Get the coordinates of hexagons adjacent to a given cordinate in a grid.
        /// </summary>
        /// <param name="loc">Location within the hexagon grid.</param>
        /// <returns>Enumerable of adjacent coordinates to a given hexagon location in a grid.</returns>
        public static IEnumerable<Vector2Int> GetAdjacent(Vector2Int loc) =>
            Enumerable.Range(0, adjacentDir.Length).Select(index => GetAdjacentDir(loc, adjacentDir[index]));

        /// <summary>
        /// Gets the coordinate of a hexagon that is a given direction from a specific hexagon.
        /// </summary>
        /// <param name="loc">Location of hexagon within the grid.</param>
        /// <param name="dir">Direction relative to hexagon.</param>
        /// <returns>Coordinate of hexagon that is the given direction from the provided location.</returns>
        public static Vector2Int GetAdjacentDir(Vector2Int loc, HexDir dir)
        {
            bool evenRow = loc.y % 2 == 0;
            switch (dir)
            {
                case HexDir.Right:
                    return loc + offsetRight;
                case HexDir.Left:
                    return loc + offsetLeft;
                case HexDir.TopRight:
                    return loc + (evenRow ? offsetTopRightEven : offsetTopRightOdd);
                case HexDir.TopLeft:
                    return loc + (evenRow ? offsetTopLeftEven : offsetTopLeftOdd);
                case HexDir.BottomRight:
                    return loc + (evenRow ? offsetBottomRightEven : offsetBottomRightOdd);
                case HexDir.BottomLeft:
                    return loc + (evenRow ? offsetBottomLeftEven : offsetBottomLeftOdd);
            }

            throw new ArgumentException($"Invalid hexagon direction \"{dir}\" provided");
        }
    }
}
