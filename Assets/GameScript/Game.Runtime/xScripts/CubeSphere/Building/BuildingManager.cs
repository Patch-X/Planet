
using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public GridManager grid;
    private HashSet<GridCell> occupied = new();

    public bool CanPlace(GridCell cell) => !occupied.Contains(cell);

    public void Place(GridCell cell)
    {
        if (!CanPlace(cell)) return;
        occupied.Add(cell);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = cell.worldPosition;
        cube.transform.localScale = Vector3.one * 0.8f;
    }
}
