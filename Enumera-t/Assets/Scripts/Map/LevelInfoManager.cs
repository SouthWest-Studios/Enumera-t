using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelInfoManager : MonoBehaviour
{

    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public GameObject characterAnimatedSlot;
    private Queue<DialogueSentence> sentences;
    public LevelInfoCanvasAnimations levelInfoAnimations;
    public Button goLevelButton;
    private string currentCharacter = "";

    public TextMeshProUGUI levelDescriptionText;
    public TextMeshProUGUI levelTitleText;
    public Image levelImage;

    private int letterCount = 0;
    public float letterDelay = 0.02f;

    public static LevelInfoManager instance;

    private bool hasTouched = false;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<DialogueSentence>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && !hasTouched))
        {
            hasTouched = true;
            DisplayNextSentences();
        }
        if (Input.touchCount == 0)
        {
            hasTouched = false;
        }
    }

    public void StartInfo(Dialogo dialogo, string levelTitle, string levelDescription, Sprite levelImageSprite, Level level, int savedStars, UnityEvent onPlayButton = null)
    {
        AudioManager.Instance.PlayOpenPanel();
        //anim.SetBool("IsOpen", true);

        
        foreach (Transform child in characterAnimatedSlot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        var go = Instantiate(dialogo.sentences[0].characterAnimated, characterAnimatedSlot.transform);
        go.GetComponent<RectTransform>().localScale = new Vector2(8, 8);
        



        levelTitleText.text = levelTitle;
        levelDescriptionText.text = levelDescription;
        levelImage.sprite = levelImageSprite;
        goLevelButton.onClick.RemoveAllListeners();
        goLevelButton.onClick.AddListener(onPlayButton.Invoke);
        levelInfoAnimations.PlayEnter();
        dialogueText.text = "";
        sentences.Clear();

        level.StartCoroutine(level.FadeStars(savedStars));

        foreach (DialogueSentence sentence in dialogo.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueSentence sentence = sentences.Dequeue();
        StopAllCoroutines();

        if (currentCharacter != sentence.characterAnimated.name)
        {
            currentCharacter = sentence.characterAnimated.name;
            levelInfoAnimations.ChangeCharacter(sentence.characterAnimated);
        }
        StartCoroutine(TypeSentence(sentence.sentence));
    }

    IEnumerator TypeSentence(String sentence)
    {
        dialogueText.text = "";
        yield return new WaitForSeconds(0.4f);
        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == ' ')
            {
                letterCount++;
                if (letterCount >= 2)
                {
                    levelInfoAnimations.NudgeCharacter();
                    letterCount = 0;
                }
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        levelInfoAnimations.NudgeDialogueBox();
    }

    void EndDialogue()
    {
        currentCharacter = "";
    }
}
