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

using UnityEngine;

namespace nickmaltbie.Data
{
    /// <summary>
    /// Demo materials for colors and drawing on the map.
    /// </summary>
    [CreateAssetMenu(fileName = "DemoMaterials", menuName = "ScriptableObjects/DemoMaterials", order = 1)]
    public class DemoMaterials : ScriptableObject
    {
        /// <summary>
        /// Default material associated with a tile.
        /// </summary>
        [Tooltip("Default material associated with a tile.")]
        public Material defaultMaterial;

        /// <summary>
        /// Material associated with the blocked tile state.
        /// </summary>
        [Tooltip("Material associated with the blocked tile state.")]
        public Material blockedMaterial;

        /// <summary>
        /// Material associated with the queued tile state.
        /// </summary>
        [Tooltip("Material associated with the queued tile state.")]
        public Material queuedMaterial;

        /// <summary>
        /// Material associated with the searched tile state.
        /// </summary>
        [Tooltip("Material associated with the searched tile state.")]
        public Material searchedMaterial;

        /// <summary>
        /// Material associated with the selected tile.
        /// </summary>
        [Tooltip("Material associated with the selected tile.")]
        public Material selectedMaterial;

        /// <summary>
        /// Material associated with tiles that are part of the final path.
        /// </summary>
        [Tooltip("Material associated with tiles that are part of the final path.")]
        public Material pathMaterial;

        /// <summary>
        /// Gradient of colors for queued tiles.
        /// </summary>
        [Tooltip("Gradient of colors for queued tiles.")]
        public Gradient queuedColorGradient;

        /// <summary>
        /// Material for default added path arrows.
        /// </summary>
        [Tooltip("Material for default added path arrows.")]
        public Material pathArrowMaterial;

        /// <summary>
        /// Material associated with a selected path arrow.
        /// </summary>
        [Tooltip("Material associated with a selected path arrow.")]
        public Material pathArrowSelectedMaterial;
    }
}
