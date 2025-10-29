using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameplayManager;
using Unity.VisualScripting;
using System.Linq;
using TMPro;

public enum BossType { None, Bessones, Drac, Barrufet }

public class GameplayManager : MonoBehaviour
{
    public int level;

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;

    public Canvas gameplayCanvas;

    public int enemyNumber;

    public int bossNumber;
    public int bossNumber2;

    public Transform enemyTransf;

    public GameObject solutionSlot;

    

    public Transform operationNumberTransf;
    public Transform operationNumberParentTransf;

    public int operationNumber;

    public Image OperationSymbolImage;

    public bool sums = true;

    public List<NumbersSlot> slots;

    public List<int> numbersList;

    public int unlockedNumbersInList;

    [HideInInspector] public List<int> alreadyUsedNumbers = new List<int>();

    public int maxRoundsBeforeBoss;
    private int roundsBeforeBoss = 0;

    public bool isBoss = false;
    public float health = 2;
    public float maxHealth;
    private bool firstHealth = false;
    [HideInInspector] public int damage = 0;
    public Image healthBar;
    public BossType bossType = BossType.None;
    private IBossBehavior bossBehavior;
    [HideInInspector]  public bool victory1;
    [HideInInspector]  public bool victory2;

    [HideInInspector] public List<GameObject> temporalPrefab = new List<GameObject>();

    private IOperationMode operationMode;

    [HideInInspector] public GameObject bossPrefab;
    public Transform bossTransf;

    [Header("Boss doble operación")]
    public GameObject firstOperationCanvas;
    public GameObject secondOperationCanvas;
    public Transform enemyTransf2;
    public List<GameObject> slots2;
    public GameObject solutionSlot2;
    public Transform operationNumberTransf2;
    public Image operationSymbolImage2;

    public int enemyNumber2;
    [HideInInspector] public int operationNumber2;

    public List <GameObject> numbersListPrefab;

    [Header("Operacion triple")]
    public Transform operationNumberTransf3;
    [HideInInspector] public int secondOperationNumber;
    public Transform secondOperationNumberTransf;
    public Image secondSymbol;
    public GameObject solutionBossSlot;
    public GameObject solutionBossSlot2;
    public GameObject solutionBossSlot3;
    public GameObject solutionBossSlot4;
    [HideInInspector] public List<GameObject> bouOperationNumbers  = new List<GameObject>();

    public Transform backGroundTransf;

    public List<GameObject> bossList;

    public GameObject number4;

    

    [Header("Ventana de Puntuacion ")]
    [HideInInspector] public int numberOfErrors = 0;
    public GameObject puntuationWindow;
    public List<Image> stars;
    public TextMeshProUGUI textErrors;

    public int maxRoundsBeforeDialogue;
    private int roundsBeforeDialogue;
    private bool hasPlayedDialogue = false;
    public Transform numberPuntuationTransf;
    public HorizontalLayoutGroup numbersLayout;
    public bool isInfinite;

    public interface IBossBehavior
    {

        void Init(GameplayManager manager);

        void GenerateOperation();
        void OnCorrectAnswer(int index);
        void OnWrongAnswer();  
        void Update();

        bool CheckAnswer(int number, int operationIndex);

        void OnAnswer(int number, int operationIndex);
    }



    void Start()
    {
        numberOfErrors = 0;

        
        //level data
        if (LevelData.instance != null && !isInfinite)
        {
            unlockedNumbersInList = LevelData.instance.numbersUnlocked;
            level = LevelData.instance.levelId + 1;
        }


        string backGroundPath = "";
        string path = "";


        switch (level)
        {
            case 1:
                level1.SetActive(true);
                numbersLayout.spacing = -450;
                backGroundPath = "Prefabs/BackGrounds/Jail-Background";
                
                path = "Prefabs/Enemies/Bruixa";
 

                Transform firstOperation = FindChildRecursive(level1.transform, "1rstOperation");
                Transform secondOperation = FindChildRecursive(level1.transform, "2ndOperation");

                if (firstOperation != null)
                {
                    operationNumberTransf = FindChildRecursive(firstOperation, "OperationNumberImage");
                    enemyTransf = FindChildRecursive(firstOperation, "EnemyImage");
                    var opSymbol = FindChildRecursive(firstOperation, "OperationSymbol");
                    if (opSymbol != null) OperationSymbolImage = opSymbol.GetComponent<Image>();
                    var solSlot = FindChildRecursive(firstOperation, "SolutionSlot");
                    if (solSlot != null) solutionSlot = solSlot.gameObject;
                }

                if (secondOperation != null)
                {
                    operationNumberTransf2 = FindChildRecursive(secondOperation, "OperationNumberImage2");
                    enemyTransf2 = FindChildRecursive(secondOperation, "EnemyImage2");
                    var opSymbol2 = FindChildRecursive(secondOperation, "OperationSymbol2");
                    if (opSymbol2 != null) operationSymbolImage2 = opSymbol2.GetComponent<Image>();
                    var solSlot2 = FindChildRecursive(secondOperation, "SolutionSlot2");
                    if (solSlot2 != null) solutionSlot2 = solSlot2.gameObject;
                }

                operationMode = new OperationModeLevel1();
                operationMode.Init(this);
                bossType = BossType.Bessones;
                break;

            case 2:
                level2.SetActive(true);
                numbersLayout.spacing = -250;
                backGroundPath = "Prefabs/BackGrounds/Cave-Background";
                path = "Prefabs/Enemies/Drac";

                GameObject prefab2 = Resources.Load<GameObject>(path);



                Transform firstOperationLevel2 = FindChildRecursive(level2.transform, "1rstOperation");

                if (firstOperationLevel2 != null)
                {
                    operationNumberTransf = FindChildRecursive(firstOperationLevel2, "OperationNumberImage").transform;
                    enemyTransf = FindChildRecursive(firstOperationLevel2, "EnemyImage").transform;

                    var opSymbol = FindChildRecursive(firstOperationLevel2, "OperationSymbol");
                    if (opSymbol != null) OperationSymbolImage = opSymbol.GetComponent<Image>();

                    var solSlot = FindChildRecursive(firstOperationLevel2, "SolutionSlot");
                    if (solSlot != null) solutionSlot = solSlot.gameObject;
                }

                Transform secondOperationLevel2 = FindChildRecursive(FindChildRecursive(level2.transform, "1rstOperation"), "BossExtra");

                if (secondOperationLevel2 != null)
                {
                    secondOperationNumberTransf = FindChildRecursive(secondOperationLevel2, "OperationNumberImage2").transform;
                    // Dos símbolos de operación
                    var opSymbol2 = FindChildRecursive(secondOperationLevel2, "OperationSymbol2");

                    if (opSymbol2 != null) secondSymbol = opSymbol2.GetComponent<Image>();

                }

                operationMode = new OperationModeLevel2();
                operationMode.Init(this);
                bossType = BossType.Drac;
                break;

            //───────────────────────────────
            case 3:
                level3.SetActive(true);
                numbersLayout.spacing = -150;
                backGroundPath = "Prefabs/BackGrounds/BullIntestine-Background";
                path = "Prefabs/Enemies/Bou";

                Transform firstOperationLevel3 = FindChildRecursive(level3.transform, "1rstOperation");

                if (firstOperationLevel3 != null)
                {
                    // En nivel 3 hay 3 números, dos operationNumber y un enemy
                    operationNumberTransf = FindChildRecursive(firstOperationLevel3, "OperationNumberImage").transform;
                    operationNumberTransf2 = FindChildRecursive(firstOperationLevel3, "OperationNumberImage2").transform;
                    operationNumberTransf3 = FindChildRecursive(firstOperationLevel3, "OperationNumberImage3").transform;
                    secondOperationNumberTransf = FindChildRecursive(firstOperationLevel3, "OperationNumberImage2").transform;

                    enemyTransf = FindChildRecursive(firstOperationLevel3, "EnemyImage").transform;

                    // Dos símbolos de operación
                    var opSymbol1 = FindChildRecursive(firstOperationLevel3, "OperationSymbol");
                    var opSymbol2 = FindChildRecursive(firstOperationLevel3, "OperationSymbol2");
                    if (opSymbol1 != null) OperationSymbolImage = opSymbol1.GetComponent<Image>();
                    if (opSymbol2 != null) operationSymbolImage2 = opSymbol2.GetComponent<Image>();

                    // Slot de la solución (donde el jugador pondrá el número faltante)
                    var solSlot = FindChildRecursive(firstOperationLevel3, "SolutionSlot");
                    if (solSlot != null) solutionSlot = solSlot.gameObject;
                }

                operationMode = new OperationModeLevel3();
                operationMode.Init(this);
                bossType = BossType.Barrufet;
                break;
            default:
                break;
        }

        GameObject backgroundPrefab = Resources.Load<GameObject>(backGroundPath);

        GameObject newBackground = Instantiate(backgroundPrefab, gameplayCanvas.transform, false);
        newBackground.transform.position = backGroundTransf.position;
        newBackground.transform.SetSiblingIndex(0);



        health = maxHealth;
        healthBar.fillAmount = health / 10f;

        RoundCompleted(1);


        for (int i = 0; i < slots.Count; i++)
        {
            if(i < unlockedNumbersInList)
            {
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().number = numbersList[i];
            }
            else
            {
                slots[i].transform.GetChild(0).gameObject.SetActive(false);
                slots[i].gameObject.SetActive(false);

            }
            
        }


        if (LevelData.dialogueInGameOne != null && LevelData.dialogueInGameOne.sentences.Length > 0)
        {
            DialogueManager.instance.StartDialogue(LevelData.dialogueInGameOne);
        }
    }

    void Update()
    {
        bossBehavior?.Update();
    }

    public void AssignNumberPrefab(int number,Transform transf, bool good, Transform parentTransform, bool isSolution = false)
    {

        string path;
        if (good)
        {
            path = "Prefabs/Numbers/Bons/Number" + number;
        }
        else
        {
            path = "Prefabs/Numbers/Dolents/Number" + number + "Bad";
        }

        GameObject prefab = Resources.Load<GameObject>(path);

        GameObject numberPrefab = Instantiate(prefab, transf.position, transf.rotation);

        numberPrefab.transform.SetParent(parentTransform);
        numberPrefab.transform.localScale = transf.localScale;


        if (level == 3 && isBoss && !isSolution)
        {
            bouOperationNumbers.Add(numberPrefab);
        }
        else
        {
            temporalPrefab.Add(numberPrefab);
        }

        

        
    }

    public void AnswerGuess(int number, int operationIndex)
    {

        bool correct = false;
        // Si hay boss activo, delega la comprobación al boss
        if (isBoss && bossBehavior != null)
        {
            correct = bossBehavior.CheckAnswer(number, operationIndex);
            if(!correct)
            {
                bossBehavior.OnAnswer(number, operationIndex);
                return;
            }
            
        }
        else
        {
            correct = operationMode.CheckAnswer(number, operationIndex);
        }
        
        // Modo normal (sin boss)

        if (correct)
        {
            CorrectAnswerNormal();
        }
        else
        {
            numberOfErrors++;
            WrongNumberToSlot(operationIndex, true);
        }
    }

    private void CorrectAnswerNormal()
    {
        AudioManager.Instance.PlayCorrect();

        roundsBeforeBoss++;
        roundsBeforeDialogue++;
        for (int i = 0; i < temporalPrefab.Count; i++)
        {
            if (temporalPrefab[i] != null)
                UnityEngine.Object.Destroy(temporalPrefab[i]);
        }
        temporalPrefab.Clear();

        if (roundsBeforeDialogue >= maxRoundsBeforeDialogue && !hasPlayedDialogue)
        {
            if (level == 1)
            {
                if (LevelData.dialogueInGameTwo != null && LevelData.dialogueInGameOne.sentences.Length > 0)
                {
                    DialogueManager.instance.StartDialogue(LevelData.dialogueInGameTwo);
                }
            }
            else if(level == 2) 
            {
                
            }
            else
            {

            }
            
            hasPlayedDialogue = true;
        }


        if (roundsBeforeBoss >= maxRoundsBeforeBoss && !isBoss)
        {
            ActivateBoss();
            bossList[level - 1].SetActive(true);
            healthBar.transform.parent.gameObject.SetActive(true);

            if (level == 1)
            {
                if (LevelData.dialogueInGameThree != null && LevelData.dialogueInGameThree.sentences.Length > 0)
                {
                    AudioManager.Instance.musicSource.loop = false;
                    AudioManager.Instance.PlayIntroBoss();
                    DialogueManager.instance.StartDialogue(LevelData.dialogueInGameThree, AudioManager.Instance.PlayBossFight);
                    StartCoroutine(WaitForIntroToEnd());
                }
            }
            else if(level == 2)
            {
                if (LevelData.dialogueInGameTwo != null && LevelData.dialogueInGameTwo.sentences.Length > 0)
                {
                    AudioManager.Instance.musicSource.loop = false;
                    AudioManager.Instance.PlayIntroBoss();
                    DialogueManager.instance.StartDialogue(LevelData.dialogueInGameTwo, AudioManager.Instance.PlayBossFight);
                    StartCoroutine(WaitForIntroToEnd());
                }
            }
            else
            {
                if (LevelData.dialogueInGameTwo != null && LevelData.dialogueInGameTwo.sentences.Length > 0)
                {
                    AudioManager.Instance.musicSource.loop = false;
                    AudioManager.Instance.PlayIntroBoss();
                    DialogueManager.instance.StartDialogue(LevelData.dialogueInGameTwo, AudioManager.Instance.PlayBossFight);
                    StartCoroutine(WaitForIntroToEnd());
                }
            }
            
            
        }
            

        print("siguiente");

        RoundCompleted(1);

        
        

        

        
    }

    private IEnumerator WaitForIntroToEnd()
    {

        yield return new WaitForSeconds(AudioManager.Instance.musicSource.clip.length);

        if(!AudioManager.Instance.musicSource.isPlaying)
        {
            AudioManager.Instance.PlayBossFight();
            AudioManager.Instance.musicSource.loop = true;
        }
        
    }

    private void ReparentKeepVisuals(Transform child, Transform newParent)
    {
        Vector3 worldScale = child.lossyScale;
        Vector3 worldPos = child.position;

        child.SetParent(newParent, true);

        Vector3 parentScale = newParent.lossyScale;
        Vector3 newLocalScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );

        float avg = (newLocalScale.x + newLocalScale.y) / 2f;
        child.localScale = new Vector3(avg, avg, newLocalScale.z);
        child.position = worldPos;
    }




    // Función auxiliar para devolver número al slot
    private void ReparentKeepWorldScale(Transform child, Transform newParent)
    {

        Vector3 worldScale = child.lossyScale;


        child.SetParent(newParent, true);   


        Vector3 parentScale = newParent.lossyScale;
        child.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }

    public void WrongNumberToSlot(int operationIndex, bool toLock)
    {
        AudioManager.Instance.PlayWrong();
        GameObject targetSlot = (operationIndex == 2) ? solutionSlot2 : solutionSlot;
        if (targetSlot == null) return;

        for (int i = 0; i < unlockedNumbersInList; i++)
        {
            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {
                var ui = targetSlot.transform.GetChild(0).GetComponent<NumberUi>();
                if (toLock)
                {
                    ui.locked = true;
                    Transform prohibit = ui.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(t => t.name == "prohibit");

                    if (prohibit != null)
                        prohibit.gameObject.SetActive(true);
                    else
                    {
                        //Debug.LogWarning("No se encontró 'prohibit'");
                    }
                        
                }

                Transform child = targetSlot.transform.GetChild(0);
                ReparentKeepVisuals(child, slots[i].transform);

                break;
            }
        }
    }

    public void RestoreNumberToSlot(GameObject targetSlot, bool toLock = false)
    {
        if (targetSlot == null || !targetSlot.activeInHierarchy) return;

        for (int i = 0; i < unlockedNumbersInList; i++)
        {
            if (slots[i] != null && slots[i].transform.childCount > 0)
            {
                var ui = slots[i].transform.GetChild(0).GetComponent<NumberUi>();
                Transform prohibit = ui.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name == "prohibit");

                if (targetSlot.transform.childCount > 0)
                {
                    var ui2 = targetSlot.transform.GetChild(0).GetComponent<NumberUi>();
                    Transform prohibit2 = ui2.GetComponentsInChildren<Transform>(true)
                        .FirstOrDefault(t => t.name == "prohibit");

                    if (toLock)
                    {
                        ui2.locked = true;
                        if (prohibit2 != null)
                            prohibit2.gameObject.SetActive(true);
                        else
                        {
                            ////Debug.LogWarning("No se encontró 'prohibit'");
                        }

                    }
                }

                if (!toLock)
                {
                    ui.locked = false;
                    if (prohibit != null)
                        prohibit.gameObject.SetActive(false);
                    else
                    {
                        ////Debug.LogWarning("No se encontró 'prohibit'");
                    }

                }
            }

            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {
                Transform child = targetSlot.transform.GetChild(0);
                ReparentKeepVisuals(child, slots[i].transform);
            }
        }
    }
    public void RoundCompleted(int operationIndex)
    {
        // Caso de operación secundaria del boss
        if (operationIndex == 2)
        {
            RestoreNumberToSlot(solutionSlot2);
            return;
        }

        // Verificar si el boss fue derrotado
        if (isBoss)
        {
            health -= damage;
            if (health > 0)
            {
                if(firstHealth)
                {
                    StartCoroutine(AnimarBarra(health / maxHealth));
                    if(health == 6)
                    {
                        if (LevelData.dialogueInGameOne != null && LevelData.dialogueInGameOne.sentences.Length > 0)
                        {
                            DialogueManager.instance.StartDialogue(LevelData.dialogueInGameMID);
                        }
                    }
                }
                else
                {
                    if(!firstHealth)
                    {
                        health = maxHealth;
                    }
                    if(health == 10)
                    {
                        firstHealth = true;
                    }
                    
                }
                
                bossBehavior.GenerateOperation();
            }
            else
            {
                StartCoroutine(AnimarBarra(health / maxHealth));
                
                Animator bossAnimaton = null;
                for (int i = 0; i < bossList.Count; i++)
                {
                    if (bossList[i].activeInHierarchy)
                    {
                        bossAnimaton = bossList[i].GetComponent<Animator>();
                    }
                }
                
                if (bossAnimaton != null)
                {
                    // Inicia la animación del boss
                    bool hasLoseTrigger = false;

                    foreach (var param in bossAnimaton.parameters)
                    {
                        if (param.name == "Lose" && param.type == AnimatorControllerParameterType.Trigger)
                        {
                            hasLoseTrigger = true;
                            break;
                        }
                    }

                    if (hasLoseTrigger)
                    {
                        bossAnimaton.SetTrigger("Lose");
                    }
                    else
                    {
                        if (LevelData.instance != null && !isInfinite)
                        {
                            int errors = numberOfErrors;
                            int starsEarned = 1;

                            if (errors < 2)
                                starsEarned = 3;
                            else if (errors < 5)
                                starsEarned = 2;

                            // Guardamos progreso completo
                            DataLevels.Instance.CompleteLevel(LevelData.instance.levelId, starsEarned, errors);
                            LevelData.instance.levelComplete = true;
                        }

                        //SceneManager.LoadScene("MapScene");
                        TransitionCanvas.instance.DoTransition("MapScene");
                    }

                    // Inicia una corrutina que esperará hasta que acabe la animación
                    //Debug.Log("Antes de StartCoroutine, activo: " + this.gameObject.activeInHierarchy);
                    healthBar.transform.parent.gameObject.SetActive(false);
                    level1.SetActive(false);
                    level2.SetActive(false);
                    
                    StartCoroutine(WaitForAnimationAndGoToMap(bossAnimaton));
                }
                else
                {
                    if (LevelData.instance != null && !isInfinite)
                    {
                        int errors = numberOfErrors;
                        int starsEarned = 1;

                        if (errors < 2)
                            starsEarned = 3;
                        else if (errors < 5)
                            starsEarned = 2;

                        // Guardamos progreso completo
                        DataLevels.Instance.CompleteLevel(LevelData.instance.levelId, starsEarned, errors);
                        LevelData.instance.levelComplete = true;
                    }

                    TransitionCanvas.instance.DoTransition("MapScene");
                }
                
                return;
            }
            
        }
        else
        {
            // Delegar generación de operaciones al modo de nivel
            if (operationMode != null)
            {
                operationMode.GenerateOperation();
            }
        }

        

        // Restaurar números a los slots
        RestoreNumberToSlot(solutionSlot);
    }


    private void ActivateBoss()
    {
        isBoss = true;
        roundsBeforeBoss = 0;

        switch (bossType)
        {
            case BossType.Bessones:
                secondOperationCanvas.SetActive(true);
                bossBehavior = new BossBessones();
                
                break;
            case BossType.Drac:
                bossBehavior = new BossDrac();
                break;
            case BossType.Barrufet:
                bossBehavior = new BossBou();
                
                break;
        }

        bossBehavior?.Init(this);
        if (bossType == BossType.Barrufet)
        {
            BossBou bouLogic = bossBehavior as BossBou;
            if (bouLogic != null)
            {
                bossList[2].GetComponent<BossBouAnimationEvents>().bossLogic = bouLogic;
            }
            else
            {
                //Debug.LogError("No se pudo castear bossBehavior a BossBou");
            }
        }
    }

    public Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private IEnumerator AnimarBarra(float valorObjetivo)
    {
        float valorInicial = healthBar.fillAmount;
        float duracion = 0.5f; 
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            healthBar.fillAmount = Mathf.Lerp(valorInicial, valorObjetivo, t);
            float oscilacion = Mathf.Sin(tiempo * 2f * Mathf.PI * 2);
            float intensidad = Mathf.Abs(oscilacion);
            healthBar.color = Color.Lerp(Color.white, Color.red, intensidad);
            tiempo += Time.deltaTime;
            yield return null;
        }

        healthBar.fillAmount = valorObjetivo;
    }

    public void PlayOperationEntryAnimation(GameObject operationContainer)
    {
        StartCoroutine(OperationEntryCoroutine(operationContainer));
    }


    private IEnumerator OperationEntryCoroutine(GameObject container)
    {
        CanvasGroup group = container.GetComponent<CanvasGroup>();
        if (group == null)
            group = container.AddComponent<CanvasGroup>();

        RectTransform rect = container.GetComponent<RectTransform>();

        float duration = 0.3f;
        float time = 0f;
        Vector3 startScale = Vector3.one * 0.6f;
        Vector3 endScale = container.transform.localScale;

        group.alpha = 0f;
        rect.localScale = startScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            rect.localScale = Vector3.Lerp(startScale, endScale, t);
            group.alpha = t;

            yield return null;
        }

        rect.localScale = endScale;
        group.alpha = 1f;
    }

    private IEnumerator WaitForAnimationAndGoToMap(Animator bossAnimator)
    {
        //Debug.Log("Coroutine entró, gameObject activo: " + gameObject.activeInHierarchy + ", enabled: " + enabled);
        //Debug.Log("Boss animator actual state: " + bossAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);

        yield return null;
        for (int i = 0; i < bossAnimator.layerCount; i++)
        {
            var clips = bossAnimator.GetCurrentAnimatorClipInfo(i);
            if (clips.Length > 0)
            {
                string clipName = clips[0].clip.name;
                //Debug.Log($"Layer {i}: clip activo = {clipName}");
            }
            else
            {
                //Debug.Log($"Layer {i}: no hay clip activo");
            }
        }

        yield return new WaitUntil(() =>
        {
            var clips = bossAnimator.GetCurrentAnimatorClipInfo(0);
            return clips.Length > 0 && clips[0].clip.name == "loseAnimation";
        });

        AnimatorStateInfo stateInfo = bossAnimator.GetCurrentAnimatorStateInfo(0);
        float duration = stateInfo.length;
        float triggerMoment = duration * 0.9f;
        yield return new WaitForSeconds(triggerMoment);
        AudioManager.Instance.StopMusicSource();
        AudioManager.Instance.PlayVictory();
        if (LevelData.instance != null && !isInfinite)
        {
            int errors = numberOfErrors;
            int starsEarned = 1;

            if (errors < 2)
                starsEarned = 3;
            else if (errors < 5)
                starsEarned = 2;

            // Guardamos progreso completo
            DataLevels.Instance.CompleteLevel(LevelData.instance.levelId, starsEarned, errors);
            LevelData.instance.levelComplete = true;
        }
        if (LevelData.dialogueInGameOne != null && LevelData.dialogueInGameOne.sentences.Length > 0)
        {
            level3.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                bossList[i].gameObject.SetActive(false);
            }
            DialogueManager.instance.StartDialogue(LevelData.dialogueInGameVICTORY, SetStarsByErrors);
        }
        else
        {
            level3.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                bossList[i].gameObject.SetActive(false);
            }
            puntuationWindow.SetActive(true);
            if (numberOfErrors <= 0)
            {
                textErrors.text = "PERFECTE!";
                textErrors.alignment = TextAlignmentOptions.Center;
            }
            else if (numberOfErrors > 9)
            {
                textErrors.text = "Continua Aprentent!";
            }
            else if(numberOfErrors == 1)
            {
                textErrors.text = "     Error";
                AssignNumberPrefab(numberOfErrors, numberPuntuationTransf, true, puntuationWindow.transform);
            }
            else
            {
                textErrors.text = "     Errors";
                AssignNumberPrefab(numberOfErrors, numberPuntuationTransf, true, puntuationWindow.transform);
            }
        }
    }



    public void SetStarsByErrors()
    {
        puntuationWindow.SetActive(true);
        PlayOperationEntryAnimation(puntuationWindow);
        AudioManager.Instance.PlayOpenPanel();

        int starsEarned = 1;

        if (numberOfErrors < 2)
            starsEarned = 3;
        else if (numberOfErrors < 5)
            starsEarned = 2;

        //StopAllCoroutines();
        StartCoroutine(FadeStars(starsEarned));
        if (numberOfErrors <= 0)
        {
            textErrors.text = "PERFECTE!";
            textErrors.alignment = TextAlignmentOptions.Center;
        }
        else if (numberOfErrors > 9)
        {
            textErrors.text = "Continua Aprentent!";
        }
        else if (numberOfErrors == 1)
        {
            textErrors.text = "     Error";
            AssignNumberPrefab(numberOfErrors, numberPuntuationTransf, true, puntuationWindow.transform);
        }
        else
        {
            textErrors.text = "     Errors";
            AssignNumberPrefab(numberOfErrors, numberPuntuationTransf, true, puntuationWindow.transform);
        }


        FadeStars(starsEarned);

    }


    public IEnumerator FadeStars(int starsToLight)
    {
        if (starsToLight == 1)
        {
            AudioManager.Instance.PlayOneStar();
        }
        else if (starsToLight == 2)
        {
            AudioManager.Instance.PlayTwoStar();
        }
        else
        {
            AudioManager.Instance.PlayThreeStar();
        }
        float fadeDuration = 0.3f;
        float waitBetweenStars = 0.5f;
        float waitBetweenStars2 = 0.5f;

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].gameObject.SetActive(true);
            Color targetColor = (i < starsToLight) ? Color.white : Color.black;
            Color startColor = stars[i].color;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                stars[i].color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            stars[i].color = targetColor;
            if (i == 0)
            {
                yield return new WaitForSeconds(waitBetweenStars);
            }
            else
            {
                yield return new WaitForSeconds(waitBetweenStars2);
            }

            
        }

    }

    public void LoadScene()
    {

        AudioManager.Instance.PlayClosePanel();
        TransitionCanvas.instance.DoTransition("MapScene");
    }






}
