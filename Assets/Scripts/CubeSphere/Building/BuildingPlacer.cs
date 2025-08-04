
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public Camera cam;
    public GridManager grid;
    public BuildingManager manager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var cell = hit.collider.GetComponent<GridCell>();
                if (cell != null && manager.CanPlace(cell))
                {
                    manager.Place(cell);
                }
            }
        }
    }
}
