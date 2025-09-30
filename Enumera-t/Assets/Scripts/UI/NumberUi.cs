using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberUi : MonoBehaviour, IBeginDragHandler, IDragHandler ,IEndDragHandler
{

    public int number;

    public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    public bool locked = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        if (parentAfterDrag != null)
        {
            if(parentAfterDrag.gameObject.GetComponent<NumbersSlot>().isIncognite)
            {
                FindAnyObjectByType<GameplayManager>().AnswerGuess(number);
            }
        }
    }

}
