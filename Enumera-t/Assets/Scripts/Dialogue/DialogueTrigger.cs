using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogo dialogo;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogo);
    }



    //borrar
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogo);
        }
        if (Input.GetMouseButtonDown(0))
        {
            FindObjectOfType<DialogueManager>().DisplayNextSentences();
        }
    }

}
