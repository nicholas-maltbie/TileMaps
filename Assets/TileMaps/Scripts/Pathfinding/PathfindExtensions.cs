using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;

namespace nickmaltbie.TileMap.Pathfinding
{
    /// <summary>
    /// Pathfinding extensions between two locations in a hex grid.
    /// </summary>
    public static class PathfindExtensions
    {
        /// <summary>
        /// Find a path between two nodes in the graph using breadth first search.
        /// </summary>
        /// <param name="tileMap">Tile map to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="path">Path found between source and dest in the graph. If a path cannot be found between the
        /// two points, this will be an empty list with no elements.</param>
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        /// <typeparam name="K">Values stored within the graph</typeparam>
        /// <returns>True if a path can be found between the source and destination, false otherwise.</returns>
        public static bool FindPathBFS<V, K>(this ITileMap<V, K> tileMap, V source, V dest, out List<V> path)
        {
            // Base case if either source or dest are not in the tile map.
            if (!tileMap.IsInMap(source) || !tileMap.IsInMap(dest))
            {
                // Return empty list and no path found.
                path = new List<V>();
                return false;
            }

            // Set of all elements that have already been searched.
            HashSet<V> searched = new HashSet<V>();

            // Queue of edges between two locations in the graph.
            Queue<Path<V>> queue = new Queue<Path<V>>();

            // Initialize queue with first element.
            queue.Enqueue(new Path<V>(source));

            // While there are still tiles to search.
            while (queue.Count > 0)
            {
                // dequeue front of the stack.
                Path<V> pathToNode = queue.Dequeue();

                // Check if we found the destination
                if (pathToNode.Node.Equals(dest))
                {
                    // Compute full path to this node and mark path as found.
                    path = pathToNode.FullPath().ToList();
                    return true;
                }

                // If the end has already been searched, continue to next edge in queue;
                // otherwise add it to the queue.
                if (searched.Contains(pathToNode.Node))
                {
                    continue;
                }
                else
                {
                    searched.Add(pathToNode.Node);
                }

                foreach (V neighbor in tileMap.GetNeighbors(pathToNode.Node))
                {
                    // if the neighbor is not already searched, add it to the queue
                    if (searched.Contains(neighbor))
                    {
                        continue;
                    }
                    else
                    {
                        // Add a new edge from the previous node to the neighbor
                        queue.Enqueue(new Path<V>(neighbor, pathToNode));
                    }
                }
            }
            // Return empty list and no path found.
            path = new List<V>();
            return false;
        }
    }
}