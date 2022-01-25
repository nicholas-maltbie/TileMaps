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
using nickmaltbie.TileMap.Common;

namespace nickmaltbie.TileMap.Pathfinding.PathOrder
{
    /// <summary>
    /// Path collection that uses weighted paths to store the next path in the collection via a function.
    /// These paths are sorted using a priority queue.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    /// <typeparam name="W">Weighted elements of paths in priority queue.</typeparam>
    public class PathPriorityQueue<W, V> : IPathOrder<V> where W : IComparable
    {
        /// <summary>
        /// Collection of paths in this priority queue.
        /// </summary>
        private readonly Heap<W, Path<V>> pQueue;

        /// <summary>
        /// Get the weight of a path.
        /// </summary>
        private readonly Func<Path<V>, W> GetWeight;

        /// <summary>
        /// Initialize and instance of PathPriorityQueue with a given weighting function and an initial capacity of 10.
        /// </summary>
        /// <param name="GetWeight">Function to determine the weight of a given path.</param>
        public PathPriorityQueue(Func<Path<V>, W> GetWeight) : this(GetWeight, 10) { }

        /// <summary>
        /// Initialize and instance of PathPriorityQueue with a given weighting function and capacity.
        /// </summary>
        /// <param name="GetWeight">Function to determine the weight of a given path.</param>
        /// <param name="initialCapacity">Initial capacity of the path.</param>
        public PathPriorityQueue(Func<Path<V>, W> GetWeight, int initialCapacity)
        {
            this.GetWeight = GetWeight;
            pQueue = new Heap<W, Path<V>>(initialCapacity);
        }

        /// <inheritdoc/>
        public void AddPath(Path<V> path)
        {
            pQueue.Add(GetWeight(path), path);
        }

        /// <inheritdoc/>
        public Path<V> Peek()
        {
            return pQueue.Peek();
        }

        /// <inheritdoc/>
        public Path<V> Pop()
        {
            return pQueue.Pop();
        }

        /// <inheritdoc/>
        public IEnumerable<Path<V>> EnumerateElements() => pQueue.EnumerateElements();

        /// <inheritdoc/>
        public int Count => pQueue.Count;
    }
}
