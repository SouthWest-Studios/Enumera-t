using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelManager;

public class Level : MonoBehaviour
{
    public int id;
    public LevelState state;
    public Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    //public Level(int id, LevelState state, Color color)
    //{
    //    this.id = id;
    //    this.state = state;
    //    this.image.color = color;
    //}
}
