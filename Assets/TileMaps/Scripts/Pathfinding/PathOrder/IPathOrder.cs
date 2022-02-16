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

namespace nickmaltbie.TileMaps.Pathfinding.PathOrder
{
    /// <summary>
    /// Generic collection of paths used for pathfinding that can have paths of a specific type added to it
    /// or paths of a specific type removed from it.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    public interface IPathOrder<V>
    {
        /// <summary>
        /// Add a path to the given collection.
        /// </summary>
        /// <param name="path">Path to add to this collection.</param>
        void AddPath(Path<V> path);

        /// <summary>
        /// Peek at the next path to check without removing it from the collection.
        /// </summary>
        /// <returns>The path that is stored at the front of the collection.</returns>
        /// <exception cref="IndexOutOfRangeException">If the collection is empty.</exception>
        Path<V> Peek();

        /// <summary>
        /// Get the next path to check and remove it from the collection.
        /// </summary>
        /// <returns>The path that is stored at the front of the collection.</returns>
        /// <exception cref="IndexOutOfRangeException">If the collection is empty.</exception>
        Path<V> Pop();

        /// <summary>
        /// Get the number of elements stored in this current path collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Enumerate the elements in this path in some given order. This intended as a debug operation and may not
        /// always result in the fully correct order of elements. 
        /// </summary>
        /// <returns>The enumerated values within this path order.</returns>
        IEnumerable<Path<V>> EnumerateElements();
    }
}
