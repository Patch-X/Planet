using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MainBuildingManager : MonoBehaviour
{
    private List<MainBuilding> targets = new();
    public float checkInterval = 0.5f;
    private float timer = 0f;

    public static MainBuildingManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Register(MainBuilding t)
    {
        if (!targets.Contains(t))
        {
            targets.Add(t);
        }
    }

    public void Unregister(MainBuilding t) => targets.Remove(t);

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < checkInterval) return;
        timer = 0;

        foreach (var t in targets)
        {
            if (!GridManager.Instance.TryGetCellFromWorld(t.worldPosition, out GridCell cell)) continue;

            if (cell)
            {
                bool covered = !cell.unlocked;

                t.CheckHidden(covered);
            }
        }
    }
}
