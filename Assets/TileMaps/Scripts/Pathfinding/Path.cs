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
using System.Linq;

namespace nickmaltbie.TileMap.Pathfinding
{
    /// <summary>
    /// Represent a path via recursive definition for finding a path between nodes in a graph.
    /// </summary>
    /// <typeparam name="V">Type of nodes stored in the path</typeparam>
    public class Path<V>
    {
        /// <summary>
        /// Previous path.
        /// </summary>
        private readonly Path<V> previous;

        /// <summary>
        /// node stored at this location in the path.
        /// </summary>
        private readonly V node;

        /// <summary>
        /// Get the node stored at this step in the path.
        /// </summary>
        public V Node => this.node;

        /// <summary>
        /// Get the path previous to this node.
        /// </summary>
        public Path<V> Previous => this.previous;

        /// <summary>
        /// Create a path that consists of just a single node that has no previous path.
        /// </summary>
        /// <param name="node">Single node within the path.</param>
        public Path(V node)
        {
            this.previous = null;
            this.node = node;
        }

        /// <summary>
        /// Create a path for a current node and with provided previous path.
        /// </summary>
        /// <param name="node">node stored at this step in the path.</param>
        /// <param name="previous">Previous nodes in the path.</param>
        public Path(V node, Path<V> previous)
        {
            this.previous = previous;
            this.node = node;
        }

        /// <summary>
        /// Get the full length of the path.
        /// </summary>
        public int Length()
        {
            int length = 1;
            Path<V> previous = this.previous;
            while (previous != null)
            {
                length += 1;
                previous = previous.previous;
            }

            return length;
        }

        /// <summary>
        /// Enumerate the full path from the start to this node (will end with this node).
        /// </summary>
        /// <returns>Enumerable of all nodes in the path</returns>
        public IEnumerable<V> FullPath()
        {
            var stack = new Stack<V>();
            stack.Push(this.node);
            Path<V> previous = this.previous;
            while (previous != null)
            {
                stack.Push(previous.node);
                previous = previous.previous;
            }

            return stack.AsEnumerable();
        }
    }
}