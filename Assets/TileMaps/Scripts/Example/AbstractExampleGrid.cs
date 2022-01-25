using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Pathfinding;
using nickmaltbie.TileMap.Pathfinding.PathOrder;
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

            if (arrows.ContainsKey((start, end)))
            {
                return null;
            }

            Vector3 startPos = this.worldGrid.GetWorldPosition(start);
            Vector3 endPos = this.worldGrid.GetWorldPosition(end);
            Vector3 dir = startPos - endPos;

            GameObject arrow = GameObject.Instantiate(
                arrowPrefab,
                (startPos + endPos) / 2 + Vector3.up * arrowOffset,
                Quaternion.FromToRotation(Vector3.forward, dir),
                this.transform);

            arrows[(start, end)] = arrow;

            return arrow;
        }

        public bool DeleteArrow(Vector2Int start, Vector2Int end)
        {
            if (arrows.ContainsKey((start, end)))
            {
                GameObject arrow = arrows[(start, end)];
                arrows.Remove((start, end));

                GameObject.Destroy(arrow);
                return true;
            }
            return false;
        }

        public void OnEnable()
        {
            (worldGrid, tileMap) = CreateGridMap();
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                GameObject spawned = GameObject.Instantiate(this.TilePrefab) as GameObject;
                spawned.transform.SetParent(this.transform);

                spawned.name = $"({pos.x}, {pos.y})";
                spawned.transform.position = this.worldGrid.GetWorldPosition(pos);
                spawned.transform.rotation = Quaternion.Euler(
                        this.tilePrefab.transform.rotation.eulerAngles +
                        this.worldGrid.GetWorldRotation(pos).eulerAngles);
                spawned.AddComponent<Coord>().coord = pos;
                this.worldGrid.GetTileMap()[pos] = spawned;
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
            GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        }

        public void Update()
        {
            if (!(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity))
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
                if (tileMap.IsBlocked(selected))
                {
                    tileMap.Unblock(selected);
                }
                else
                {
                    tileMap.Block(selected);
                }

                // Update color
                UpdateTileColor(selected);
            }
            if (Input.GetButtonDown("Fire1"))
            {
                if (tileMap.IsBlocked(selected))
                {
                    return;
                }

                if (toggle == 0)
                {
                    this.StopAllCoroutines();

                    // Clear out previous  path
                    selected1 = null;
                    selected2 = null;
                    List<Vector2Int> savedPath = path;
                    this.path = null;
                    // savedPath.ForEach(loc => UpdateTileColor(loc));
                    foreach (var key in arrows)
                    {
                        GameObject.Destroy(key.Value);
                    }
                    arrows.Clear();
                    var toClear = this.searched.ToList();
                    this.searched.Clear();
                    foreach (Vector2Int loc in toClear)
                    {
                        UpdateTileColor(loc);
                    }

                    var weightsToClear = this.tileWeights.ToList().Select(e => e.Key);
                    this.tileWeights.Clear();
                    foreach (Vector2Int loc in weightsToClear)
                    {
                        UpdateTileColor(loc);
                    }

                    // Start new path
                    selected1 = selected;
                    UpdateTileColor(selected);
                }
                else if (toggle == 1)
                {
                    selected2 = selected;
                    UpdateTileColor(selected);

                    DrawPath();
                }

                toggle = (toggle + 1) % 2;
            }
        }

        public void UpdatePathWeight(PathfindingStep<Vector2Int> step)
        {
            float weight = 1.0f;
            foreach (Path<Vector2Int> path in step.pathOrder.EnumerateElements())
            {
                tileWeights[path.Node] = weight;
                // Skip elements that are in the searched group
                if (!searched.Contains(path.Node))
                {
                    weight *= priorityDecay;
                }
                UpdateTileColor(path.Node);
            }
        }

        public IEnumerator DrawPathVisualization(IEnumerable<PathfindingStep<Vector2Int>> steps)
        {
            this.searched.Clear();
            this.searched.Add(selected1.Value);
            this.searched.Add(selected2.Value);
            var stepEnumerator = steps.GetEnumerator();
            while (stepEnumerator.MoveNext())
            {
                var step = stepEnumerator.Current;

                switch (step.stepType)
                {
                    case StepType.StartPath:
                        UpdateTileColor(step.currentPath.Node);
                        UpdatePathWeight(step);
                        break;
                    case StepType.AddNode:
                        if (step.currentPath.Previous != null)
                        {
                            CreateArrow(step.currentPath.Node, step.currentPath.Previous.Node);
                            UpdatePathWeight(step);
                            yield return new WaitForSeconds(stepDelay);
                        }

                        break;
                    case StepType.EndPath:
                        if (step.pathFound)
                        {
                            this.searched.Add(step.currentPath.Node);
                            CreateArrow(step.currentPath.Node, step.currentPath.Previous.Node);
                            UpdateTileColor(step.currentPath.Node);
                            UpdatePathWeight(step);

                            // Animate path
                            this.path = step.currentPath.FullPath().ToList();
                            for (int i = 1; i < this.path.Count; i++)
                            {
                                UpdateTileColor(path[i]);
                                GameObject arrow = this.arrows[(path[i], path[i - 1])];
                                foreach (MeshRenderer mr in arrow.GetComponentsInChildren<MeshRenderer>())
                                {
                                    mr.material.SetColor("_Color", this.pathArrowColor);
                                }
                                yield return new WaitForSeconds(finalPathDelay);
                            }
                        }
                        else
                        {

                        }
                        break;
                    case StepType.MarkSearched:
                        this.searched.Add(step.currentPath.Node);
                        UpdateTileColor(step.currentPath.Node);
                        UpdatePathWeight(step);
                        break;
                    case StepType.SkipSearched:
                        if (DeleteArrow(step.currentPath.Node, step.currentPath.Previous.Node))
                        {
                            UpdatePathWeight(step);
                            yield return new WaitForSeconds(stepDelay);
                        }
                        break;
                }

            }
        }

        public void DrawPath()
        {
            switch (searchMode)
            {
                case PathMode.DepthFirstSearch:
                    StartCoroutine(DrawPathVisualization(
                        WorldGrid.GetTileMap().VisualizePathDFS(
                            selected1.Value,
                            selected2.Value)));
                    break;
                case PathMode.BreadthFirstSearch:
                    StartCoroutine(DrawPathVisualization(
                        WorldGrid.GetTileMap().VisualizePathBFS(
                            selected1.Value,
                            selected2.Value)));
                    break;
                case PathMode.HillClimbing:
                    Func<Path<Vector2Int>, float> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                        Vector2Int.Distance(path.Node, selected2.Value);
                    StartCoroutine(DrawPathVisualization(
                        WorldGrid.GetTileMap().VisualizePathAStar(
                            selected1.Value,
                            selected2.Value,
                            pathWeightHillClimbing)));
                    break;
                case PathMode.AStar:
                    // Func<Path<Vector2Int>, Tuple<int, float>> pathWeightAStar = (Path<Vector2Int> path) =>
                    //     new Tuple<int, float>(path.Length(), Vector2Int.Distance(path.Node, selected2));
                    Func<Path<Vector2Int>, float> pathWeightAStar = (Path<Vector2Int> path) =>
                        path.Length() + Vector2Int.Distance(path.Node, selected2.Value);
                    StartCoroutine(DrawPathVisualization(
                        WorldGrid.GetTileMap().VisualizePathAStar(
                            selected1.Value,
                            selected2.Value,
                            pathWeightAStar)));
                    break;
            }

            // path.ForEach(loc => UpdateTileColor(loc));
        }

        public void UpdateTileColor(Vector2Int loc)
        {
            ColorTile(loc, GetTileColor(loc));
        }

        public Color GetTileColor(Vector2Int loc)
        {
            if (tileMap.IsBlocked(loc))
            {
                return blockedTileColor;
            }
            if (loc == selected1 || loc == selected2)
            {
                return selectedTileColor;
            }
            if (path != null && path.Contains(loc))
            {
                return pathTileColor;
            }
            if (this.searched.Contains(loc))
            {
                return searchedTileColor;
            }
            if (this.tileWeights.ContainsKey(loc))
            {
                return priorityGradient.Evaluate(this.tileWeights[loc]);
            }
            return defaultTileColor;
        }

        private GameObject GetTile(Vector2Int loc) => this.worldGrid.GetTileMap()[loc];
    }
}