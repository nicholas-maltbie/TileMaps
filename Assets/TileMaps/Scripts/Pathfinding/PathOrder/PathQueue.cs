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

namespace nickmaltbie.TileMap.Pathfinding.PathOrder
{
    /// <summary>
    /// Path collection that uses a queue to store the next path to check with a first in first out methodology.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    public class PathQueue<V> : IPathOrder<V>
    {
        /// <summary>
        /// Collection of paths in this queue.
        /// </summary>
        private readonly Queue<Path<V>> queue;

        /// <summary>
        /// Initialize and instance of PathQueue.
        /// </summary>
        public PathQueue()
        {
            queue = new Queue<Path<V>>();
        }

        /// <inheritdoc/>
        public void AddPath(Path<V> path)
        {
            queue.Enqueue(path);
        }

        /// <inheritdoc/>
        public Path<V> Peek()
        {
            return queue.Peek();
        }

        /// <inheritdoc/>
        public Path<V> Pop()
        {
            return queue.Dequeue();
        }

        /// <inheritdoc/>
        public IEnumerable<Path<V>> EnumerateElements()
        {
            IEnumerator<Path<V>> paths = queue.GetEnumerator();
            while (paths.MoveNext())
            {
                yield return paths.Current;
            }

            yield break;
        }

        /// <inheritdoc/>
        public int Count => queue.Count;
    }
}
