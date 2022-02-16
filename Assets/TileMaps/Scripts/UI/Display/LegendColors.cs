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

using System.Collections.Generic;
using System.Linq;
using nickmaltbie.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace nickmaltbie.TileSet.UI
{
    public class LegendColors : MonoBehaviour, IScreenComponent
    {
        public const string ShaderName = "Shader Graphs/PlanarMappingUnlit";
        public static readonly string[] colorNames = new string[]{"_Recolor1", "_Recolor2"};

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
            SetupMaterialProperties();
        }

        public void Awake()
        {
            blockedImage.material = new Material(Shader.Find(ShaderName));
            selectedImage.material = new Material(Shader.Find(ShaderName));
            searchedImage.material = new Material(Shader.Find(ShaderName));
            pathImage.material = new Material(Shader.Find(ShaderName));
            queuedImage.material = new Material(Shader.Find(ShaderName));
            searchedPathPath.material = new Material(Shader.Find(ShaderName));
            foundPathPath.material = new Material(Shader.Find(ShaderName));
            foundPathBase.material = new Material(Shader.Find(ShaderName));
            searchedPathBase.material = new Material(Shader.Find(ShaderName));
            
            foreach (var queued in queuedPriority)
            {
                queued.material = new Material(Shader.Find(ShaderName));
            }

            SetupMaterialProperties();
        }

        public void SetupMaterialProperties()
        {
            CopyProperties(demoMaterials.blockedMaterial, blockedImage.material);
            CopyProperties(demoMaterials.selectedMaterial, selectedImage.material);
            CopyProperties(demoMaterials.searchedMaterial, searchedImage.material);
            CopyProperties(demoMaterials.pathMaterial, pathImage.material);
            CopyProperties(demoMaterials.queuedMaterial, queuedImage.material);
            CopyProperties(demoMaterials.pathArrowMaterial, searchedPathPath.material);
            CopyProperties(demoMaterials.pathArrowSelectedMaterial, foundPathPath.material);
            CopyProperties(demoMaterials.pathMaterial, foundPathBase.material);
            CopyProperties(demoMaterials.searchedMaterial, searchedPathBase.material);

            queuedImage.material.SetColor("_Recolor2", this.demoMaterials.queuedColorGradient.Evaluate(1));
            int steps = queuedPriority.Length;
            for (int i = 0; i < steps; i++)
            {
                // Copy over base color
                CopyProperties(demoMaterials.queuedMaterial, queuedPriority[i].material);

                // Copy over gradient color
                float percent = ((float) i) / (steps - 1);
                Color color = this.demoMaterials.queuedColorGradient.Evaluate(percent);
                queuedPriority[i].material.SetColor("_Recolor2", color);
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
