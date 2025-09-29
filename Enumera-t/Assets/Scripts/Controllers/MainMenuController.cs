using UnityEngine;
using TMPro;

public class TitleUIAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.05f;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color pulseColor = Color.cyan;

    private Vector3 initialScale;

    void Start()
    {
        if (title != null)
            initialScale = title.rectTransform.localScale;

        if (title != null)
            title.color = baseColor;
    }

    void Update()
    {
        if (title == null) return;

        // Rotación suave en Y
        title.rectTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Pulso de escala (respiración)
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        title.rectTransform.localScale = initialScale * scaleFactor;

        // Cambio de color progresivo
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        title.color = Color.Lerp(baseColor, pulseColor, t);
    }
}
