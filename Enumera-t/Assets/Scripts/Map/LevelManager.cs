using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public enum LevelState
    {
        Locked,
        Unlocked,
        Completed
    }

    [Header("Lista de Niveles en la escena")]
    public List<Level> levels = new List<Level>();

    public static LevelManager instance;

    public Color lockedColor;
    public Color unlockedColor;
    public Color completeColor;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        
        DataLevels.Instance.InitializeLevels(levels.Count);

        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var lvl in levels)
        {
            lvl.state = DataLevels.Instance.GetLevelState(lvl.id);
            UpdateLevelColor(lvl);
        }
    }

    private void UpdateLevelColor(Level lvl)
    {
        switch (lvl.state)
        {
            case LevelState.Locked:
                lvl.image.color = lockedColor;
                break;
            case LevelState.Unlocked:
                lvl.image.color = unlockedColor;
                break;
            case LevelState.Completed:
                lvl.image.color = completeColor;
                break;
        }
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].state == LevelState.Locked)
            {
                levels[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                levels[i].GetComponent<Button>().interactable = true;
            }
        }


        int savedStars = DataLevels.Instance.dataLevels[lvl.id].starsEarned;
        lvl.StopAllCoroutines();
        lvl.StartCoroutine(lvl.FadeStars(savedStars));
    }

    public void LoadLevelScene(int levelId)
    {
        AudioManager.Instance.PlayLevelStart();
        if (levels[levelId].state != LevelState.Locked)
        {
            DataLevels.Instance.dataLevels[levelId].numbersUnlocked = levels[levelId].unlockedNumbersToBe;          
            DataLevels.Instance.currentLevel = levelId;
            TransitionCanvas.instance.DoTransition("Gameplay");
        }
       
    }

    public void LoadLevelSceneSimple()
    {
        TransitionCanvas.instance.DoTransition("GameplayInfinite");

    }
}
