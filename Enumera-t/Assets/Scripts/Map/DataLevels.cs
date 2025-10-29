using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

public class DataLevels : MonoBehaviour
{
    public static DataLevels Instance;

    [System.Serializable]
    public class DataLevel
    {
        public int id;
        public int numbersUnlocked;
        public LevelManager.LevelState state;
        public int starsEarned;
        public int numberOfErrors;

        public DataLevel(int id, LevelManager.LevelState state)
        {
            this.id = id;
            this.state = state;
            this.starsEarned = 0;
            this.numberOfErrors = 0;
        }
    }

    public List<DataLevel> dataLevels = new List<DataLevel>();

    public int currentLevel = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeLevels(int totalLevels)
    {
        // Intentar cargar los datos guardados primero
        bool hasSavedData = false;
        for (int i = 0; i < totalLevels; i++)
        {
            if (PlayerPrefs.HasKey("Level_" + i))
            {
                hasSavedData = true;
                break;
            }
        }

        //Debug.Log($"InitializeLevels called. hasSavedData = {hasSavedData}");


        if (hasSavedData)
        {
            // Hay datos guardados, cargarlos
            dataLevels.Clear();
            for (int i = 0; i < totalLevels; i++)
            {
                LevelManager.LevelState state = (LevelManager.LevelState)PlayerPrefs.GetInt(
                    "Level_" + i,
                    (i == 0 ? (int)LevelManager.LevelState.Unlocked : (int)LevelManager.LevelState.Locked)
                );

                int stars = PlayerPrefs.GetInt($"Level_{i}_Stars", 0);
                int errors = PlayerPrefs.GetInt($"Level_{i}_Errors", 0);
                dataLevels.Add(new DataLevel(i, state)
                {
                    starsEarned = stars,
                    numberOfErrors = errors
                });


            }
        }
        else
        {
            // No hay datos guardados, inicializar lista desde cero
            dataLevels.Clear();
            for (int i = 0; i < totalLevels; i++)
            {
                var state = (i == 0) ? LevelManager.LevelState.Unlocked : LevelManager.LevelState.Locked;
                dataLevels.Add(new DataLevel(i, state));
            }
            SaveLevelsState();
        }
    }


    public void CompleteLevel(int id, int stars, int errors)
    {
        var level = dataLevels.Find(l => l.id == id);
        if (level != null)
        {
            level.state = LevelManager.LevelState.Completed;
            level.starsEarned = Mathf.Max(level.starsEarned, stars); // guarda la mejor puntuación
            level.numberOfErrors = errors;
            // Desbloquear siguiente nivel
            int nextIndex = dataLevels.IndexOf(level) + 1;
            if (nextIndex < dataLevels.Count)
            {
                var nextLevel = dataLevels[nextIndex];
                if (nextLevel.state == LevelManager.LevelState.Locked)
                    nextLevel.state = LevelManager.LevelState.Unlocked;
            }

            SaveLevelsState();
        }
    }

    public LevelManager.LevelState GetLevelState(int id)
    {
        var lvl = dataLevels.Find(l => l.id == id);
        return (lvl != null) ? lvl.state : LevelManager.LevelState.Locked;
    }

    public void SaveLevelsState()
    {
        for (int i = 0; i < dataLevels.Count; i++)
        { 
            PlayerPrefs.SetInt("Level_" + i, (int)dataLevels[i].state);
            PlayerPrefs.SetInt($"Level_{i}_Stars", dataLevels[i].starsEarned);
            PlayerPrefs.SetInt($"Level_{i}_Errors", dataLevels[i].numberOfErrors);
        }

        PlayerPrefs.Save();
    }

    public void LoadLevelsState()
    {
        for (int i = 0; i < dataLevels.Count; i++)
        {
            if (PlayerPrefs.HasKey($"Level_{i}_State"))
                dataLevels[i].state = (LevelManager.LevelState)PlayerPrefs.GetInt($"Level_{i}_State");

            dataLevels[i].starsEarned = PlayerPrefs.GetInt($"Level_{i}_Stars", 0);
            dataLevels[i].numberOfErrors = PlayerPrefs.GetInt($"Level_{i}_Errors", 0);
        }
    }
}
