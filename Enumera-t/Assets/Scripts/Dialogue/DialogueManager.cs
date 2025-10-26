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
    public GameObject characterAnimatedSlot;
    private Queue<DialogueSentence> sentences;
    public DialogueCanvasAnimations dialogueAnimations;

    private UnityEvent onDialogueFinish;
    private string currentCharacter = "";

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
        
        foreach (Transform child in characterAnimatedSlot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Instantiate(dialogo.sentences[0].characterAnimated, characterAnimatedSlot.transform);

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


        if(currentCharacter != sentence.characterAnimated.name)
        {
            currentCharacter = sentence.characterAnimated.name;
            dialogueAnimations.ChangeCharacter(sentence.characterAnimated);
        }
        StartCoroutine(TypeSentence(sentence.sentence));
    }

    IEnumerator TypeSentence (String sentence)
    {
        dialogueText.text = "";
        yield return new WaitForSeconds(0.4f);
        foreach (char letter in sentence.ToCharArray())
        {
            if(letter == ' ')
            {
                letterCount++;
                if (letterCount >= 2)
                {
                    dialogueAnimations.NudgeCharacter();
                    letterCount = 0;
                }
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        dialogueAnimations.NudgeDialogueBox();
    }

    void EndDialogue()
    {
        dialogueAnimations.PlayExit();

        if (onDialogueFinish != null)
        {
            currentCharacter = "";
            onDialogueFinish.Invoke();
            onDialogueFinish = null;
        }
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextSentences();
        }
    }

}
