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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace nickmaltbie.TileMaps.UI.Text
{
    /// <summary>
    /// This class handles basic link color behavior, supports also underline (static only)
    /// Does not support strike-through, but can be easily implemented in the same way as the underline
    /// 
    /// forum post reference - https://forum.unity.com/threads/textmeshpro-hyperlinks.1091296/
    /// 
    /// https://github.com/EpsilonD3lta/UnityUtilities/blob/master/Scripts/Runtime/TMProUGUIHyperlinks.cs
    /// </summary>
    [DisallowMultipleComponent()]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMProUGUIHyperlinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Color32 hoveredColor = new Color32(0x00, 0x59, 0xFF, 0xFF);
        [SerializeField]
        private Color32 pressedColor = new Color32(0x00, 0x00, 0xB7, 0xFF);
        [SerializeField]
        private Color32 usedColor = new Color32(0xFF, 0x00, 0xFF, 0xFF);
        [SerializeField]
        private Color32 usedHoveredColor = new Color32(0xFD, 0x5E, 0xFD, 0xFF);
        [SerializeField]
        private Color32 usedPressedColor = new Color32(0xCF, 0x00, 0xCF, 0xFF);

        private List<Color32[]> startColors = new List<Color32[]>();
        private TextMeshProUGUI textMeshPro;
        private Dictionary<int, bool> usedLinks = new Dictionary<int, bool>();
        private int hoveredLinkIndex = -1;
        private int pressedLinkIndex = -1;

        public void Awake()
        {
            this.textMeshPro = this.GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            int linkIndex = this.GetLinkIndex();
            if (linkIndex != -1) // Was pointer intersecting a link?
            {
                this.pressedLinkIndex = linkIndex;
                if (this.usedLinks.TryGetValue(linkIndex, out bool isUsed) && isUsed) // Has the link been already used?
                {
                    // Have we hovered before we pressed? Touch input will first press, then hover
                    if (this.pressedLinkIndex != this.hoveredLinkIndex)
                    {
                        this.startColors = this.SetLinkColor(linkIndex, this.usedPressedColor);
                    }
                    else
                    {
                        this.SetLinkColor(linkIndex, this.usedPressedColor);
                    }
                }
                else
                {
                    // Have we hovered before we pressed? Touch input will first press, then hover
                    if (this.pressedLinkIndex != this.hoveredLinkIndex)
                    {
                        this.startColors = this.SetLinkColor(linkIndex, this.pressedColor);
                    }
                    else
                    {
                        this.SetLinkColor(linkIndex, this.pressedColor);
                    }
                }

                this.hoveredLinkIndex = this.pressedLinkIndex; // Changes flow in LateUpdate
            }
            else
            {
                this.pressedLinkIndex = -1;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            int linkIndex = this.GetLinkIndex();
            if (linkIndex != -1 && linkIndex == this.pressedLinkIndex) // Was pointer intersecting the same link as OnPointerDown?
            {
                TMP_LinkInfo linkInfo = this.textMeshPro.textInfo.linkInfo[linkIndex];
                this.SetLinkColor(linkIndex, this.usedHoveredColor);
                this.startColors.ForEach(c => c[0] = c[1] = c[2] = c[3] = this.usedColor);
                this.usedLinks[linkIndex] = true;
                UnityEngine.Debug.Log($"Opening link - {linkInfo.GetLinkID()}");
                Application.OpenURL(linkInfo.GetLinkID());
            }

            this.pressedLinkIndex = -1;
        }

        public void LateUpdate()
        {
            int linkIndex = this.GetLinkIndex();
            if (linkIndex != -1) // Was pointer intersecting a link?
            {
                if (linkIndex != this.hoveredLinkIndex) // We started hovering above link (hover can be set from OnPointerDown!)
                {
                    if (this.hoveredLinkIndex != -1)
                    {
                        this.ResetLinkColor(this.hoveredLinkIndex, this.startColors); // If we hovered above other link before
                    }

                    this.hoveredLinkIndex = linkIndex;
                    if (this.usedLinks.TryGetValue(linkIndex, out bool isUsed) && isUsed) // Has the link been already used?
                    {
                        // If we have pressed on link, wandered away and came back, set the pressed color
                        if (this.pressedLinkIndex == linkIndex)
                        {
                            this.startColors = this.SetLinkColor(this.hoveredLinkIndex, this.usedPressedColor);
                        }
                        else
                        {
                            this.startColors = this.SetLinkColor(this.hoveredLinkIndex, this.usedHoveredColor);
                        }
                    }
                    else
                    {
                        // If we have pressed on link, wandered away and came back, set the pressed color
                        if (this.pressedLinkIndex == linkIndex)
                        {
                            this.startColors = this.SetLinkColor(this.hoveredLinkIndex, this.pressedColor);
                        }
                        else
                        {
                            this.startColors = this.SetLinkColor(this.hoveredLinkIndex, this.hoveredColor);
                        }
                    }
                }
            }
            else if (this.hoveredLinkIndex != -1) // If we hovered above other link before
            {
                this.ResetLinkColor(this.hoveredLinkIndex, this.startColors);
                this.hoveredLinkIndex = -1;
            }
        }

        public int GetLinkIndex()
        {
            return TMP_TextUtilities.FindIntersectingLink(this.textMeshPro, Mouse.current.position.ReadValue(), null);
        }

        public List<Color32[]> SetLinkColor(int linkIndex, Color32 color)
        {
            TMP_LinkInfo linkInfo = this.textMeshPro.textInfo.linkInfo[linkIndex];

            var oldVertexColors = new List<Color32[]>(); // Store the old character colors
            int underlineIndex = -1;
            for (int i = 0; i < linkInfo.linkTextLength; i++)
            {
                // For each character in the link string
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // The current character index
                TMP_CharacterInfo charInfo = this.textMeshPro.textInfo.characterInfo[characterIndex];
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material/subtext object used by this character.
                int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

                // This array contains colors for all vertices of the mesh (might be multiple chars)
                Color32[] vertexColors = this.textMeshPro.textInfo.meshInfo[meshIndex].colors32;
                oldVertexColors.Add(new Color32[] { vertexColors[vertexIndex + 0], vertexColors[vertexIndex + 1], vertexColors[vertexIndex + 2], vertexColors[vertexIndex + 3] });
                if (charInfo.isVisible)
                {
                    vertexColors[vertexIndex + 0] = color;
                    vertexColors[vertexIndex + 1] = color;
                    vertexColors[vertexIndex + 2] = color;
                    vertexColors[vertexIndex + 3] = color;
                }
                // Each line will have its own underline mesh with different index, index == 0 means there is no underline
                if (charInfo.isVisible && charInfo.underlineVertexIndex > 0 && charInfo.underlineVertexIndex != underlineIndex && charInfo.underlineVertexIndex < vertexColors.Length)
                {
                    underlineIndex = charInfo.underlineVertexIndex;
                    for (int j = 0; j < 12; j++) // Underline seems to be always 3 quads == 12 vertices
                    {
                        vertexColors[underlineIndex + j] = color;
                    }
                }
            }

            this.textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            return oldVertexColors;
        }

        public void ResetLinkColor(int linkIndex, List<Color32[]> startColors)
        {
            TMP_LinkInfo linkInfo = this.textMeshPro.textInfo.linkInfo[linkIndex];
            int underlineIndex = -1;
            for (int i = 0; i < linkInfo.linkTextLength; i++)
            {
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i;
                TMP_CharacterInfo charInfo = this.textMeshPro.textInfo.characterInfo[characterIndex];
                int meshIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Color32[] vertexColors = this.textMeshPro.textInfo.meshInfo[meshIndex].colors32;
                if (charInfo.isVisible)
                {
                    vertexColors[vertexIndex + 0] = startColors[i][0];
                    vertexColors[vertexIndex + 1] = startColors[i][1];
                    vertexColors[vertexIndex + 2] = startColors[i][2];
                    vertexColors[vertexIndex + 3] = startColors[i][3];
                }

                if (charInfo.isVisible && charInfo.underlineVertexIndex > 0 && charInfo.underlineVertexIndex != underlineIndex && charInfo.underlineVertexIndex < vertexColors.Length)
                {
                    underlineIndex = charInfo.underlineVertexIndex;
                    for (int j = 0; j < 12; j++)
                    {
                        vertexColors[underlineIndex + j] = startColors[i][0];
                    }
                }
            }

            this.textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}
