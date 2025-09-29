using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumbersSlot : MonoBehaviour, IDropHandler
{
    public bool isIncognite = false;
    public void OnDrop(PointerEventData eventData)
    {
        NumberUi draggedItem = eventData.pointerDrag.GetComponent<NumberUi>();

        if (draggedItem == null) return;

        if (transform.childCount == 0)
        {
            draggedItem.parentAfterDrag = transform;
        }
        else
        {
            Transform existingChild = transform.GetChild(0);

            existingChild.SetParent(draggedItem.parentAfterDrag);
            existingChild.localPosition = Vector3.zero;

            draggedItem.parentAfterDrag = transform;
        }
    }

}
