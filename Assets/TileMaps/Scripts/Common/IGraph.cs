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
    /// A generic graph which represents a collection of connected vertices.
    /// </summary>
    /// <typeparam name="V">Types of vertices within the graph.</typeparam>
    public interface IGraph<V> : IEnumerable<V>
    {
        /// <summary>
        /// Get the neighbors of a given vertex within the graph.
        /// </summary>
        /// <param name="vertex">Vertex on the map to find neighbors of.</param>
        /// <returns>An enumerable list of all the neighbors of a given vertex.</returns>
        IEnumerable<V> GetNeighbors(V vertex);

        /// <summary>
        /// Gets the number of neighbors (or degree) of a vertex within the graph.
        /// </summary>
        /// <param name="vertex">Vertex on the map to compute neighbors for.</param>
        /// <returns>The number of neighbors that a given vertex has.</returns>
        int Degree(V vertex);

        /// <summary>
        /// Check if a given vertex is contained within the graph.
        /// </summary>
        /// <param name="vertex">Vertex to check if it is within bounds.</param>
        /// <returns>True if the vertex is in the graph, false otherwise.</returns>
        bool Contains(V vertex);

        /// <summary>
        /// Clear all vertices from the tile graph.
        /// </summary>
        void Clear();
    }
}
