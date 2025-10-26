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
                lvl.image.color = Color.grey;
                break;
            case LevelState.Unlocked:
                lvl.image.color = Color.green;
                break;
            case LevelState.Completed:
                lvl.image.color = Color.blue;
                break;
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
            SceneManager.LoadScene("Gameplay");
            DataLevels.Instance.currentLevel = levelId;
        }
       
    }
}
