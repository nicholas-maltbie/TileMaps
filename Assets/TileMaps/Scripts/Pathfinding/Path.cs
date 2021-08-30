using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;

namespace nickmaltbie.TileMap.Pathfinding
{
    /// <summary>
    /// Represent a path via recursive definition for finding a path between nodes in a graph.
    /// </summary>
    /// <typeparam name="V">Type of elements stored in the path</typeparam>
    public class Path<V>
    {
        /// <summary>
        /// Previous path.
        /// </summary>
        private Path<V> previous;

        /// <summary>
        /// Element stored at this location in the path.
        /// </summary>
        private V element;

        public Path(V element, Path<V> previous)
        {
            this.previous = previous;
            this.element = element;
        }

        /// <summary>
        /// Get the full length of the path.
        /// </summary>
        public int Length()
        {
            int length = 1;
            Path<V> previous = this.previous;
            while(previous != null)
            {
                length += 1;
                previous = previous.previous;
            }
            return length;
        }

        /// <summary>
        /// Enumerate the full path from the start to this node (will end with this node).
        /// </summary>
        /// <returns>Enumerable of all elements in the path</returns>
        public IEnumerable<V> FullPath()
        {
            Stack<V> stack = new Stack<V>();
            stack.Push(element);
            Path<V> previous = this.previous;
            while(previous != null)
            {
                stack.Push(previous.element);
                previous = previous.previous;
            }
            return stack.AsEnumerable();
        }
    }
}