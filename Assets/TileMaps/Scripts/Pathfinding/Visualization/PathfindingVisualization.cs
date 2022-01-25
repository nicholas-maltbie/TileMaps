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
using nickmaltbie.TileMap.Pathfinding.PathOrder;

namespace nickmaltbie.TileMap.Pathfinding.Visualization
{
    public enum StepType
    {
        StartPath,
        AddNode,
        MarkSearched,
        SkipSearched,
        EndPath
    }

    public struct PathfindingStep<V>
    {
        public IPathOrder<V> pathOrder;
        public Path<V> currentPath;
        public bool pathFound;
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
        /// <param name="tileMap">Tile map to find path within.</param>
        /// <param name="source">Starting position for path.</param>
        /// <param name="dest">Destination of path.</param>
        /// <param name="pathOrder">Path collection used to order paths when adding them to the list of possible
        /// paths.</param> 
        /// <typeparam name="K">Type of coordinates within the graph.</typeparam>
        /// <typeparam name="V">Values stored within the graph</typeparam>
        private static IEnumerable<PathfindingStep<K>> VisualizePath<K, V>(
            this ITileMap<K, V> tileMap,
            K source,
            K dest,
            IPathOrder<K> pathOrder)
        {
            // Base case if either source or dest are not in the tile map.
            if (!tileMap.IsInMap(source) || !tileMap.IsInMap(dest))
            {
                // Return empty list and no path found.
                yield return new PathfindingStep<K> { stepType = StepType.EndPath };
                yield break;
            }

            // Set of all elements that have already been searched.
            var searched = new HashSet<K>();

            // Initialize queue with first element.
            var sourcePath = new Path<K>(source);
            pathOrder.AddPath(sourcePath);

            // Mark as starting path
            yield return new PathfindingStep<K>
            {
                pathOrder = pathOrder,
                currentPath = sourcePath,
                stepType = StepType.StartPath
            };

            // While there are still tiles to search.
            while (pathOrder.Count > 0)
            {
                // Pop front of the priority queue.
                Path<K> pathToNode = pathOrder.Pop();

                // Check if we found the destination
                if (pathToNode.Node.Equals(dest))
                {
                    // Compute full path to this node and mark path as found.
                    yield return new PathfindingStep<K>
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
                    yield return new PathfindingStep<K>
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
                    yield return new PathfindingStep<K>
                    {
                        pathOrder = pathOrder,
                        currentPath = pathToNode,
                        stepType = StepType.MarkSearched,
                        pathFound = false,
                    };
                    searched.Add(pathToNode.Node);
                }

                foreach (K neighbor in tileMap.GetNeighbors(pathToNode.Node))
                {
                    // if the neighbor is not already searched, add it to the queue
                    if (searched.Contains(neighbor))
                    {
                        continue;
                    }
                    else
                    {
                        // Add a new edge from the previous node to the neighbor
                        var neighborPath = new Path<K>(neighbor, pathToNode);
                        pathOrder.AddPath(neighborPath);
                    }
                }

                yield return new PathfindingStep<K>
                {
                    pathOrder = pathOrder,
                    currentPath = pathToNode,
                    stepType = StepType.AddNode,
                    pathFound = false,
                };
            }
            // Return empty list and no path found.
            yield return new PathfindingStep<K>
            {
                pathOrder = pathOrder,
                stepType = StepType.EndPath,
                pathFound = false,
            };
        }

        public static IEnumerable<PathfindingStep<K>> VisualizePathAStar<K, V, W>(
            this ITileMap<K, V> tileMap,
            K source,
            K dest,
            Func<Path<K>, W> GetWeight)
            where W : IComparable
        {
            return VisualizePath<K, V>(tileMap, source, dest, new PathPriorityQueue<W, K>(GetWeight));
        }

        public static IEnumerable<PathfindingStep<K>> VisualizePathDFS<V, K>(
            this ITileMap<K, V> tileMap,
            K source,
            K dest)
        {
            return VisualizePath<K, V>(tileMap, source, dest, new PathStack<K>());
        }

        public static IEnumerable<PathfindingStep<K>> VisualizePathBFS<V, K>(
            this ITileMap<K, V> tileMap,
            K source,
            K dest)
        {
            return VisualizePath<K, V>(tileMap, source, dest, new PathQueue<K>());
        }
    }
}
