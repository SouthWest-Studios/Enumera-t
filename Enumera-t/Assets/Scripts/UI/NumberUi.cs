using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int number;
    public GameObject graphics;
    public Image image;

    [HideInInspector] public Transform parentAfterDrag;
    public bool locked = false;

    private bool canDrag = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        canDrag = false; // por defecto no se puede

        // Verifica si el objeto tiene un padre con NumbersSlot
        NumbersSlot slot = GetComponentInParent<NumbersSlot>();

        if (slot == null)
        {

            return;
        }

        // Si el slot es incógnita, tampoco se puede draggear
        if (slot.isIncognite)
        {
            return;
        }

        // Si llega aquí, sí se puede arrastrar
        canDrag = true;

        // Desactiva raycasts de las imágenes
        if (graphics)
        {
            foreach (Image img in graphics.GetComponentsInChildren<Image>(true))
                img.raycastTarget = false;
        }

        image.raycastTarget = false;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!canDrag) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!canDrag) return;

        if (graphics)
        {
            foreach (Image img in graphics.GetComponentsInChildren<Image>(true))
                img.raycastTarget = true;
        }

        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        // Si el padre tiene un NumbersSlot con isIncognite, avisamos al manager
        if (parentAfterDrag != null)
        {
            NumbersSlot slot = parentAfterDrag.GetComponent<NumbersSlot>();
            if (slot != null && slot.isIncognite)
            {
                FindAnyObjectByType<GameplayManager>().AnswerGuess(number, slot.rowIndex);
            }
        }
    }
}
