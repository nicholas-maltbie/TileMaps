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

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Path modes that can be used when pathfinding in this grid.
    /// </summary>
    public enum PathMode
    {
        DepthFirstSearch,
        BreadthFirstSearch,
        AStar,
        HillClimbing
    }

    /// <summary>
    /// Example grid of spawned prefabs.
    /// </summary>
    public abstract class AbstractExampleGrid : MonoBehaviour
    {
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

        public float stepDelay = 0.25f;
        public float finalPathDelay = 0.05f;

        public float arrowOffset = 0.5f;

        public Color pathDefaultColor = Color.white;

        public Color pathArrowColor = Color.magenta;

        public Color selectedTileColor = Color.red;

        public Color defaultTileColor = Color.white;

        public Color searchedTileColor = Color.green;

        public Color pathTileColor = Color.yellow;

        public Color blockedTileColor = Color.blue;

        public float priorityDecay = 0.95f;

        public Gradient priorityGradient;

        /// <summary>
        /// Arrow to spawn between hexes
        /// </summary>
        [SerializeField]
        public GameObject arrowPrefab;

        private HashSet<Vector2Int> searched = new HashSet<Vector2Int>();

        private Dictionary<Vector2Int, float> tileWeights = new Dictionary<Vector2Int, float>();

        private Dictionary<(Vector2Int, Vector2Int), GameObject> arrows =
            new Dictionary<(Vector2Int, Vector2Int), GameObject>();

        public IWorldGrid<Vector2Int, GameObject> WorldGrid => this.worldGrid;

        public GameObject TilePrefab => this.tilePrefab;

        public GameObject CreateArrow(Vector2Int start, Vector2Int end)
        {
            if (start == null || end == null)
            {
                return null;
            }

            if (this.arrows.ContainsKey((start, end)))
            {
                return null;
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

        public void OnEnable()
        {
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
        }

        public void OnDisable()
        {
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                GameObject.Destroy(this.worldGrid.GetTileMap()[pos]);
            }
        }

        protected abstract (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap();

        private void ColorTile(Vector2Int loc, Color color)
        {
            this.GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
        }

        public void Update()
        {
            if (!(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                return;
            }

            if (hit.collider == null)
            {
                return;
            }

            Coord coord = hit.collider.gameObject.GetComponent<Coord>();
            if (coord == null)
            {
                return;
            }

            Vector2Int selected = coord.coord;

            if (Input.GetButtonDown("Fire2"))
            {
                // toggle blocked state
                if (this.tileMap.IsBlocked(selected))
                {
                    this.tileMap.Unblock(selected);
                }
                else
                {
                    this.tileMap.Block(selected);
                }

                // Update color
                this.UpdateTileColor(selected);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (this.tileMap.IsBlocked(selected))
                {
                    return;
                }

                if (this.toggle == 0)
                {
                    this.StopAllCoroutines();

                    // Clear out previous  path
                    this.selected1 = null;
                    this.selected2 = null;
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

                    // Start new path
                    this.selected1 = selected;
                    this.UpdateTileColor(selected);
                }
                else if (this.toggle == 1)
                {
                    this.selected2 = selected;
                    this.UpdateTileColor(selected);

                    this.DrawPath();
                }

                this.toggle = (this.toggle + 1) % 2;
            }
        }

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
                        break;
                    case StepType.AddNode:
                        if (step.currentPath.Previous != null)
                        {
                            this.CreateArrow(step.currentPath.Node, step.currentPath.Previous.Node);
                            this.searched.Add(step.currentPath.Node);
                            this.UpdatePathWeight(step);
                            if (this.stepDelay > 0)
                            {
                                yield return new WaitForSeconds(this.stepDelay);
                            }
                        }

                        break;
                    case StepType.EndPath:
                        if (step.pathFound)
                        {
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

                                if (this.stepDelay > 0)
                                {
                                    yield return new WaitForSeconds(this.stepDelay);
                                }
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
                            if (this.stepDelay > 0)
                            {
                                yield return new WaitForSeconds(this.stepDelay);
                            }
                        }

                        break;
                }
            }
        }

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
                    Func<Path<Vector2Int>, float> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                        Vector2Int.Distance(path.Node, this.selected2.Value);
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

        public void UpdateTileColor(Vector2Int loc)
        {
            this.ColorTile(loc, this.GetTileColor(loc));
        }

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

        private GameObject GetTile(Vector2Int loc) => this.worldGrid.GetTileMap()[loc];
    }
}
