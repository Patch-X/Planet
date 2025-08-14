
using UnityEngine;
using System.Collections.Generic;
public class GridCell : MonoBehaviour
{
    [HideInInspector] public int face, x, y;
    [HideInInspector] public Vector3 worldPosition;
    [HideInInspector] public Vector3[] corners; // 4个顶点（传入赋值）
    [HideInInspector] public bool unlocked = false;
    [HideInInspector] public bool available = true;

    public GameObject landObject;
    public GameObject fogObject;

    public void Init(int face, int x, int y, Vector3 pos, Vector3[] corners)
    {
        this.unlocked = false;
        this.available = true;

        this.face = face;
        this.x = x;
        this.y = y;
        this.worldPosition = pos;
        this.corners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            this.corners[i] = corners[i];
        }

        SetQuadMesh(landObject);
        SetBigQuadMesh(fogObject);
    }

    public bool WalkAble()
    {
        return unlocked && available;
    }
    public void Unlock()
    {
        unlocked = true;
        if (fogObject != null)
            fogObject.SetActive(false); // 隐藏迷雾
        // 也可以切换主材质/播放动画等
    }

    private void SetQuadMesh(GameObject go)
    {
        if (corners == null || corners.Length != 4) return;

        // 1. 计算中心点和平面法线
        Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
        Vector3 normal = Vector3.Cross(corners[1] - corners[0], corners[2] - corners[0]).normalized;

        // 2. 投影到该平面并按极角排序
        Vector3 refDir = (corners[0] - center).normalized;
        var sorted = new List<Vector3>(corners);
        sorted.Sort((a, b) =>
        {
            Vector3 dirA = (a - center).normalized;
            Vector3 dirB = (b - center).normalized;
            float angleA = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, dirA), normal), Vector3.Dot(refDir, dirA));
            float angleB = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, dirB), normal), Vector3.Dot(refDir, dirB));
            return angleA.CompareTo(angleB);
        });

        Vector3[] quad = sorted.ToArray();

        // 3. 创建mesh（两个三角形，顺时针/逆时针不重要，只要统一）
        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = quad;
        mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        var collider = gameObject.GetComponent<MeshCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();

        collider.sharedMesh = mesh;
    }

    private void SetBigQuadMesh(GameObject go)
    {
        bool isFog = true;

        if (corners == null || corners.Length != 4) return;

        // 1. 计算中心点和平面法线
        Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
        Vector3 normal = Vector3.Cross(corners[1] - corners[0], corners[2] - corners[0]).normalized;

        // 2. 投影到该平面并按极角排序
        Vector3 refDir = (corners[0] - center).normalized;
        var sorted = new List<Vector3>(corners);
        sorted.Sort((a, b) =>
        {
            Vector3 dirA = (a - center).normalized;
            Vector3 dirB = (b - center).normalized;
            float angleA = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, dirA), normal), Vector3.Dot(refDir, dirA));
            float angleB = Mathf.Atan2(Vector3.Dot(Vector3.Cross(refDir, dirB), normal), Vector3.Dot(refDir, dirB));
            return angleA.CompareTo(angleB);
        });

        Vector3[] quad = sorted.ToArray();

        if (isFog)
        {
            float expandFactor = 1.05f;
            float offset = 0.1f;

            for (int i = 0; i < quad.Length; i++)
            {
                Vector3 fromCenter = quad[i] - center;
                quad[i] = center + fromCenter * expandFactor + normal * offset;
            }
        }

        // 设置 GameObject 的世界位置为中心
        go.transform.position = center;

        // 顶点局部化（相对 transform.position）
        for (int i = 0; i < quad.Length; i++)
        {
            quad[i] = quad[i] - center;
        }

        // 构建 mesh
        Mesh mesh = new Mesh();
        mesh.vertices = quad;
        mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.RecalculateNormals();

        var mf = go.GetComponent<MeshFilter>() ?? go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
    }

}
