using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    private Queue<DialogueSentence> sentences;
    public DialogueCanvasAnimations dialogueAnimations;

    private UnityEvent onDialogueFinish;

    public static DialogueManager instance;

    private int letterCount = 0;
    public float letterDelay = 0.02f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<DialogueSentence>();
    }

    public void StartDialogue(Dialogo dialogo, UnityEvent onDialogueFinish = null)
    {
        //anim.SetBool("IsOpen", true);

        dialogueAnimations.PlayEnter();
        this.onDialogueFinish = onDialogueFinish;

        sentences.Clear();
        foreach(DialogueSentence sentence in dialogo.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueSentence sentence = sentences.Dequeue();
        StopAllCoroutines();
        characterImage.sprite = sentence.character;
        StartCoroutine(TypeSentence(sentence.sentence));
    }

    IEnumerator TypeSentence (String sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if(letterCount > 5)
            {
                dialogueAnimations.NudgeCharacter();
                letterCount = 0;
            }
            
            letterCount++;
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
    }

    void EndDialogue()
    {
        dialogueAnimations.PlayExit();

        if (onDialogueFinish != null)
        {
            onDialogueFinish.Invoke();
            onDialogueFinish = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            dialogueAnimations.NudgeCharacter();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            dialogueAnimations.NudgeDialogueBox();
        }
    }

}
