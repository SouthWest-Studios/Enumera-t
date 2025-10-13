using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberUi : MonoBehaviour, IBeginDragHandler, IDragHandler ,IEndDragHandler
{

    public int number;

    public GameObject graphics;

    public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    public bool locked = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (graphics)
        {
            Image[] images = graphics.GetComponentsInChildren<Image>(true);

            foreach (Image img in images)
            {
                img.raycastTarget = false;
            }
        }
        
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
        if (graphics)
        {
            Image[] images = graphics.GetComponentsInChildren<Image>(true);

            foreach (Image img in images)
            {
                img.raycastTarget = true;
            }
        }
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        if (parentAfterDrag != null)
        {
            if(parentAfterDrag.gameObject.GetComponent<NumbersSlot>().isIncognite)
            {
                FindAnyObjectByType<GameplayManager>().AnswerGuess(number, parentAfterDrag.gameObject.GetComponent<NumbersSlot>().rowIndex);
            }
        }
    }

}
