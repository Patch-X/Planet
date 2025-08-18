using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public int side = SettingsManager.GirdSide;
    public float radius = SettingsManager.PlanetRadius;
    public GameObject cellPrefab;

    [HideInInspector] public string jsonFileName = SettingsManager.expandFileName;
    [HideInInspector] public GridCell[,,] grid;
    [HideInInspector] public bool[,,] unlocked;
    [HideInInspector] public bool[,,] occupied;

    private void Awake()
    {
        //??????????????????
        // 单例模式 + 防止重复创建
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景保留
    }

    void Start()
    {
        LoadGrid();
        Init();
    }

    void LoadGrid()
    {
        grid = new GridCell[6, side, side];
        string path = GetUnlockSavePath();
        if (!File.Exists(path))
        {
            Debug.LogError($"❌ 文件不存在: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);

        foreach (var cell in wrapper.Items)
        {
            GameObject go = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, transform);
            go.name = $"Cell_{cell.face}_{cell.y}_{cell.x}";

            GridCell gridCell = go.GetComponent<GridCell>();
            if (gridCell)
            {
                gridCell.Init(cell.face, cell.x, cell.y, cell.center, cell.corners);
                grid[cell.face, cell.x, cell.y] = gridCell;
            }

        }

        Debug.Log($"✅ 成功加载格子数量: {wrapper.Items.Count}");

        Transform parent = grid[0, 0, 0].transform.parent;
        // 收集所有子物体
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i));
        }

        // 使用字典序排序（Cell_0_10_0 会排在 Cell_0_2_0 前面）
        children.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

        // 设置 sibling 顺序
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
        Debug.Log($"✅ 成功排序");

    }

    public bool TryGetCellFromWorld(Vector3 worldPos, out GridCell cell)
    {
        cell = null;
        float bestDist = float.MaxValue;

        for (int face = 0; face < 6; face++)
        {
            for (int y = 0; y < GridManager.Instance.side; y++)
            {
                for (int x = 0; x < GridManager.Instance.side; x++)
                {
                    GridCell c = GridManager.Instance.grid[face, x, y];
                    if (c == null) continue;

                    float d = Vector3.SqrMagnitude(worldPos - c.worldPosition);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        cell = c;
                    }
                }
            }
        }

        return cell != null;
    }

    private static readonly List<(int face, int x, int y)> excludedList = new()
    {
        (5, 2, 3),
        (5, 3, 3),
        (5, 4, 3),
        (5, 2, 4),
        (5, 3, 4),
        (5, 4, 4),
        (5, 2, 5),
        (5, 3, 5),
        (5, 4, 5),
        (5, 2, 6),
        (5, 3, 6),
        (5, 4, 6),
    };
    private static bool IsExcluded(int face, int x, int y)
    {
        foreach (var e in excludedList)
        {
            if (e.face == face && e.x == x && e.y == y)
                return true;
        }
        return false;
    }
    public bool TryGetRandomAvailableCell(out GridCell cell)
    {
        cell = null;
        List<GridCell> candidates = new List<GridCell>();

        for (int face = 0; face < 6; face++)
        {
            for (int y = 0; y < GridManager.Instance.side; y++)
            {
                for (int x = 0; x < GridManager.Instance.side; x++)
                {
                    if (IsExcluded(face, x, y))
                        continue;

                    GridCell c = GridManager.Instance.grid[face, x, y];
                    if (c == null) continue;

                    if (c.unlocked && c.available)
                        candidates.Add(c);
                }
            }
        }

        if (candidates.Count == 0)
            return false;

        int index = Random.Range(0, candidates.Count);
        cell = candidates[index];
        return true;
    }



    private float clickThresholdTime = 0.3f;
    private float clickThresholdDistance = 10f;

    private Vector3 downPosition;
    private float downTime;
    private bool isDragging = false;

    void Update()
    {
        // === 鼠标控制（编辑器 / PC） ===
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#endif
        // === 触摸控制（移动端） ===
        HandleTouchInput();
    }
    void HandleMouseInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            downPosition = Input.mousePosition;
            downTime = Time.time;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector3.Distance(Input.mousePosition, downPosition) > clickThresholdDistance)
                isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            float heldTime = Time.time - downTime;
            float moveDistance = Vector3.Distance(Input.mousePosition, downPosition);
            if (!isDragging && heldTime <= clickThresholdTime && moveDistance <= clickThresholdDistance)
            {
                TryToClick(Input.mousePosition);
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                downPosition = touch.position;
                downTime = Time.time;
                isDragging = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (!isDragging && Vector2.Distance(touch.position, downPosition) > clickThresholdDistance)
                    isDragging = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float heldTime = Time.time - downTime;
                float moveDistance = Vector2.Distance(touch.position, downPosition);

                if (!isDragging && heldTime <= clickThresholdTime && moveDistance <= clickThresholdDistance)
                {
                    TryToClick(touch.position);
                }
            }
        }
    }

    void TryToClick(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (cell != null)
            {
                int face = cell.face;
                int x = cell.x;
                int y = cell.y;

                if (unlocked[face, x, y] == false)
                {
                    UnlockGrid(face, x, y);
                    // UnlockArea(cell.face, cell.x, cell.y, 1);
                }

                // SaveUnlockState(); // 👈 实时保存
            }
        }
    }

    void UnlockGrid(int face, int x, int y)
    {
        Debug.Log("Unlock");
        grid[face, x, y].Unlock();
        unlocked[face, x, y] = true;
    }

    public static List<(int face, int x, int y)> GetNeighborCells(int face, int x, int y, int radius)
    {
        List<(int face, int x, int y)> neighbors = new List<(int, int, int)>();

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (NeighborUtils.TryGetNeighbor(face, nx, ny, out int f2, out int x2, out int y2))
                {
                    neighbors.Add((f2, x2, y2));
                }
            }
        }

        return neighbors;
    }

    void UnlockArea(int face, int x, int y, int radius)
    {
        Debug.Log("UnlockArea");
        var neighbors = GetNeighborCells(face, x, y, 1);
        foreach (var (f, x2, y2) in neighbors)
        {
            Debug.Log($"邻居格子: face={f}, y={y2}, x={x2}");
            grid[f, x2, y2].Unlock();
        }
    }

    void Init()
    {
        unlocked = new bool[6, side, side];
        occupied = new bool[6, side, side];

        LoadUnlockState();
    }

    private string GetUnlockSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "grid_unlock_state.json");
    }

    //public void SaveUnlockState()
    //{
    //    var saveData = new UnlockStateWrapper();

    //    for (int face = 0; face < 6; face++)
    //    {
    //        for (int x = 0; x < side; x++)
    //        {
    //            for (int y = 0; y < side; y++)
    //            {
    //                if (unlocked[face, x, y])
    //                {
    //                    saveData.unlockedCells.Add(new UnlockInfo { face = face, x = x, y = y });
    //                }
    //            }
    //        }
    //    }

    //    string json = JsonUtility.ToJson(saveData, true);
    //    File.WriteAllText(GetUnlockSavePath(), json);
    //    Debug.Log($"✅ 解锁状态已保存，共 {saveData.unlockedCells.Count} 个格子");
    //}

    void LoadUnlockState()
    {
        string path = GetUnlockSavePath();
        if (!File.Exists(path))
        {
            Debug.Log("⚠️ 没有保存的解锁状态文件，跳过加载");
            return;
        }

        string json = File.ReadAllText(path);
        var saveData = JsonUtility.FromJson<UnlockStateWrapper>(json);
        foreach (var info in saveData.unlockedCells)
        {
            unlocked[info.face, info.x, info.y] = true;

            // 应用到 cell 上显示
            var cell = grid[info.face, info.x, info.y];
            if (cell != null)
            {
                cell.Unlock(); // 隐藏雾等
            }
        }

        Debug.Log($"✅ 加载解锁状态完成，共 {saveData.unlockedCells.Count} 个格子");
    }

}
