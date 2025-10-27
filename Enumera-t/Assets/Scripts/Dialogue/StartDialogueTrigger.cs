using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogueTrigger : MonoBehaviour
{

    public Dialogo startDialogue;
    public Dialogo endDialogue;
    private float contador = 0;
    private bool hasStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("DIALOGUE_START_PLAYED", 0) == 0)
        {
            DialogueManager.instance.StartDialogue(startDialogue);
            PlayerPrefs.SetInt("DIALOGUE_START_PLAYED", 1);
        }
        if (PlayerPrefs.GetInt($"Level_{2}_Stars", 0) > 0 && PlayerPrefs.GetInt("DIALOGUE_END_PLAYED", 0) == 0)
        {
            DialogueManager.instance.StartDialogue(endDialogue);
            PlayerPrefs.SetInt("DIALOGUE_END_PLAYED", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
