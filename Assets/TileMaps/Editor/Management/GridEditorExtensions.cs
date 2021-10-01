
using nickmaltbie.TileMap.Example;
using UnityEditor;
using UnityEngine;

namespace nickmaltbie.TileMap.Management
{
    [CustomEditor(typeof(AbstractExampleGrid), true)]
    public class GridEditorExtensions : Editor
    {
        private AbstractExampleGrid grid;

        private void OnEnable()
        {
            this.grid = (AbstractExampleGrid) target;
        }

        public void CreateGrid()
        {
            // Check if tile map currently exists, if so, remove it
            foreach (Coord coord in grid.GetComponentsInChildren<Coord>())
            {
                GameObject.DestroyImmediate(coord.gameObject);
            }

            this.grid.SetupGridMap();

            foreach (Vector2Int pos in this.grid.WorldGrid.GetTileMap())
            {
                GameObject spawned = PrefabUtility.InstantiatePrefab(this.grid.TilePrefab) as GameObject;
                spawned.transform.SetParent(this.grid.transform);

                spawned.name = $"({pos.x}, {pos.y})";
                spawned.transform.position = this.grid.WorldGrid.GetWorldPosition(pos);
                spawned.transform.rotation = Quaternion.Euler(
                        this.grid.TilePrefab.transform.rotation.eulerAngles +
                        this.grid.WorldGrid.GetWorldRotation(pos).eulerAngles);
                spawned.AddComponent<Coord>().coord = pos;
                this.grid.WorldGrid.GetTileMap()[pos] = spawned;
            }
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Create Grid"))
            {
                CreateGrid();
            }

            // Draw default inspector after button...
            base.OnInspectorGUI();
        }
    }
}
