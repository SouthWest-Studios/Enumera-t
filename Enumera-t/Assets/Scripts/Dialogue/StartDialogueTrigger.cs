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
        DialogueManager.instance.StartDialogue(startDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (contador > 0.1) {
            if (!hasStarted)
            {
                hasStarted = true;
                
            }
        }
        else
        {
            contador += Time.deltaTime;
        }
    }
}
