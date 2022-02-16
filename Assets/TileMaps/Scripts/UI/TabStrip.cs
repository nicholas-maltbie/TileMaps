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
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Code based on a nice tutorial provided by Mark Hedberg via their blog
// https://www.hedberggames.com/blog/creating-ui-tabs-in-unity-part-1

namespace nickmaltbie.TileMaps.UI
{
    [Serializable]
    public class TabPair
    {
        public Button TabButton;
        public CanvasGroup TabContent;
    }

    public class TabStrip : MonoBehaviour
    {
        public TabPair[] TabCollection;
        public Sprite TabIconPicked;
        public Sprite TabIconDefault;
        public Button DefaultTab;

        public int CurrentTabIndex { get; private set; }

        protected void SetTabState(int index, bool picked)
        {
            TabPair affectedItem = this.TabCollection[index];
            affectedItem.TabContent.interactable = picked;
            affectedItem.TabContent.blocksRaycasts = picked;
            affectedItem.TabContent.alpha = picked ? 1 : 0;
            affectedItem.TabButton.image.sprite = picked ? this.TabIconPicked : this.TabIconDefault;
        }

        public void PickTab(int index)
        {
            this.SetTabState(this.CurrentTabIndex, false);
            this.CurrentTabIndex = index;
            this.SetTabState(this.CurrentTabIndex, true);
        }

        protected int? FindTabIndex(Button tabButton)
        {
            TabPair currentTabPair = this.TabCollection.FirstOrDefault(x => x.TabButton == tabButton);
            if (currentTabPair == default)
            {
                Debug.LogWarning("The tab " + this.DefaultTab.gameObject.name + " does not belong to the tab strip " + this.name + ".");
                return null;
            }

            return Array.IndexOf(this.TabCollection, currentTabPair);
        }

        public void Start()
        {
            for (int i = 0; i < this.TabCollection.Length; i++)
            {
                //Storing the current value of i in a locally scoped variable.
                int index = i;
                this.TabCollection[index].TabButton.onClick.AddListener(new UnityAction(() => this.PickTab(index)));
            }
            //Initialize all tabs to an unpicked state
            for (int i = 0; i < this.TabCollection.Length; i++)
            {
                this.SetTabState(i, false);
            }
            //Pick the default tab
            if (this.TabCollection.Length > 0)
            {
                int? index = this.FindTabIndex(this.DefaultTab);
                //If tab is invalid, instead default to the first tab.
                if (index == null)
                {
                    index = 0;
                }

                this.CurrentTabIndex = index.Value;
                this.SetTabState(this.CurrentTabIndex, true);
            }
        }
    }
}
