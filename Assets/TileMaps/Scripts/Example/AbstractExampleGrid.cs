
using System;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Pathfinding;
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
        [SerializeField]
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        /// <summary>
        /// Prefab to spawn within each square in the grid.
        /// </summary>
        [Tooltip("Prefab to spawn within each square in the grid.")]
        [SerializeField]
        private GameObject tilePrefab;

        /// <summary>
        /// First selected element in path.
        /// </summary>
        protected Vector2Int selected1;

        /// <summary>
        /// Second selected element in path.
        /// </summary>
        protected Vector2Int selected2;

        /// <summary>
        /// Toggle of current state in pathfinding.
        /// </summary>
        protected int toggle = 0;

        /// <summary>
        /// Currently found path.
        /// </summary>
        protected List<Vector2Int> path = new List<Vector2Int>();

        public IWorldGrid<Vector2Int, GameObject> WorldGrid => this.worldGrid;

        public GameObject TilePrefab => this.tilePrefab;

        public void Start()
        {
            this.worldGrid = CreateGridMap();

            foreach (Coord coord in GetComponentsInChildren<Coord>())
            {
                this.worldGrid.GetTileMap()[coord.coord] = coord.gameObject;
            }
        }

        public void SetupGridMap()
        {
            this.worldGrid = CreateGridMap();
        }

        protected abstract IWorldGrid<Vector2Int, GameObject> CreateGridMap();

        private void ColorTile(Vector2Int loc, Color color)
        {
            GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        }

        public void Update()
        {

            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
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

                    if (toggle == 0)
                    {
                        path.ForEach(loc => ColorTile(loc, Color.white));
                        selected1 = selected;
                        ColorTile(selected, Color.yellow);
                    }
                    else if (toggle == 1)
                    {
                        selected2 = selected;
                        ColorTile(selected, Color.yellow);

                        switch (searchMode)
                        {
                            case PathMode.DepthFirstSearch:
                                WorldGrid.GetTileMap().FindPathDFS(selected1, selected2, out path);
                                break;
                            case PathMode.BreadthFirstSearch:
                                WorldGrid.GetTileMap().FindPathBFS(selected1, selected2, out path);
                                break;
                            case PathMode.HillClimbing:
                                Func<Path<Vector2Int>, float> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                                    Vector2Int.Distance(path.Node, selected2);
                                WorldGrid.GetTileMap().FindPathAStar(selected1, selected2, pathWeightHillClimbing, out path);
                                break;
                            case PathMode.AStar:
                                // Func<Path<Vector2Int>, Tuple<int, float>> pathWeightAStar = (Path<Vector2Int> path) =>
                                //     new Tuple<int, float>(path.Length(), Vector2Int.Distance(path.Node, selected2));
                                Func<Path<Vector2Int>, float> pathWeightAStar = (Path<Vector2Int> path) =>
                                    path.Length() + Vector2Int.Distance(path.Node, selected2);
                                WorldGrid.GetTileMap().FindPathAStar(selected1, selected2, pathWeightAStar, out path);
                                break;
                        }

                        path.Where(loc => loc != selected1 && loc != selected2)
                            .ToList()
                            .ForEach(loc => ColorTile(loc, Color.red));
                    }

                    toggle = (toggle + 1) % 2;
                }
            }
        }

        private GameObject GetTile(Vector2Int loc) => this.worldGrid.GetTileMap()[loc];
    }
}