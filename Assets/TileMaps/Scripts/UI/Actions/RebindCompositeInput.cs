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
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace nickmaltbie.TileSet.UI.Actions
{
    /// <summary>
    /// Rebind a composite set of inputs
    /// </summary>
    public class RebindCompositeInput : MonoBehaviour, IBindingControl
    {
        /// <summary>
        /// Prefix for input mapping for saving to player preferences
        /// </summary>
        public const string inputMappingPlayerPrefPrefix = "Input Mapping";

        /// <summary>
        /// Composite input action being modified
        /// </summary>
        public InputActionReference inputAction = null;
        /// <summary>
        /// Menu controller related to this selected object
        /// </summary>
        public MenuController menuController;

        /// <summary>
        /// Rebinding group definition that includes the string name and button to control it
        /// </summary>
        [System.Serializable]
        public struct RebindingGroup
        {
            /// <summary>
            /// Location of descriptive name of button
            /// </summary>
            public UnityEngine.UI.Text bindingDisplayNameText;
            /// <summary>
            /// Button used to start rebinding operation
            /// </summary>
            public Button startRebinding;
            /// <summary>
            /// Text to show when waiting for player input
            /// </summary>
            public GameObject waitingForInputObject;
        }

        /// <summary>
        /// Binding groups that control binding for each component of the composite
        /// </summary>
        public RebindingGroup[] rebindingGroups;

        /// <summary>
        /// Rebinding operation action waiting for player command to change button bindings
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation rebindingOperation { get; private set; }

        /// <summary>
        /// Get a readable display name for a binding index
        /// </summary>
        /// <param name="index">Index of binding information</param>
        /// <returns>Human readable information of button for this binding index</returns>
        private string GetKeyReadableName(int index) => InputControlPath.ToHumanReadableString(
            this.inputAction.action.bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        /// <summary>
        /// Get the input mapping player preference key from a given index
        /// </summary>
        /// <param name="index">Index of binding component</param>
        public string InputMappingKey(int index) => $"{inputMappingPlayerPrefPrefix} {index} {this.inputAction.action.name}";

        public void Awake()
        {
            // Load menu controller if not set
            if (this.menuController == null)
            {
                this.menuController = this.GetComponentInParent<MenuController>();
            }

            // Load the default mapping saved to the file
            // Start bindings at 1 as 0 is the composite start and 1 represents the first binding index
            for (int i = 1; i <= this.rebindingGroups.Length; i++)
            {
                string inputMapping = PlayerPrefs.GetString(this.InputMappingKey(i), string.Empty);
                if (!string.IsNullOrEmpty(inputMapping))
                {
                    this.inputAction.action.ApplyBindingOverride(i, inputMapping);
                }
            }
        }

        public void Start()
        {
            for (int i = 0; i < this.rebindingGroups.Length; i++)
            {
                int temp = i;
                this.rebindingGroups[i].startRebinding.onClick.AddListener(() => this.StartRebinding(temp));
                this.rebindingGroups[i].bindingDisplayNameText.text = this.GetKeyReadableName(i + 1);
            }
        }

        /// <summary>
        /// Start the rebinding process for a given component of this composite axis.
        /// </summary>
        /// <param name="index">Index of binding (starting at 0)</param>
        public void StartRebinding(int index)
        {
            this.rebindingGroups[index].startRebinding.gameObject.SetActive(false);
            this.rebindingGroups[index].waitingForInputObject.SetActive(true);
            this.menuController.allowInputChanges = false;

            this.inputAction.action.Disable();
            this.inputAction.action.actionMap.Disable();

            this.rebindingOperation = this.inputAction.action.PerformInteractiveRebinding(index + 1)
                .WithControlsExcluding("<Pointer>/position") // Don't bind to mouse position
                .WithControlsExcluding("<Pointer>/delta")    // To avoid accidental input from mouse motion
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => this.RebindComplete(index))
                .OnCancel(operation => this.RebindCancel(index))
                .Start();
        }

        /// <summary>
        /// Cancel the rebinding process for a given component of this composite axis.
        /// </summary>
        public void RebindCancel(int index)
        {
            int bindingIndex = index + 1;

            this.rebindingGroups[index].bindingDisplayNameText.text = this.GetKeyReadableName(bindingIndex);
            this.rebindingOperation.Dispose();

            this.rebindingGroups[index].startRebinding.gameObject.SetActive(true);
            this.rebindingGroups[index].waitingForInputObject.SetActive(false);
            this.menuController.allowInputChanges = true;
            this.inputAction.action.Enable();
            this.inputAction.action.actionMap.Enable();
        }

        /// <summary>
        /// Finish the rebinding process for a given component of this composite axis.
        /// </summary>
        /// <param name="index">Index of binding (starting at 0)</param>
        public void RebindComplete(int index)
        {
            int bindingIndex = index + 1;
            string overridePath = this.inputAction.action.bindings[bindingIndex].overridePath;
            foreach (PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                InputAction action = input.actions.FindAction(this.inputAction.name);
                if (action != null)
                {
                    action.ApplyBindingOverride(bindingIndex, overridePath);
                }
            }

            this.rebindingGroups[index].bindingDisplayNameText.text = this.GetKeyReadableName(bindingIndex);
            this.rebindingOperation.Dispose();

            PlayerPrefs.SetString(this.InputMappingKey(bindingIndex), this.inputAction.action.bindings[bindingIndex].overridePath);

            this.rebindingGroups[index].startRebinding.gameObject.SetActive(true);
            this.rebindingGroups[index].waitingForInputObject.SetActive(false);
            this.menuController.allowInputChanges = true;
            this.inputAction.action.Enable();
            this.inputAction.action.actionMap.Enable();
        }

        public void ResetBinding()
        {
            this.inputAction.action.RemoveAllBindingOverrides();
            for (int bindingIndex = 1; bindingIndex <= this.rebindingGroups.Length; bindingIndex++)
            {
                PlayerPrefs.DeleteKey(this.InputMappingKey(bindingIndex));
            }
        }

        public void UpdateDisplay()
        {
            for (int index = 0; index < this.rebindingGroups.Length; index++)
            {
                int bindingIndex = index + 1;
                this.rebindingGroups[index].bindingDisplayNameText.text = this.GetKeyReadableName(bindingIndex);
            }
        }
    }
}
