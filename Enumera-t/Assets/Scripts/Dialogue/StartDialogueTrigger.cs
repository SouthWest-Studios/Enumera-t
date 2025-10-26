using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogueTrigger : MonoBehaviour
{

    public Dialogo startDialogue;
    private float contador = 0;
    private bool hasStarted = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
        if (contador > 0.5) {
            if (!hasStarted)
            {
                hasStarted = true;
                DialogueManager.instance.StartDialogue(startDialogue);
            }
        }
        else
        {
            contador += Time.deltaTime;
        }
    }
}
