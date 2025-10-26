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

        if (slot == null || locked)
        {

            return;
        }

        AudioManager.Instance.PlayGrab();

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
        transform.position = data.position;
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

        ReparentKeepVisuals(transform, parentAfterDrag);
        transform.localPosition = new Vector3(0f, -2f, 0f);

        if (parentAfterDrag != null)
        {
            
            NumbersSlot slot = parentAfterDrag.GetComponent<NumbersSlot>();
            if (slot != null && slot.isIncognite)
            {
                AudioManager.Instance.PlayDrop();
                FindAnyObjectByType<GameplayManager>().AnswerGuess(number, slot.rowIndex);
            }
            else
            {
                AudioManager.Instance.PlayBadDrop();

            }
        }

    }

    private void ReparentKeepVisuals(Transform child, Transform newParent)
    {
        Vector3 worldScale = child.lossyScale;
        Vector3 worldPos = child.position;

        child.SetParent(newParent, true);

        Vector3 parentScale = newParent.lossyScale;
        Vector3 newLocalScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );

        float avg = (newLocalScale.x + newLocalScale.y) / 2f;
        child.localScale = new Vector3(avg, avg, newLocalScale.z);
        child.position = worldPos;
    }


}
