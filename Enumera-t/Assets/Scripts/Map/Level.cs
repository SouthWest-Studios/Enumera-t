using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static LevelManager;

public class Level : MonoBehaviour
{
    public int id;
    public LevelState state;
    public Image image;
    public int unlockedNumbersToBe;

    
    public string levelTitle;
    [TextArea(3, 10)]
    public string levelDescription;
    public Sprite levelImage;
    public GameObject characterAnimatedSlot;

    public Dialogo dialogueBeforeEnter;
    public Dialogo dialogueInGameOne;
    public Dialogo dialogueInGameTwo;
    public Dialogo dialogueInGameThree;

    public UnityEvent onFinishDialogueBeforeEnter;

    [Header("UI de puntuación")]
    public List<Image> stars;
    public float fadeDuration = 0.4f;
    TextMeshProUGUI errorsText;


    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        //int savedStars = DataLevels.Instance.dataLevels[id].starsEarned;

        //if (savedStars > 0)
        //{
        //    StartCoroutine(FadeStars(savedStars));
        //    for (int i = 0; i < stars.Count; i++)
        //        stars[i].gameObject.SetActive(true);
        //}
        //else
        //{
        //    for (int i = 0; i < stars.Count; i++)
        //        stars[i].gameObject.SetActive(false);
        //}
    }
    public void PlayDialogueLevel()
    {
        //DialogueManager.instance.StartDialogue(dialogueBeforeEnter, onFinishDialogueBeforeEnter);
        LevelInfoManager.instance.StartInfo(dialogueBeforeEnter, levelTitle, levelDescription, levelImage, this, DataLevels.Instance.dataLevels[id].starsEarned, onFinishDialogueBeforeEnter);
        LevelData.dialogueInGameOne = dialogueInGameOne;
        LevelData.dialogueInGameTwo = dialogueInGameTwo;
        LevelData.dialogueInGameThree = dialogueInGameThree;
        LevelData.instance.levelId = id;
        LevelData.instance.numbersUnlocked = unlockedNumbersToBe;
        LevelData.instance.levelComplete = false;
    }

    public void SetStarsByErrors(int errors)
    {
        int starsEarned = 1;

        if (errors < 2)
            starsEarned = 3;
        else if (errors < 5)
            starsEarned = 2;

        // Guardar datos de nivel
        DataLevels.Instance.CompleteLevel(id, starsEarned, errors);

        StopAllCoroutines();
        StartCoroutine(FadeStars(starsEarned));
        errorsText.text = "Errores:" + errors;
    }

    public IEnumerator FadeStars(int starsToLight)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].gameObject.SetActive(true); // Asegura que estén visibles
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


    //public Level(int id, LevelState state, Color color)
    //{
    //    this.id = id;
    //    this.state = state;
    //    this.image.color = color;
    //}
}


