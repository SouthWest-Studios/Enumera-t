using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]


public struct DialogueSentence
{
    [TextArea(3, 10)]
    public string sentence;
    public Sprite character;
}
[System.Serializable]
public class Dialogo{
    public DialogueSentence[] sentences;

}
