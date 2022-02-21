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
using nickmaltbie.TileMaps.Example;
using UnityEngine;
using UnityEngine.UI;

namespace nickmaltbie.TileMaps.UI.Actions
{

    /// <summary>
    /// Manage controls to configure the map pathfinding.
    /// </summary>
    public class ConfigureMap : MonoBehaviour
    {
        /// <summary>
        /// Dropdown for selecting path mode.
        /// </summary>
        public Dropdown modeDropdown;

        /// <summary>
        /// Button to step pathfinding forward.
        /// </summary>
        public Button stepButton;

        /// <summary>
        /// Button to toggle animation.
        /// </summary>
        public Button togglePlay;

        /// <summary>
        /// Toggle main pointer action between block and pathfinding.
        /// </summary>
        public Dropdown mainActionDropdown;

        /// <summary>
        /// Reset the blocked tiles.
        /// </summary>
        public Button resetBlockedButton;

        /// <summary>
        /// Reset the current pathfinding progress
        /// </summary>
        public Button restartPathfinding;

        /// <summary>
        /// Reset the current pathfinding.
        /// </summary>
        public Button clearPathButton;

        /// <summary>
        /// Slider to select pathfinding speed for animation.
        /// </summary>
        public Slider pathfindingSpeed;

        /// <summary>
        /// Slider to select pathfinding speed for drawing final path.
        /// </summary>
        public Slider finalPathSpeed;

        /// <summary>
        /// Sprite to show play/pause button.
        /// </summary>
        public Image animationSprite;

        /// <summary>
        /// Sprite for play button.
        /// </summary>
        public Sprite playSprite;

        /// <summary>
        /// Sprite for pause button.
        /// </summary>
        public Sprite pauseSprite;

        /// <summary>
        /// Setup pathfinding controls.
        /// </summary>
        public void Awake()
        {
            // Setup mode Dropdown
            this.modeDropdown.ClearOptions();
            this.modeDropdown.AddOptions(Enum.GetNames(typeof(PathMode)).ToList<string>());
            this.modeDropdown.onValueChanged.AddListener(
                selected => this.ApplyOperationToMaps(
                    gridMap => gridMap.searchMode = (PathMode)Enum.Parse(
                        typeof(PathMode), this.modeDropdown.options[selected].text)));
            this.modeDropdown.value = 0;
            this.GetGrid().searchMode = (PathMode)Enum.Parse(typeof(PathMode), this.modeDropdown.options[0].text);

            // Setup blocking toggle
            this.mainActionDropdown.ClearOptions();
            this.mainActionDropdown.AddOptions(Enum.GetNames(typeof(BoardAction)).ToList<string>());
            this.mainActionDropdown.onValueChanged.AddListener(
                selected => this.ApplyOperationToMaps(
                    gridMap => gridMap.SetPrimaryAction((BoardAction)Enum.Parse(
                        typeof(BoardAction), this.mainActionDropdown.options[selected].text))));
            this.mainActionDropdown.value = 0;
            this.GetGrid().SetPrimaryAction((BoardAction)
                Enum.Parse(typeof(BoardAction), this.mainActionDropdown.options[0].text));

            // Setup action buttons
            this.stepButton?.onClick.AddListener(() => this.ApplyOperationToMaps(
                gridMap =>
                {
                    gridMap.CurrentMode = PathfindingAnimationState.Paused;
                    gridMap.PathfindingStep();
                }));
            this.togglePlay?.onClick.AddListener(() => this.ApplyOperationToMaps(
                gridMap => gridMap.TogglePlay()));
            foreach (AbstractExampleGrid exampleGrid in GameObject.FindObjectsOfType<AbstractExampleGrid>())
            {
                exampleGrid.OnPlayModeChange += (_, playMode) =>
                {
                    switch (playMode)
                    {
                        case PathfindingAnimationState.Playing:
                            this.animationSprite.sprite = this.pauseSprite;
                            break;
                        case PathfindingAnimationState.Paused:
                            this.animationSprite.sprite = this.playSprite;
                            break;
                    }
                };
                exampleGrid.CurrentMode = PathfindingAnimationState.Playing;
            }

            this.resetBlockedButton?.onClick.AddListener(() => this.ApplyOperationToMaps(
                gridMap => gridMap.ResetBlocked()));
            this.clearPathButton?.onClick.AddListener(() => this.ApplyOperationToMaps(
                gridMap => gridMap.ClearPath()));

            // Setup sliders
            this.finalPathSpeed.onValueChanged.AddListener(value => this.ApplyOperationToMaps(
                gridMap => gridMap.stepDelay = this.finalPathSpeed.maxValue - value));
            // finalPathDelay.onValueChanged.AddListener(value => ApplyOperationToMaps(
            //     gridMap => gridMap.finalPathDelay = value));
            this.finalPathSpeed.value = this.finalPathSpeed.value;
            this.finalPathSpeed.value = this.finalPathSpeed.value;
        }

        public AbstractExampleGrid GetGrid()
        {
            foreach (AbstractExampleGrid exampleGrid in GameObject.FindObjectsOfType<AbstractExampleGrid>())
            {
                if (exampleGrid.gameObject.activeSelf)
                {
                    return exampleGrid;
                }
            }

            return null;
        }

        /// <summary>
        /// Applies a grid action to every active abstract example grid.
        /// </summary>
        /// <param name="gridAction">Action to apply to an abstract example grid.</param>
        public void ApplyOperationToMaps(Action<AbstractExampleGrid> gridAction)
        {
            gridAction(this.GetGrid());
        }
    }
}
