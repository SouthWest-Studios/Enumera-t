using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PuntuationScript : MonoBehaviour
{
    [Header("Configuración del nivel")]
    public int level;

    [Header("Referencias UI")]
    public Image[] stars;
    public TextMeshProUGUI textMeshProUGUI;

    [Header("Animación")]
    [SerializeField] private float fadeDuration = 0.5f;

    private int numberOfErrors;
    private int starsEarned;

    public void SetStars(int numberErrors)
    {
        numberOfErrors = numberErrors;
        textMeshProUGUI.text = $"Número d'errors: {numberOfErrors}";

        // Determinar cuántas estrellas obtiene este nivel
        if (numberErrors < 2)
            starsEarned = 3;
        else if (numberErrors < 5)
            starsEarned = 2;
        else
            starsEarned = 1;

        // Guarda el resultado del nivel
        SaveLevelScore();

        // Inicia la animación de las estrellas
        StopAllCoroutines();
        StartCoroutine(FadeStars(starsEarned));
    }

    private IEnumerator FadeStars(int starsToLight)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Color targetColor = (i < starsToLight) ? Color.white : Color.black;
            Color startColor = stars[i].color;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                stars[i].color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            stars[i].color = targetColor;
        }
    }

    private void SaveLevelScore()
    {
        // Guarda la puntuación en PlayerPrefs (clave única por nivel)
        PlayerPrefs.SetInt($"Level_{level}_Stars", starsEarned);
        PlayerPrefs.SetInt($"Level_{level}_Errors", numberOfErrors);
        PlayerPrefs.Save();
    }

    public int GetSavedStars()
    {
        return PlayerPrefs.GetInt($"Level_{level}_Stars", 0);
    }

    public int GetSavedErrors()
    {
        return PlayerPrefs.GetInt($"Level_{level}_Errors", 0);
    }

    // Si quieres que al cargar el nivel se muestren las estrellas anteriores
    private void Start()
    {
        int savedStars = GetSavedStars();
        if (savedStars > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FadeStars(savedStars));
        }
    }
}
