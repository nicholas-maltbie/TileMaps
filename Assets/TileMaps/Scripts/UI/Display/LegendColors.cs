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

using nickmaltbie.TileMaps.Data;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.TileMaps.UI
{
    public class LegendColors : MonoBehaviour, IScreenComponent
    {
        public const string ShaderName = "Shader Graphs/PlanarMappingUnlit";
        public static readonly string[] colorNames = new string[] { "_Recolor1", "_Recolor2" };

        public DemoMaterials demoMaterials;

        public Image blockedImage;
        public Image selectedImage;
        public Image searchedImage;
        public Image pathImage;
        public Image queuedImage;
        public Image[] queuedPriority;
        public Image searchedPathBase;
        public Image searchedPathPath;
        public Image foundPathBase;
        public Image foundPathPath;

        public void OnScreenLoaded()
        {
            this.SetupMaterialProperties();
        }

        public void Awake()
        {
            this.blockedImage.material = new Material(Shader.Find(ShaderName));
            this.selectedImage.material = new Material(Shader.Find(ShaderName));
            this.searchedImage.material = new Material(Shader.Find(ShaderName));
            this.pathImage.material = new Material(Shader.Find(ShaderName));
            this.queuedImage.material = new Material(Shader.Find(ShaderName));
            this.searchedPathPath.material = new Material(Shader.Find(ShaderName));
            this.foundPathPath.material = new Material(Shader.Find(ShaderName));
            this.foundPathBase.material = new Material(Shader.Find(ShaderName));
            this.searchedPathBase.material = new Material(Shader.Find(ShaderName));

            foreach (Image queued in this.queuedPriority)
            {
                queued.material = new Material(Shader.Find(ShaderName));
            }

            this.SetupMaterialProperties();
        }

        public void SetupMaterialProperties()
        {
            this.CopyProperties(this.demoMaterials.blockedMaterial, this.blockedImage.material);
            this.CopyProperties(this.demoMaterials.selectedMaterial, this.selectedImage.material);
            this.CopyProperties(this.demoMaterials.searchedMaterial, this.searchedImage.material);
            this.CopyProperties(this.demoMaterials.pathMaterial, this.pathImage.material);
            this.CopyProperties(this.demoMaterials.queuedMaterial, this.queuedImage.material);
            this.CopyProperties(this.demoMaterials.pathArrowMaterial, this.searchedPathPath.material);
            this.CopyProperties(this.demoMaterials.pathArrowSelectedMaterial, this.foundPathPath.material);
            this.CopyProperties(this.demoMaterials.pathMaterial, this.foundPathBase.material);
            this.CopyProperties(this.demoMaterials.searchedMaterial, this.searchedPathBase.material);

            this.queuedImage.material.SetColor("_Recolor2", this.demoMaterials.queuedColorGradient.Evaluate(1));
            int steps = this.queuedPriority.Length;
            for (int i = 0; i < steps; i++)
            {
                // Copy over base color
                this.CopyProperties(this.demoMaterials.queuedMaterial, this.queuedPriority[i].material);

                // Copy over gradient color
                float percent = ((float)i) / (steps - 1);
                Color color = this.demoMaterials.queuedColorGradient.Evaluate(percent);
                this.queuedPriority[i].material.SetColor("_Recolor2", color);
            }
        }

        public void OnScreenUnloaded()
        {

        }

        public void CopyProperties(Material mat1, Material mat2)
        {
            foreach (string colorName in colorNames)
            {
                mat2.SetColor(colorName, mat1.GetColor(colorName));
            }

            mat2.SetTexture("_Albedo", mat1.GetTexture("_Albedo"));
            mat2.SetVector("_Scale", new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}
