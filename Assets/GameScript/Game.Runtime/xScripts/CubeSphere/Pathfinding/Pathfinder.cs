using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public static List<GridCell> FindPath(GridCell start, GridCell end)
    {
        var open = new List<PathNode>();
        var closed = new HashSet<GridCell>();

        var startNode = new PathNode(start)
        {
            gCost = 0,
            hCost = Vector3.Distance(start.worldPosition, end.worldPosition)
        };
        open.Add(startNode);

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.fCost.CompareTo(b.fCost));
            var current = open[0];
            open.RemoveAt(0);
            closed.Add(current.cell);

            if (current.cell == end)
            {
                return Reconstruct(current);
            }

            foreach (var neighbor in GetNeighbors(current.cell))
            {
                if (!neighbor.WalkAble() || closed.Contains(neighbor)) continue;

                var node = new PathNode(neighbor)
                {
                    parent = current,
                    gCost = current.gCost + Vector3.Distance(current.cell.worldPosition, neighbor.worldPosition),
                    hCost = Vector3.Distance(neighbor.worldPosition, end.worldPosition)
                };

                open.Add(node);
            }
        }

        return null; // 没有路径
    }

    private static List<GridCell> Reconstruct(PathNode node)
    {
        List<GridCell> path = new();
        while (node != null)
        {
            path.Add(node.cell);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static List<GridCell> GetNeighbors(GridCell cell)
    {
        List<GridCell> neighbors = new();
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int d = 0; d < 4; d++)
        {
            int nx = cell.x + dx[d];
            int ny = cell.y + dy[d];

            if (NeighborUtils.TryGetNeighbor(cell.face, nx, ny, out int f2, out int x2, out int y2))
            {
                var neighbor = GridManager.Instance.grid[f2, x2, y2];
                if (neighbor != null) neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
