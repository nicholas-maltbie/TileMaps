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
using nickmaltbie.TileMaps.Common;
using nickmaltbie.TileMaps.Pathfinding.PathOrder;

namespace nickmaltbie.TileMaps.Pathfinding
{
    /// <summary>
    /// Pathfinding extensions between two locations in a hex grid.
    /// </summary>
    public static class PathfindExtensions
    {
        /// <summary>
        /// Find a path between two nodes in the graph using a given path collection.
        /// </summary>
        /// <param name="graph">Graph to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="path">Path found between source and dest in the graph. If a path cannot be found between the
        /// two points, this will be an empty list with no elements.</param>
        /// <param name="pathOrder">Path collection used to order paths when adding them to the list of possible
        /// paths.</param> 
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        /// <returns>True if a path can be found between the source and destination, false otherwise.</returns>
        private static bool FindPath<V>(
            this IGraph<V> graph,
            V source,
            V dest,
            IPathOrder<V> pathOrder,
            out List<V> path)
        {
            // Base case if either source or dest are not in the tile map.
            if (!graph.Contains(source) || !graph.Contains(dest))
            {
                // Return empty list and no path found.
                path = new List<V>();
                return false;
            }

            // Set of all elements that have already been searched.
            var searched = new HashSet<V>();

            // Initialize queue with first element.
            var sourcePath = new Path<V>(source);
            pathOrder.AddPath(sourcePath);

            // While there are still tiles to search.
            while (pathOrder.Count > 0)
            {
                // Pop front of the priority queue.
                Path<V> pathToNode = pathOrder.Pop();

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

                foreach (V neighbor in graph.GetNeighbors(pathToNode.Node))
                {
                    // if the neighbor is not already searched, add it to the queue
                    if (searched.Contains(neighbor))
                    {
                        continue;
                    }
                    else
                    {
                        // Add a new edge from the previous node to the neighbor
                        var neighborPath = new Path<V>(neighbor, pathToNode);
                        pathOrder.AddPath(neighborPath);
                    }
                }
            }
            // Return empty list and no path found.
            path = new List<V>();
            return false;
        }

        /// <summary>
        /// Find a path between two nodes in the graph using A Star Algorithm.
        /// </summary>
        /// <param name="graph">Graph to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="path">Path found between source and dest in the graph. If a path cannot be found between the
        /// two points, this will be an empty list with no elements.</param>
        /// <param name="GetWeight">Get the weight of a given path.</param>
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        /// <typeparam name="W">Type of value used to store weight of paths. Must be comparable to W.</typeparam>
        /// <returns>True if a path can be found between the source and destination, false otherwise.</returns>
        public static bool FindPathAStar<V, W>(
            this IGraph<V> graph,
            V source,
            V dest,
            Func<Path<V>, W> GetWeight,
            out List<V> path)
            where W : IComparable
        {
            return FindPath(graph, source, dest, new PathPriorityQueue<W, V>(GetWeight), out path);
        }

        /// <summary>
        /// Find a path between two nodes in the graph using depth first search.
        /// </summary>
        /// <param name="graph">Graph to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="path">Path found between source and dest in the graph. If a path cannot be found between the
        /// two points, this will be an empty list with no elements.</param>
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        /// <returns>True if a path can be found between the source and destination, false otherwise.</returns>
        public static bool FindPathDFS<V>(this IGraph<V> graph, V source, V dest, out List<V> path)
        {
            return FindPath(graph, source, dest, new PathStack<V>(), out path);
        }

        /// <summary>
        /// Find a path between two nodes in the graph using breadth first search.
        /// </summary>
        /// <param name="graph">Graph to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="path">Path found between source and dest in the graph. If a path cannot be found between the
        /// two points, this will be an empty list with no elements.</param>
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        /// <typeparam name="K">Values stored within the graph</typeparam>
        /// <returns>True if a path can be found between the source and destination, false otherwise.</returns>
        public static bool FindPathBFS<V>(this IGraph<V> graph, V source, V dest, out List<V> path)
        {
            return FindPath(graph, source, dest, new PathQueue<V>(), out path);
        }
    }
}
