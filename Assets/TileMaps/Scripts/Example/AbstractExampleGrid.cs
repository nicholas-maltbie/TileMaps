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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Pathfinding;
using nickmaltbie.TileMap.Pathfinding.Visualization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Action for main click to toggle between blocking tiles on the map or selecting for pathfinding.
    /// </summary>
    public enum BoardAction
    {
        SelectPath,
        BlockTile
    }

    /// <summary>
    /// Path modes that can be used when pathfinding in this grid.
    /// </summary>
    public enum PathMode
    {
        AStar,
        DepthFirstSearch,
        BreadthFirstSearch,
        HillClimbing
    }

    /// <summary>
    /// Current mode of the pathfinding.
    /// </summary>
    public enum PathfindingAnimationState
    {
        Playing,
        Paused
    }

    /// <summary>
    /// Example grid of spawned prefabs.
    /// </summary>
    public abstract class AbstractExampleGrid : MonoBehaviour
    {
        /// <summary>
        /// Input action reference for finding the current player cursor position.
        /// </summary>
        [SerializeField]
        private InputActionReference cursorPosition;

        /// <summary>
        /// Input action reference for if the primary action is selected.
        /// </summary>
        [SerializeField]
        private InputActionReference primaryPressed;

        /// <summary>
        /// Input action reference for if the secondary action is selected.
        /// </summary>
        [SerializeField]
        private InputActionReference secondaryPressed;

        /// <summary>
        /// Type of mode for searching for the 
        /// </summary>
        [Tooltip("Path finding mode to use when searching for path.")]
        [SerializeField]
        public PathMode searchMode = PathMode.AStar;

        /// <summary>
        /// World grid containing spawned prefabs.
        /// </summary>
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        /// <summary>
        /// Tile for this grid.
        /// </summary>
        private IBlockableTileMap<Vector2Int, GameObject> tileMap;

        /// <summary>
        /// Prefab to spawn within each square in the grid.
        /// </summary>
        [Tooltip("Prefab to spawn within each square in the grid.")]
        [SerializeField]
        private GameObject tilePrefab;

        /// <summary>
        /// First selected element in path.
        /// </summary>
        protected Nullable<Vector2Int> selected1 = null;

        /// <summary>
        /// Second selected element in path.
        /// </summary>
        protected Nullable<Vector2Int> selected2 = null;

        /// <summary>
        /// Toggle of current state in pathfinding.
        /// </summary>
        protected int toggle = 0;

        /// <summary>
        /// Currently found path.
        /// </summary>
        protected List<Vector2Int> path = new List<Vector2Int>();

        /// <summary>
        /// Step delay between updates of pathfinding rendering.
        /// </summary>
        public float stepDelay = 0.25f;

        /// <summary>
        /// Vertical offset between the board and arrows drawn.
        /// </summary>
        public float arrowOffset = 0.5f;

        /// <summary>
        /// Default color of path elements.
        /// </summary>
        public Color pathDefaultColor = Color.white;

        /// <summary>
        /// Color of path arrows when highlighted.
        /// </summary>
        public Color pathArrowColor = Color.magenta;

        /// <summary>
        /// Color of a tile when it is selected as start or end of path.
        /// </summary>
        public Color selectedTileColor = Color.red;

        /// <summary>
        /// Default color of tile in the board.
        /// </summary>
        public Color defaultTileColor = Color.white;

        /// <summary>
        /// Default color of a tile that has been marked as "searched".
        /// </summary>
        public Color searchedTileColor = Color.green;

        /// <summary>
        /// Color of a tile when it is added to the final path.
        /// </summary>
        public Color pathTileColor = Color.yellow;

        /// <summary>
        /// Color of a tile when it is marked as blocked.
        /// </summary>
        public Color blockedTileColor = Color.blue;

        /// <summary>
        /// Decay of color when displaying priority of next tile to select.
        /// </summary>
        public float priorityDecay = 0.95f;

        /// <summary>
        /// Gradient of colors to select from when displaying priority.
        /// </summary>
        public Gradient priorityGradient;

        /// <summary>
        /// Arrow to spawn between hexes
        /// </summary>
        [SerializeField]
        public GameObject arrowPrefab;

        /// <summary>
        /// Action associated with primary action.
        /// </summary>
        private BoardAction primaryAction = BoardAction.SelectPath;

        /// <summary>
        /// Get the mode selected for the primary button action.
        /// </summary>
        /// <returns>Current mode selected for primary button</returns>
        public BoardAction PrimaryAction => primaryAction;

        /// <summary>
        /// Get the mode selected for the primary button action.
        /// </summary>
        /// <returns>Current mode selected for primary button</returns>
        public BoardAction SecondaryAction => (BoardAction) (1 - PrimaryAction);

        /// <summary>
        /// Toggle primary and secondary actions.
        /// </summary>
        public void ToggleAction()
        {
            primaryAction = SecondaryAction;
        }

        /// <summary>
        /// Set of all locations that have been marked as "searched".
        /// </summary>
        private HashSet<Vector2Int> searched = new HashSet<Vector2Int>();

        /// <summary>
        /// Weights associated with each tile for rendering on the screen.
        /// </summary>
        private Dictionary<Vector2Int, float> tileWeights = new Dictionary<Vector2Int, float>();

        /// <summary>
        /// Arrows that have been created at locations within the grid
        /// </summary>
        private Dictionary<(Vector2Int, Vector2Int), GameObject> arrows =
            new Dictionary<(Vector2Int, Vector2Int), GameObject>();

        /// <summary>
        /// World grid associated with this example.
        /// </summary>
        public IWorldGrid<Vector2Int, GameObject> WorldGrid => this.worldGrid;

        /// <summary>
        /// Tile map prefab object for creating hexes.
        /// </summary>
        public GameObject TilePrefab => this.tilePrefab;

        /// <summary>
        /// Current play mode selected.
        /// </summary>
        private PathfindingAnimationState _currentMode;

        /// <summary>
        /// Current mode of pathfinding animation.
        /// </summary>
        public PathfindingAnimationState CurrentMode
        {
            get => this._currentMode;
            set
            {
                this._currentMode = value;
                this.OnPlayModeChange?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Do we allow the player to input actions.
        /// </summary>
        public bool AllowInputs { get; set; }

        /// <summary>
        /// Event that is invoked whenever the play mode changes.
        /// </summary>
        public EventHandler<PathfindingAnimationState> OnPlayModeChange;

        /// <summary>
        /// Number of queued steps.
        /// </summary>
        private int stepCount;

        /// <summary>
        /// Perform one step of the pathfinding animation.
        /// </summary>
        public void PathfindingStep()
        {
            this.stepCount = 1;
        }

        /// <summary>
        /// Toggle the current animation pathfinding state.
        /// </summary>
        public void TogglePlay()
        {
            this.CurrentMode = (PathfindingAnimationState)(1 - this.CurrentMode);
        }

        /// <summary>
        /// Reset the blocked tiles.
        /// </summary>
        public void ResetBlocked()
        {
            this.tileMap.ResetBlocks();
            foreach (Vector2Int loc in this.tileMap)
            {
                this.UpdateTileColor(loc);
            }
        }

        /// <summary>
        /// Create an arrow on the screen for highlighting the path
        /// </summary>
        /// <param name="start">Source position of the arrow.</param>
        /// <param name="end">Destination position of the arrow.</param>
        /// <returns>Game object representing the created arrow.</returns>
        public GameObject CreateArrow(Vector2Int start, Vector2Int end)
        {
            if (start == null || end == null || start == end)
            {
                return null;
            }

            if (this.arrows.ContainsKey((start, end)))
            {
                return this.arrows[(start, end)];
            }

            Vector3 startPos = this.worldGrid.GetWorldPosition(start);
            Vector3 endPos = this.worldGrid.GetWorldPosition(end);
            Vector3 dir = startPos - endPos;

            var arrow = GameObject.Instantiate(
                this.arrowPrefab,
                (startPos + endPos) / 2 + Vector3.up * this.arrowOffset,
                Quaternion.FromToRotation(Vector3.forward, dir),
                this.transform);

            foreach (MeshRenderer mr in arrow.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.SetColor("_BaseColor", this.pathDefaultColor);
            }

            this.arrows[(start, end)] = arrow;

            return arrow;
        }

        /// <summary>
        /// Delete an arrow for a given location in the grid.
        /// </summary>
        /// <param name="start">Source position of the arrow.</param>
        /// <param name="end">Destination position of the arrow.</param>
        /// <returns>True if an arrow was removed form that position, false otherwise.</returns>
        public bool DeleteArrow(Vector2Int start, Vector2Int end)
        {
            if (this.arrows.ContainsKey((start, end)))
            {
                GameObject arrow = this.arrows[(start, end)];
                this.arrows.Remove((start, end));

                GameObject.Destroy(arrow);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toggle primary and secondary actions for board interactions.
        /// </summary>
        public void SetPrimaryAction(BoardAction primaryAction)
        {
            this.primaryAction = primaryAction;
        }

        /// <summary>
        /// Setup basic actions for the game.
        /// </summary>
        public void Start()
        {
            // Setup block action when player activates block action
            this.primaryPressed.action.performed += _ =>
            {
                if (PrimaryAction == BoardAction.SelectPath)
                {
                    this.DoOnValidPres(this.SelectTile);
                }
                else
                {
                    this.DoOnValidPres(this.BlockTile);
                }
            };

            // Setup select action when player activates select action
            this.secondaryPressed.action.performed += _ => 
            {
                if (SecondaryAction == BoardAction.SelectPath)
                {
                    this.DoOnValidPres(this.SelectTile);
                }
                else
                {
                    this.DoOnValidPres(this.BlockTile);
                }
            };
        }

        /// <summary>
        /// Do an action if a valid tile is pressed.
        /// </summary>
        /// <param name="action">Action to perform for a given tile if the tile pressed is valid.</param>
        public void DoOnValidPres(Action<Vector2Int> action)
        {
            if (!this.AllowInputs)
            {
                return;
            }

            Vector2Int? selected = this.GetSelectedPosition();
            if (selected != null)
            {
                action(selected.Value);
            }
        }

        /// <summary>
        /// Setup the example grid upon enabling.
        /// </summary>
        public void OnEnable()
        {
            // spawn hexes at each position in the world with labeled information.
            (this.worldGrid, this.tileMap) = this.CreateGridMap();
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                var spawned = GameObject.Instantiate(this.TilePrefab) as GameObject;
                spawned.transform.SetParent(this.transform);

                spawned.name = $"({pos.x}, {pos.y})";
                spawned.transform.position = this.worldGrid.GetWorldPosition(pos);
                spawned.transform.rotation = Quaternion.Euler(
                        this.tilePrefab.transform.rotation.eulerAngles +
                        this.worldGrid.GetWorldRotation(pos).eulerAngles);
                spawned.AddComponent<Coord>().coord = pos;
                this.worldGrid.GetTileMap()[pos] = spawned;
                this.UpdateTileColor(pos);
            }

            // Ensure actions are enabled
            this.primaryPressed.action.Enable();
            this.secondaryPressed.action.Enable();
            this.cursorPosition.action.Enable();
        }

        /// <summary>
        /// Cleanup the tile map when this object is disabled.
        /// </summary>
        public void OnDisable()
        {
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                GameObject.Destroy(this.worldGrid.GetTileMap()[pos]);
            }
        }

        /// <summary>
        /// Function to create a world grid
        /// </summary>
        /// <returns>The created world grid and associated blockable tile map for that grid.</returns>
        protected abstract (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap();

        private void ColorTile(Vector2Int loc, Color color)
        {
            this.GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
        }

        public void BlockTile(Vector2Int location)
        {
            // Don't block a searched tile or selected tile
            if (this.searched.Contains(location) ||
                this.tileWeights.ContainsKey(location) ||
                this.selected1 == location ||
                this.selected2 == location)
            {
                return;
            }

            // toggle blocked state
            if (this.tileMap.IsBlocked(location))
            {
                this.tileMap.Unblock(location);
            }
            else
            {
                this.tileMap.Block(location);
            }

            // Update color
            this.UpdateTileColor(location);
        }

        /// <summary>
        /// Get the currently selected position based on the cursor position.
        /// </summary>
        /// <returns>Returns the position of the selected element or null if no valid element is clicked.</returns>
        public Vector2Int? GetSelectedPosition()
        {
            Vector2 mousePosition = this.cursorPosition.action.ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                return null;
            }

            if (hit.collider == null)
            {
                return null;
            }

            Coord coord = hit.collider.gameObject.GetComponent<Coord>();
            if (coord == null)
            {
                return null;
            }

            return coord.coord;
        }

        /// <summary>
        /// Reset the pathfinding progress.
        /// </summary>
        public void ResetProgress()
        {
            this.StopAllCoroutines();
            List<Vector2Int> savedPath = this.path;
            this.path = null;
            // savedPath.ForEach(loc => UpdateTileColor(loc));
            foreach (KeyValuePair<(Vector2Int, Vector2Int), GameObject> key in this.arrows)
            {
                GameObject.Destroy(key.Value);
            }

            this.arrows.Clear();
            var toClear = this.searched.ToList();
            this.searched.Clear();
            foreach (Vector2Int loc in toClear)
            {
                this.UpdateTileColor(loc);
            }

            IEnumerable<Vector2Int> weightsToClear = this.tileWeights.ToList().Select(e => e.Key);
            this.tileWeights.Clear();
            foreach (Vector2Int loc in weightsToClear)
            {
                this.UpdateTileColor(loc);
            }
        }

        /// <summary>
        /// Reset the path the player is currently animating/working on.
        /// </summary>
        public void ClearPath()
        {
            this.StopAllCoroutines();
            this.ResetProgress();

            Vector2Int? temp1 = this.selected1;
            Vector2Int? temp2 = this.selected2;

            // Clear out previous  path
            this.selected1 = null;
            this.selected2 = null;

            if (temp1 != null)
            {
                this.UpdateTileColor(temp1.Value);
            }

            if (temp2 != null)
            {
                this.UpdateTileColor(temp2.Value);
            }
        }

        /// <summary>
        /// clear out any blocked tile in the map.
        /// </summary>
        public void ClearBlockedTiles()
        {
            this.tileMap.ResetBlocks();
        }

        /// <summary>
        /// Resets any visuals drawn on the board currently.
        /// </summary>
        public void ResetBoard()
        {
            this.ClearPath();
            this.ClearBlockedTiles();
        }

        public void SelectTile(Vector2Int location)
        {
            if (this.tileMap.IsBlocked(location))
            {
                return;
            }

            if (this.toggle == 0)
            {
                this.ClearPath();
                // Start new path
                this.selected1 = location;
                this.UpdateTileColor(location);
            }
            else if (this.toggle == 1)
            {
                this.selected2 = location;
                this.UpdateTileColor(location);

                this.DrawPath();
            }

            this.toggle = (this.toggle + 1) % 2;
        }

        /// <summary>
        /// Update the path weight from a given pathfinding step.
        /// </summary>
        /// <param name="step">Pathfinding step.</param>
        public void UpdatePathWeight(PathfindingStep<Vector2Int> step)
        {
            float weight = 1.0f;
            int count = 0;
            var current = new HashSet<Vector2Int>();
            foreach (Path<Vector2Int> path in step.pathOrder.EnumerateElements())
            {
                this.tileWeights[path.Node] = weight;
                // Skip elements that are in the searched group
                // Also skip elements already mentioned in this path order
                bool skip = this.searched.Contains(path.Node) || current.Contains(path.Node);

                if (skip)
                {
                    continue;
                }

                current.Add(path.Node);
                weight *= this.priorityDecay;

                count++;

                this.UpdateTileColor(path.Node);
            }
        }

        /// <summary>
        /// Wait for the next step in the sequence to be ready.
        /// </summary>
        /// <returns>Enumerator which indicates when the next step is ready.</returns>
        public IEnumerator WaitStep()
        {
            // Wait until in playmode or advance if step button is pressed
            if (this.CurrentMode != PathfindingAnimationState.Playing)
            {
                // Or if paused, wait until step once is pressed.
                yield return new WaitUntil(
                    () => this.CurrentMode == PathfindingAnimationState.Playing || this.stepCount > 0);
                this.stepCount = 0;
            }
            else if (this.stepDelay > 0)
            {
                yield return new WaitForSeconds(this.stepDelay);
            }
        }

        /// <summary>
        /// A coroutine to start drawing a path visualization on the screen for a given set of steps.
        /// </summary>
        /// <param name="steps">Steps associated with the path visualization.</param>
        /// <returns>IEnumerator representing the delays between each step of the path rendering.</returns>
        public IEnumerator DrawPathVisualization(IEnumerable<PathfindingStep<Vector2Int>> steps)
        {
            this.searched.Clear();
            this.searched.Add(this.selected1.Value);
            this.searched.Add(this.selected2.Value);
            IEnumerator<PathfindingStep<Vector2Int>> stepEnumerator = steps.GetEnumerator();
            while (stepEnumerator.MoveNext())
            {
                PathfindingStep<Vector2Int> step = stepEnumerator.Current;

                switch (step.stepType)
                {
                    case StepType.StartPath:
                        this.UpdateTileColor(step.currentPath.Node);
                        this.UpdatePathWeight(step);
                        this.stepCount = 0;
                        if (this.CurrentMode != PathfindingAnimationState.Playing)
                        {
                            yield return this.WaitStep();
                        }

                        break;
                    case StepType.AddNode:
                        if (step.currentPath.Previous != null)
                        {
                            this.CreateArrow(step.currentPath.Node, step.currentPath.Previous.Node);
                            this.searched.Add(step.currentPath.Node);
                            this.UpdatePathWeight(step);
                            yield return this.WaitStep();
                        }

                        break;
                    case StepType.EndPath:
                        if (step.pathFound)
                        {
                            if (step.currentPath == null || step.currentPath.Previous == null)
                            {
                                break;
                            }

                            this.searched.Add(step.currentPath.Node);
                            this.CreateArrow(step.currentPath.Node, step.currentPath.Previous.Node);
                            this.UpdateTileColor(step.currentPath.Node);
                            this.UpdatePathWeight(step);

                            // Animate path
                            this.path = step.currentPath.FullPath().ToList();
                            for (int i = 1; i < this.path.Count; i++)
                            {
                                this.UpdateTileColor(this.path[i]);
                                GameObject arrow = this.arrows[(this.path[i], this.path[i - 1])];
                                arrow.transform.position += Vector3.up * 0.001f;
                                foreach (MeshRenderer mr in arrow.GetComponentsInChildren<MeshRenderer>())
                                {
                                    mr.material.SetColor("_BaseColor", this.pathArrowColor);
                                }

                                yield return this.WaitStep();
                            }
                        }
                        else
                        {

                        }

                        break;
                    case StepType.MarkSearched:
                        this.searched.Add(step.currentPath.Node);
                        this.UpdateTileColor(step.currentPath.Node);
                        this.UpdatePathWeight(step);
                        break;
                    case StepType.SkipSearched:
                        if (this.DeleteArrow(step.currentPath.Node, step.currentPath.Previous.Node))
                        {
                            this.UpdatePathWeight(step);
                            yield return this.WaitStep();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Draw the path based on the search mode selected for this demonstration.
        /// </summary>
        public void DrawPath()
        {
            switch (this.searchMode)
            {
                case PathMode.DepthFirstSearch:
                    this.StartCoroutine(this.DrawPathVisualization(
                        this.WorldGrid.GetTileMap().VisualizePathDFS(
                            this.selected1.Value,
                            this.selected2.Value)));
                    break;
                case PathMode.BreadthFirstSearch:
                    this.StartCoroutine(this.DrawPathVisualization(
                        this.WorldGrid.GetTileMap().VisualizePathBFS(
                            this.selected1.Value,
                            this.selected2.Value)));
                    break;
                case PathMode.HillClimbing:
                    Func<Path<Vector2Int>, (int, float)> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                        (-path.Length(), Vector2Int.Distance(path.Node, this.selected2.Value));
                    this.StartCoroutine(this.DrawPathVisualization(
                        this.WorldGrid.GetTileMap().VisualizePathAStar(
                            this.selected1.Value,
                            this.selected2.Value,
                            pathWeightHillClimbing)));
                    break;
                case PathMode.AStar:
                    // Func<Path<Vector2Int>, Tuple<int, float>> pathWeightAStar = (Path<Vector2Int> path) =>
                    //     new Tuple<int, float>(path.Length(), Vector2Int.Distance(path.Node, selected2));
                    Func<Path<Vector2Int>, float> pathWeightAStar = (Path<Vector2Int> path) =>
                        path.Length() + Vector2Int.Distance(path.Node, this.selected2.Value);
                    this.StartCoroutine(this.DrawPathVisualization(
                        this.WorldGrid.GetTileMap().VisualizePathAStar(
                            this.selected1.Value,
                            this.selected2.Value,
                            pathWeightAStar)));
                    break;
            }

            // path.ForEach(loc => UpdateTileColor(loc));
        }

        /// <summary>
        /// Update the color of a tile within the grid.
        /// </summary>
        /// <param name="loc">Location to update within the grid.</param>
        public void UpdateTileColor(Vector2Int loc)
        {
            this.ColorTile(loc, this.GetTileColor(loc));
        }

        /// <summary>
        /// Get the color of a tile within the grid.
        /// </summary>
        /// <param name="loc">Location of a tile within the grid.</param>
        /// <returns>The color that a tile should be based on its current state.</returns>
        public Color GetTileColor(Vector2Int loc)
        {
            if (this.tileMap.IsBlocked(loc))
            {
                return this.blockedTileColor;
            }

            if (loc == this.selected1 || loc == this.selected2)
            {
                return this.selectedTileColor;
            }

            if (this.path != null && this.path.Contains(loc))
            {
                return this.pathTileColor;
            }

            if (this.searched.Contains(loc))
            {
                return this.searchedTileColor;
            }

            if (this.tileWeights.ContainsKey(loc))
            {
                return this.priorityGradient.Evaluate(this.tileWeights[loc]);
            }

            return this.defaultTileColor;
        }

        /// <summary>
        /// Gets the game object that represents a tile for an associated location within the grid.
        /// </summary>
        /// <param name="loc">Location within the grid.</param>
        /// <returns>Tile object at the associated location in the grid.</returns>
        private GameObject GetTile(Vector2Int loc) => this.worldGrid.GetTileMap()[loc];
    }
}
