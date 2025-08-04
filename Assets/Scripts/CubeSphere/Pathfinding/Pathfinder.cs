
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public static List<GridCell> FindPath(GridCell start, GridCell end, GridManager grid)
    {
        var open = new List<PathNode>();
        var closed = new HashSet<GridCell>();

        var startNode = new PathNode(start);
        startNode.gCost = 0;
        startNode.hCost = Vector3.Distance(start.worldPosition, end.worldPosition);
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

            foreach (var neighbor in GetNeighbors(current.cell, grid))
            {
                if (!neighbor.WalkAble() || closed.Contains(neighbor)) continue;
                var node = new PathNode(neighbor);
                node.parent = current;
                node.gCost = current.gCost + Vector3.Distance(current.cell.worldPosition, neighbor.worldPosition);
                node.hCost = Vector3.Distance(neighbor.worldPosition, end.worldPosition);
                open.Add(node);
            }
        }

        return null;
    }

    static List<GridCell> Reconstruct(PathNode node)
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

    static List<GridCell> GetNeighbors(GridCell cell, GridManager grid)
    {
        List<GridCell> neighbors = new();
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int d = 0; d < 4; d++)
        {
            int nx = cell.x + dx[d];
            int ny = cell.y + dy[d];
            if (nx >= 0 && nx < grid.side && ny >= 0 && ny < grid.side)
            {
                var neighbor = grid.grid[cell.face, nx, ny];
                if (neighbor != null) neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
