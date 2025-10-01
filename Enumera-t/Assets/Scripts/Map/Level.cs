using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static LevelManager;

public class Level : MonoBehaviour
{
    public int id;
    public LevelState state;
    public Image image;

    public Dialogo dialogueBeforeEnter;
    public Dialogo dialogueInGameOne;
    public Dialogo dialogueInGameTwo;
    public Dialogo dialogueInGameThree;

    public UnityEvent onFinishDialogueBeforeEnter;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void PlayDialogueLevel()
    {
        DialogueManager.instance.StartDialogue(dialogueBeforeEnter, onFinishDialogueBeforeEnter);
    }

    //public Level(int id, LevelState state, Color color)
    //{
    //    this.id = id;
    //    this.state = state;
    //    this.image.color = color;
    //}
}
