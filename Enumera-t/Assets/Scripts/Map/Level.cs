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

    
    public string levelTitle;
    [TextArea(3, 10)]
    public string levelDescription;
    public Sprite levelImage;

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
        //DialogueManager.instance.StartDialogue(dialogueBeforeEnter, onFinishDialogueBeforeEnter);
        LevelInfoManager.instance.StartInfo(dialogueBeforeEnter, levelTitle, levelDescription, levelImage, onFinishDialogueBeforeEnter);
        LevelData.dialogueInGameOne = dialogueInGameOne;
        LevelData.dialogueInGameTwo = dialogueInGameTwo;
        LevelData.dialogueInGameThree = dialogueInGameThree;
    }

    //public Level(int id, LevelState state, Color color)
    //{
    //    this.id = id;
    //    this.state = state;
    //    this.image.color = color;
    //}
}
