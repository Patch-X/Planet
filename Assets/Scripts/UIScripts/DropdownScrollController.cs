using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownScrollController : Dropdown
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        // 检查点击对象是否为Scrollbar或其子对象
        if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Scrollbar>())
            return; // 忽略Scrollbar的点击，不执行后续逻辑

        base.OnPointerClick(eventData);
        if (!IsInteractable() || !IsActive())
        {
            return;
        }
        Scrollbar scrollbar = gameObject.GetComponentInChildren<ScrollRect>()?.verticalScrollbar;
        if (scrollbar != null && options.Count > 1)

        {

            scrollbar.value = Mathf.Max(0.001f, 1.0f - (float)value / (options.Count - 1));
        }

    }
}
