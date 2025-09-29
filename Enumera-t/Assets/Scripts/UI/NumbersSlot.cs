using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumbersSlot : MonoBehaviour, IDropHandler
{
    public bool isIncognite = false;
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            NumberUi inventoryItem = eventData.pointerDrag.GetComponent<NumberUi>();
            inventoryItem.parentAfterDrag = transform;
        }
    }

}
