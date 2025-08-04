
using UnityEngine;

public class PathNode
{
    public GridCell cell;
    public PathNode parent;
    public float gCost, hCost;
    public float fCost => gCost + hCost;

    public PathNode(GridCell cell)
    {
        this.cell = cell;
    }
}
