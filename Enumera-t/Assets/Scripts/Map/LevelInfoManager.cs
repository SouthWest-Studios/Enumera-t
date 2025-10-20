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
    private Queue<DialogueSentence> sentences;
    public LevelInfoCanvasAnimations levelInfoAnimations;
    public Button goLevelButton;

    public TextMeshProUGUI levelDescriptionText;
    public TextMeshProUGUI levelTitleText;
    public Image levelImage;

    private int letterCount = 0;
    public float letterDelay = 0.02f;

    public static LevelInfoManager instance;


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
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextSentences();
        }
    }

    public void StartInfo(Dialogo dialogo, string levelTitle, string levelDescription, Sprite levelImageSprite, UnityEvent onPlayButton = null)
    {
        //anim.SetBool("IsOpen", true);
        if(dialogo.sentences[0].character == null)
        {
            characterImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            characterImage.sprite = dialogo.sentences[0].character;
            characterImage.color = new Color(1, 1, 1, 1);
        }

            
        levelTitleText.text = levelTitle;
        levelDescriptionText.text = levelDescription;
        levelImage.sprite = levelImageSprite;
        goLevelButton.onClick.RemoveAllListeners();
        goLevelButton.onClick.AddListener(onPlayButton.Invoke);
        levelInfoAnimations.PlayEnter();
        dialogueText.text = "";
        sentences.Clear();

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

        if (characterImage.sprite != sentence.character)
        {
            levelInfoAnimations.ChangeCharacter(sentence.character);
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
        
    }
}
