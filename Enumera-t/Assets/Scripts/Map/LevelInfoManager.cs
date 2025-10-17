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

    private int letterCount = 0;
    public float letterDelay = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartText(Dialogo dialogo)
    {
        //anim.SetBool("IsOpen", true);
        characterImage.sprite = dialogo.sentences[0].character;


        dialogueText.text = "";
        sentences.Clear();

        foreach (DialogueSentence sentence in dialogo.sentences)
        {
            sentences.Enqueue(sentence);
        }
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
            //dialogueAnimations.ChangeCharacter(sentence.character);
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
                    //dialogueAnimations.NudgeCharacter();
                    letterCount = 0;
                }
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        //dialogueAnimations.NudgeDialogueBox();
    }

    void EndDialogue()
    {
        
    }
}
