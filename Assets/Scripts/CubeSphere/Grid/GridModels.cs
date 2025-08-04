
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GridQuadInfo
{
    public Vector3[] corners;   // 四个顶点
    public Vector3 center;      // 中心点
    public Vector3 normal;      // 法线
    public int face;
    public int x;
    public int y;
}
[System.Serializable]
public class Wrapper
{
    public List<GridQuadInfo> Items;
}


[System.Serializable]
public class UnlockStateWrapper
{
    public List<UnlockInfo> unlockedCells = new();
}

[System.Serializable]
public class UnlockInfo
{
    public int face;
    public int x;
    public int y;
}