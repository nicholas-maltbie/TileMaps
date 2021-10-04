
using System;
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
            this.queue = new Queue<Path<V>>();
        }

        /// <inheritdoc/>
        public void AddPath(Path<V> path)
        {
            this.queue.Enqueue(path);
        }

        /// <inheritdoc/>
        public Path<V> Peek()
        {
            return this.queue.Peek();
        }

        /// <inheritdoc/>
        public Path<V> Pop()
        {
            return this.queue.Dequeue();
        }

        /// <inheritdoc/>
        public IEnumerable<Path<V>> EnumerateElements()
        {
            IEnumerator<Path<V>> paths = this.queue.GetEnumerator();
            while (paths.MoveNext())
            {
                yield return paths.Current;
            }
            yield break;
        }

        /// <inheritdoc/>
        public int Count => this.queue.Count;
    }
}
