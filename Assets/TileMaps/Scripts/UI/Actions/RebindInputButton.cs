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
    /// Rebind an individual button input action
    /// </summary>
    public class RebindInputButton : MonoBehaviour, IBindingControl
    {
        /// <summary>
        /// Prefix for input mapping for saving to player preferences
        /// </summary>
        public const string inputMappingPlayerPrefPrefix = "Input Mapping";

        /// <summary>
        /// Path used to cancel binding mid operation
        /// </summary>
        public string cancelPath = "<Keyboard>/escape";

        /// <summary>
        /// Input action being modified
        /// </summary>
        public InputActionReference inputAction = null;
        /// <summary>
        /// Binding display name for showing the control button description
        /// </summary>
        public UnityEngine.UI.Text bindingDisplayNameText = null;
        /// <summary>
        /// Button to start rebinding for the given input action
        /// </summary>
        public Button startRebinding = null;
        /// <summary>
        /// Text to display when waiting for the player to press a new input action
        /// </summary>
        public GameObject waitingForInputObject = null;
        /// <summary>
        /// Menu controller related to this selected object
        /// </summary>
        public MenuController menuController;

        /// <summary>
        /// Rebinding operation action waiting for player command to change button bindings
        /// </summary>
        public InputActionRebindingExtensions.RebindingOperation rebindingOperation { get; private set; }

        /// <summary>
        /// Get a readable display name for a binding index
        /// </summary>
        /// <returns>Human readable information of button for this binding index</returns>
        private string GetKeyReadableName() =>
            InputControlPath.ToHumanReadableString(
                this.inputAction.action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

        /// <summary>
        /// Get the input mapping player preference key from a given index
        /// </summary>
        public string InputMappingKey => $"{inputMappingPlayerPrefPrefix} {this.inputAction.action.name}";

        public void Awake()
        {
            // Load the default mapping saved to the file
            string inputMapping = PlayerPrefs.GetString(this.InputMappingKey, string.Empty);
            if (!string.IsNullOrEmpty(inputMapping))
            {
                this.inputAction.action.ApplyBindingOverride(inputMapping);
            }

            if (this.menuController == null)
            {
                this.menuController = this.GetComponentInParent<MenuController>();
            }
        }

        public void Start()
        {
            this.startRebinding.onClick.AddListener(() => this.StartRebinding());
            this.bindingDisplayNameText.text = this.GetKeyReadableName();
        }

        /// <summary>
        /// Start the rebinding process for a given component of this composite axis.
        /// </summary>
        public void StartRebinding()
        {
            this.startRebinding.gameObject.SetActive(false);
            this.waitingForInputObject.SetActive(true);
            this.menuController.allowInputChanges = false;

            this.inputAction.action.Disable();
            this.inputAction.action.actionMap.Disable();
            this.rebindingOperation = this.inputAction.action.PerformInteractiveRebinding(0)
                .WithControlsExcluding("<Pointer>/position") // Don't bind to mouse position
                .WithControlsExcluding("<Pointer>/delta")    // To avoid accidental input from mouse motion
                .WithCancelingThrough(this.cancelPath)
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => this.RebindComplete())
                .OnCancel(operation => this.RebindCancel())
                .Start();
        }

        /// <summary>
        /// Cancel the rebinding process for a given component of this composite axis.
        /// </summary>
        public void RebindCancel()
        {
            this.bindingDisplayNameText.text = this.GetKeyReadableName();
            this.rebindingOperation.Dispose();

            this.startRebinding.gameObject.SetActive(true);
            this.waitingForInputObject.SetActive(false);
            this.menuController.allowInputChanges = true;
            this.inputAction.action.Enable();
            this.inputAction.action.actionMap.Enable();
        }

        /// <summary>
        /// Finish the rebinding process for a given component of this composite axis.
        /// </summary>
        public void RebindComplete()
        {
            string overridePath = this.inputAction.action.bindings[0].overridePath;
            foreach (PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                InputAction action = input.actions.FindAction(this.inputAction.name);
                if (action != null)
                {
                    action.ApplyBindingOverride(0, overridePath);
                }
            }

            this.bindingDisplayNameText.text = this.GetKeyReadableName();
            this.rebindingOperation.Dispose();

            PlayerPrefs.SetString(this.InputMappingKey, this.inputAction.action.bindings[0].overridePath);

            this.startRebinding.gameObject.SetActive(true);
            this.waitingForInputObject.SetActive(false);
            this.menuController.allowInputChanges = true;
            this.inputAction.action.Enable();
            this.inputAction.action.actionMap.Enable();
        }

        public void ResetBinding()
        {
            PlayerPrefs.DeleteKey(this.InputMappingKey);
            this.inputAction.action.RemoveAllBindingOverrides();
        }

        public void UpdateDisplay()
        {
            this.bindingDisplayNameText.text = this.GetKeyReadableName();
        }
    }
}
