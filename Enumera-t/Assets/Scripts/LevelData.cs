using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{

    public static LevelData instance;

    public static Dialogo dialogueInGameOne;
    public static Dialogo dialogueInGameTwo;
    public static Dialogo dialogueInGameThree;




    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
}
