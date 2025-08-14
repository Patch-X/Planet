using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CubeSphereExporter : MonoBehaviour
{
    string exportFile = SettingsManager.expandFileName;
    int gridSize = SettingsManager.GirdSide;

    [System.Serializable]
    public class GridQuadInfo
    {
        public Vector3[] corners;   // 四个顶点，每个是Vector3
        public Vector3 center;      // 中心点
        public Vector3 normal;      // 法线
        public int face;
        public int x;
        public int y;
    }

    [ContextMenu("Export All Quads")]
    public void ExportAllQuads()
    {
        List<GridQuadInfo> allQuads = new List<GridQuadInfo>();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Length; i += 6)
        {
            int[] idx = new int[] {
                triangles[i + 0], triangles[i + 1], triangles[i + 2],
                triangles[i + 3], triangles[i + 4], triangles[i + 5]
            };

            // 去重，得到四个不同的顶点索引
            List<int> quadArray = new List<int>();
            foreach (var t in idx)
                if (!quadArray.Contains(t)) quadArray.Add(t);

            if (quadArray.Count != 4)
            {
                Debug.LogWarning($"Quad not 4 points! Found {quadArray.Count} at {i}");
                continue;
            }

            Vector3[] quadVerts = new Vector3[4]
            {
                localToWorld.MultiplyPoint3x4(vertices[quadArray[0]]),
                localToWorld.MultiplyPoint3x4(vertices[quadArray[1]]),
                localToWorld.MultiplyPoint3x4(vertices[quadArray[2]]),
                localToWorld.MultiplyPoint3x4(vertices[quadArray[3]])
            };

            GridQuadInfo quad = new GridQuadInfo();
            quad.corners = quadVerts;

            Vector3 center = (quadVerts[0] + quadVerts[1] + quadVerts[2] + quadVerts[3]) / 4f;
            quad.center = center;

            Vector3 right = (quadVerts[1] - quadVerts[0]).normalized;
            Vector3 forward = (quadVerts[2] - quadVerts[0]).normalized;
            Vector3 normal = Vector3.Cross(right, forward).normalized;
            quad.normal = normal;

            int quadsPerFace = gridSize * gridSize;
            int quadsPerFaceDouble = quadsPerFace * 2;
            int quadIdx = i / 6;
            int axisCoordinate = quadIdx / quadsPerFaceDouble; // 0 Z, 1 X, 2 Y
            int face = axisCoordinate * 2;
            int localIdx = quadIdx % quadsPerFaceDouble;
            int x = localIdx % gridSize;
            int yIndex = localIdx / gridSize;
            int y = yIndex / 2;
            bool bBack = yIndex % 2 == 1;
            if (bBack)
                face++;

            if (axisCoordinate == 2)
            {
                int faceIndex = quadIdx / quadsPerFace;
                face = faceIndex + 1;
                if (face > 5) face = 4;
                y = quadIdx % quadsPerFace / 10;
            }

            // int quadIdx = i / 6;
            // int face = quadIdx / quadsPerFace;
            // int localIdx = quadIdx % quadsPerFace;
            // int y = localIdx / gridSize;
            // int x = localIdx % gridSize;

            quad.face = face;
            quad.x = x;
            quad.y = y;
            Debug.Log("face:" + face + ",x:" + x + ",y:" + y);

            allQuads.Add(quad);
        }

        string json = JsonHelper.ToJson<GridQuadInfo>(allQuads.ToArray(), true);
        Debug.Log(json);
        File.WriteAllText(Path.Combine(Application.dataPath, exportFile), json);

        Debug.Log($"导出Cube Sphere格子数: {allQuads.Count} 已保存到: {exportFile}");
    }
}

public static class JsonHelper
{
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
