// Copyright (c) 2018 Chris Nolet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickOutline
{
    [DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

        public enum Mode
        {
            OutlineAll,
            OutlineVisible,
            OutlineHidden,
            OutlineAndSilhouette,
            SilhouetteOnly
        }

        public Mode OutlineMode
        {
            get => this.outlineMode;
            set
            {
                this.outlineMode = value;
                this.needsUpdate = true;
            }
        }

        public Color OutlineColor
        {
            get => this.outlineColor;
            set
            {
                this.outlineColor = value;
                this.needsUpdate = true;
            }
        }

        public float OutlineWidth
        {
            get => this.outlineWidth;
            set
            {
                this.outlineWidth = value;
                this.needsUpdate = true;
            }
        }

        [Serializable]
        private class ListVector3
        {
            public List<Vector3> data;
        }

        [SerializeField]
        private Mode outlineMode;

        [SerializeField]
        private Color outlineColor = Color.white;

        [SerializeField, Range(0f, 10f)]
        private float outlineWidth = 2f;

        [Header("Optional")]

        [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
        + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
        private bool precomputeOutline;

        [SerializeField, HideInInspector]
        private List<Mesh> bakeKeys = new List<Mesh>();

        [SerializeField, HideInInspector]
        private List<ListVector3> bakeValues = new List<ListVector3>();

        private Renderer[] renderers;
        private Material outlineMaskMaterial;
        private Material outlineFillMaterial;

        private bool needsUpdate;

        public const string OUTLINE_MASK_SHADER = "Custom/Outline Mask";
        public const string OUTLINE_FILL_SHADER = "Custom/Outline Fill";

        private void Awake()
        {

            // Cache renderers
            this.renderers = this.GetComponentsInChildren<Renderer>();

            // Instantiate outline materials
            this.outlineMaskMaterial = new Material(Shader.Find(OUTLINE_MASK_SHADER));
            this.outlineFillMaterial = new Material(Shader.Find(OUTLINE_FILL_SHADER));

            this.outlineMaskMaterial.name = "OutlineMask (Instance)";
            this.outlineFillMaterial.name = "OutlineFill (Instance)";

            // Retrieve or generate smooth normals
            this.LoadSmoothNormals();

            this.UpdateMaterialProperties();

            // Apply material properties immediately
            this.needsUpdate = true;
        }

        private void OnEnable()
        {
            foreach (Renderer renderer in this.renderers)
            {

                // Append outline shaders
                var materials = renderer.sharedMaterials.ToList();

                materials.Add(this.outlineMaskMaterial);
                materials.Add(this.outlineFillMaterial);

                renderer.materials = materials.ToArray();
            }
        }

        private void OnValidate()
        {

            // Update material properties
            this.needsUpdate = true;

            // Clear cache when baking is disabled or corrupted
            if (!this.precomputeOutline && this.bakeKeys.Count != 0 || this.bakeKeys.Count != this.bakeValues.Count)
            {
                this.bakeKeys.Clear();
                this.bakeValues.Clear();
            }

            // Generate smooth normals when baking is enabled
            if (this.precomputeOutline && this.bakeKeys.Count == 0)
            {
                this.Bake();
            }
        }

        private void Update()
        {
            if (this.needsUpdate)
            {
                this.needsUpdate = false;

                this.UpdateMaterialProperties();
            }
        }

        private void OnDisable()
        {
            foreach (Renderer renderer in this.renderers)
            {

                // Remove outline shaders
                var materials = renderer.sharedMaterials.ToList();

                int index = 0;
                while (index < materials.Count)
                {
                    if (materials[index].shader.name == OUTLINE_FILL_SHADER ||
                        materials[index].shader.name == OUTLINE_MASK_SHADER)
                    {
                        materials.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }

                renderer.materials = materials.ToArray();
            }
        }

        private void OnDestroy()
        {

            // Destroy material instances
            Destroy(this.outlineMaskMaterial);
            Destroy(this.outlineFillMaterial);
        }

        private void Bake()
        {

            // Generate smooth normals for each mesh
            var bakedMeshes = new HashSet<Mesh>();

            foreach (MeshFilter meshFilter in this.GetComponentsInChildren<MeshFilter>())
            {

                // Skip duplicates
                if (!bakedMeshes.Add(meshFilter.sharedMesh))
                {
                    continue;
                }

                // Serialize smooth normals
                List<Vector3> smoothNormals = this.SmoothNormals(meshFilter.sharedMesh);

                this.bakeKeys.Add(meshFilter.sharedMesh);
                this.bakeValues.Add(new ListVector3() { data = smoothNormals });
            }
        }

        private void LoadSmoothNormals()
        {

            // Retrieve or generate smooth normals
            foreach (MeshFilter meshFilter in this.GetComponentsInChildren<MeshFilter>())
            {

                // Skip if smooth normals have already been adopted
                if (!registeredMeshes.Add(meshFilter.sharedMesh))
                {
                    continue;
                }

                // Retrieve or generate smooth normals
                int index = this.bakeKeys.IndexOf(meshFilter.sharedMesh);
                List<Vector3> smoothNormals = (index >= 0) ? this.bakeValues[index].data : this.SmoothNormals(meshFilter.sharedMesh);

                // Store smooth normals in UV3
                meshFilter.sharedMesh.SetUVs(3, smoothNormals);
            }

            // Clear UV3 on skinned mesh renderers
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                {
                    skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
                }
            }
        }

        private List<Vector3> SmoothNormals(Mesh mesh)
        {

            // Group vertices by location
            IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            var smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> group in groups)
            {

                // Skip single vertices
                if (group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                Vector3 smoothNormal = Vector3.zero;

                foreach (KeyValuePair<Vector3, int> pair in group)
                {
                    smoothNormal += mesh.normals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach (KeyValuePair<Vector3, int> pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        private void UpdateMaterialProperties()
        {

            // Apply properties according to mode
            this.outlineFillMaterial.SetColor("_OutlineColor", this.outlineColor);
            this.outlineMaskMaterial.SetColor("_OutlineColor", this.outlineColor);

            switch (this.outlineMode)
            {
                case Mode.OutlineAll:
                    this.outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    this.outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    this.outlineFillMaterial.SetFloat("_OutlineWidth", this.outlineWidth);
                    break;

                case Mode.OutlineVisible:
                    this.outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    this.outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    this.outlineFillMaterial.SetFloat("_OutlineWidth", this.outlineWidth);
                    break;

                case Mode.OutlineHidden:
                    this.outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    this.outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    this.outlineFillMaterial.SetFloat("_OutlineWidth", this.outlineWidth);
                    break;

                case Mode.OutlineAndSilhouette:
                    this.outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    this.outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    this.outlineFillMaterial.SetFloat("_OutlineWidth", this.outlineWidth);
                    break;

                case Mode.SilhouetteOnly:
                    this.outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    this.outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    this.outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                    break;
            }
        }
    }
}
