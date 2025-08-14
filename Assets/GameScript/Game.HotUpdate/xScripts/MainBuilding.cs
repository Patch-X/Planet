using UnityEngine;
using System.Collections;
public class MainBuilding : MonoBehaviour
{
    public GameObject workingObject = null;
    public GameObject completeObject = null;


    public Vector3 worldPosition => transform.position;

    IEnumerator Start()
    {
        // 等待直到 Manager 初始化完毕
        yield return new WaitUntil(() => MainBuildingManager.Instance != null);
        MainBuildingManager.Instance.Register(this);
    }

    void OnDisable()
    {
        MainBuildingManager.Instance?.Unregister(this);
    }

    public void CheckHidden(bool hidden)
    {
        if (hidden)
        {
            if (workingObject != null)
                workingObject.SetActive(true);
            if (completeObject != null)
                completeObject.SetActive(false);
        }
        else
        {
            if (workingObject != null)
                workingObject.SetActive(false);
            if (completeObject != null)
                completeObject.SetActive(true);
        }
    }
}