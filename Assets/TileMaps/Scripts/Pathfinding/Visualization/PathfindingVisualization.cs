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
using nickmaltbie.TileMaps.Common;
using nickmaltbie.TileMaps.Pathfinding.PathOrder;

namespace nickmaltbie.TileMaps.Pathfinding.Visualization
{
    /// <summary>
    /// Types of steps/iterations in the pathfinding algorithm.
    /// </summary>
    public enum StepType
    {
        StartPath,
        AddNode,
        MarkSearched,
        SkipSearched,
        EndPath
    }

    /// <summary>
    /// Structure describing an individual step within a pathfinding search.
    /// </summary>
    public struct PathfindingStep<V>
    {
        /// <summary>
        /// Order of all vertices to be searched next.
        /// </summary>
        public IPathOrder<V> pathOrder;

        /// <summary>
        /// Current path that is being evaluated in this step.
        /// </summary>
        public Path<V> currentPath;

        /// <summary>
        /// Has a path to the destination been found.
        /// </summary>
        public bool pathFound;

        /// <summary>
        /// Type of step within the pathfinding algorithm.
        /// </summary>
        public StepType stepType;
    }

    /// <summary>
    /// Pathfinding extensions between two locations in a hex grid.
    /// </summary>
    public static class PathfindingVisualization
    {
        /// <summary>
        /// Find and visualize path between two nodes in the graph using a given path collection.
        /// </summary>
        /// <param name="graph">Tile map to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="pathOrder">Path collection used to order paths when adding them to the list of possible
        /// paths.</param> 
        /// <typeparam name="V">Type of coordinates within the graph.</typeparam>
        private static IEnumerable<PathfindingStep<V>> VisualizePath<V>(
            this IGraph<V> graph,
            V source,
            V dest,
            IPathOrder<V> pathOrder)
        {
            // Base case if either source or dest are not in the tile map.
            if (!graph.Contains(source) || !graph.Contains(dest))
            {
                // Return empty list and no path found.
                yield return new PathfindingStep<V> { stepType = StepType.EndPath };
                yield break;
            }

            // Set of all elements that have already been searched.
            var searched = new HashSet<V>();

            // Initialize queue with first element.
            var sourcePath = new Path<V>(source);
            pathOrder.AddPath(sourcePath);

            // Mark as starting path
            yield return new PathfindingStep<V>
            {
                pathOrder = pathOrder,
                currentPath = sourcePath,
                stepType = StepType.StartPath
            };

            // While there are still tiles to search.
            while (pathOrder.Count > 0)
            {
                // Pop front of the priority queue.
                Path<V> pathToNode = pathOrder.Pop();

                // Check if we found the destination
                if (pathToNode.Node.Equals(dest))
                {
                    // Compute full path to this node and mark path as found.
                    yield return new PathfindingStep<V>
                    {
                        pathOrder = pathOrder,
                        currentPath = pathToNode,
                        stepType = StepType.EndPath,
                        pathFound = true,
                    };
                    yield break;
                }

                // If the end has already been searched, continue to next edge in queue;
                // otherwise add it to the queue.
                if (searched.Contains(pathToNode.Node))
                {
                    yield return new PathfindingStep<V>
                    {
                        pathOrder = pathOrder,
                        currentPath = pathToNode,
                        stepType = StepType.SkipSearched,
                        pathFound = false,
                    };
                    continue;
                }
                else
                {
                    yield return new PathfindingStep<V>
                    {
                        pathOrder = pathOrder,
                        currentPath = pathToNode,
                        stepType = StepType.MarkSearched,
                        pathFound = false,
                    };
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

                yield return new PathfindingStep<V>
                {
                    pathOrder = pathOrder,
                    currentPath = pathToNode,
                    stepType = StepType.AddNode,
                    pathFound = false,
                };
            }
            // Return empty list and no path found.
            yield return new PathfindingStep<V>
            {
                pathOrder = pathOrder,
                stepType = StepType.EndPath,
                pathFound = false,
            };
        }

        public static IEnumerable<PathfindingStep<V>> VisualizePathAStar<V, W>(
            this IGraph<V> tileMap,
            V source,
            V dest,
            Func<Path<V>, W> GetWeight)
            where W : IComparable
        {
            return VisualizePath<V>(tileMap, source, dest, new PathPriorityQueue<W, V>(GetWeight));
        }

        public static IEnumerable<PathfindingStep<V>> VisualizePathDFS<V>(
            this IGraph<V> tileMap,
            V source,
            V dest)
        {
            return VisualizePath<V>(tileMap, source, dest, new PathStack<V>());
        }

        public static IEnumerable<PathfindingStep<V>> VisualizePathBFS<V>(
            this IGraph<V> tileMap,
            V source,
            V dest)
        {
            return VisualizePath<V>(tileMap, source, dest, new PathQueue<V>());
        }
    }
}
