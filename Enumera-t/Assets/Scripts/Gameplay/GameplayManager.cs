using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameplayManager;
using Unity.VisualScripting;
using System.Linq;

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
        //level data
        if(LevelData.instance != null)
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

                backGroundPath = "Prefabs/BackGrounds/Cave-Background";
                path = "Prefabs/Enemies/Drac";

                GameObject prefab2 = Resources.Load<GameObject>(path);

                Vector3 temporalPosition2 = bossTransf.position;
                bossPrefab = Instantiate(prefab2, gameplayCanvas.transform, true);
                bossPrefab.transform.position = temporalPosition2;

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

                backGroundPath = "Prefabs/BackGrounds/BullIntestine-Background";
                path = "Prefabs/Enemies/Bou";

                Transform firstOperationLevel3 = FindChildRecursive(level3.transform, "1rstOperation");

                if (firstOperationLevel3 != null)
                {
                    // En nivel 3 hay 3 números, dos operationNumber y un enemy
                    operationNumberTransf = FindChildRecursive(firstOperationLevel3, "OperationNumberImage").transform;
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

        bossList[level-1].SetActive(true);

        //prefab = Resources.Load<GameObject>(path);

        //temporalPosition = bossTransf.position;
        //bossPrefab = Instantiate(prefab, gameplayCanvas.transform, true);
        //bossPrefab.transform.position = temporalPosition;

        health = maxHealth;
        healthBar.fillAmount = health / 10f;

        RoundCompleted(1);

        //if(sums)
        //{
        //    OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/plus");
        //}
        //else
        //{
        //    OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/minus");
        //}

        for (int i = 0; i < slots.Count; i++)
        {
            if(i < unlockedNumbersInList)
            {
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().number = numbersList[i];
            }
            else
            {
                slots[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }

        if(LevelData.dialogueInGameOne != null && LevelData.dialogueInGameOne.sentences.Length > 0)
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
            WrongNumberToSlot(operationIndex, true);
        }
    }

    private void CorrectAnswerNormal()
    {
        //print("BONA RESPOSTA!");

        roundsBeforeBoss++;
        for (int i = 0; i < temporalPrefab.Count; i++)
        {
            if (temporalPrefab[i] != null)
                UnityEngine.Object.Destroy(temporalPrefab[i]);
        }
        temporalPrefab.Clear();


        if (roundsBeforeBoss >= maxRoundsBeforeBoss && !isBoss)
        {
            ActivateBoss();
        }
            

        print("siguiente");

        RoundCompleted(1);

        
        

        

        
    }




    // Función auxiliar para devolver número al slot
    public void WrongNumberToSlot(int operationIndex, bool toLock)
    {
        GameObject targetSlot = (operationIndex == 2) ? solutionSlot2 : solutionSlot;

        if (targetSlot == null) return;

        for (int i = 0; i < unlockedNumbersInList; i++)
        {
            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {
                var ui = targetSlot.transform.GetChild(0).GetComponent<NumberUi>();
                if(toLock)
                {
                    ui.locked = true;
                    Transform prohibit = ui.GetComponentsInChildren<Transform>(true)
                      .FirstOrDefault(t => t.name == "prohibit");

                    if (prohibit != null)
                        prohibit.gameObject.SetActive(true);
                    else
                        Debug.LogWarning("No se encontró 'prohibit'");
                }
                    
                //ui.image.color = Color.red;
                targetSlot.transform.GetChild(0).SetParent(slots[i].transform);
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

                if(targetSlot.transform.childCount > 0)
                {
                    var ui2 = targetSlot.transform.GetChild(0).GetComponent<NumberUi>();
                    Transform prohibit2 = ui2.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "prohibit");

                    if (toLock)
                    {
                        ui2.locked = true;
                        if (prohibit2 != null)
                            prohibit2.gameObject.SetActive(true);
                        else
                            Debug.LogWarning("No se encontró 'prohibit'");
                    }
                }
                
                
                if(!toLock)
                {
                    ui.locked = false;
                    if (prohibit != null)
                        prohibit.gameObject.SetActive(false);
                    else
                        Debug.LogWarning("No se encontró 'prohibit'");
                }
                
                
            }

            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {

                targetSlot.transform.GetChild(0).SetParent(slots[i].transform);

            }
        }

        
    }



    //public void RoundCompleted(int operationIndex)
    //{
    //    if(operationIndex == 2)
    //    {
    //        RestoreNumberToSlot(operationIndex);
    //        return;
    //    }
    //    if( isBoss && health == 0)
    //    {

    //        SceneManager.LoadScene("MapScene");
    //    }
    //    // Límite de intentos global para evitar bucle infinito
    //    int intentosGlobales = 0;
    //    int maxIntentosGlobales = 50;
    //    bool numeroValido = false;

    //    if (unlockedNumbersInList == 0 || numbersList.Count == 0)
    //    {
    //        Debug.LogError("No hay números disponibles en numbersList o unlockedNumbersInList es 0.");
    //        return;
    //    }

    //    while (!numeroValido && intentosGlobales < maxIntentosGlobales)
    //    {
    //        intentosGlobales++;

    //        if (sums)
    //        {
    //            if (isBoss)
    //            {
    //                enemyNumber = bossNumber;
    //            }
    //            else
    //            {
    //                enemyNumber = Random.Range(5, 10);
    //            }

    //            if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 5)
    //            {

    //                operationNumber = OperationGenerator.PosibleSolution(
    //                sums,
    //                operationNumber,
    //                false,
    //                1,
    //                6,
    //                enemyNumber,
    //                numbersList,
    //                alreadyUsedNumbers,
    //                unlockedNumbersInList);
    //            }
    //            else
    //            {

    //                operationNumber = OperationGenerator.PosibleSolution(
    //                sums,
    //                operationNumber,
    //                true,
    //                1,
    //                6,
    //                enemyNumber,
    //                numbersList,
    //                alreadyUsedNumbers,
    //                unlockedNumbersInList);


    //            }
    //        }
    //        else
    //        {
    //            if (isBoss)
    //            {
    //                enemyNumber = bossNumber;
    //            }
    //            else
    //            {
    //                enemyNumber = Random.Range(1, 6);
    //            }

    //            if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 10 - enemyNumber)
    //            {

    //                operationNumber = OperationGenerator.PosibleSolution(
    //                sums,
    //                operationNumber,
    //                false,
    //                enemyNumber,
    //                10,
    //                enemyNumber,
    //                numbersList,
    //                alreadyUsedNumbers,
    //                unlockedNumbersInList);
    //            }
    //            else
    //            {
    //                operationNumber = OperationGenerator.PosibleSolution(
    //                 sums,
    //                 operationNumber,
    //                 true,
    //                 enemyNumber,
    //                 10,
    //                 enemyNumber,
    //                 numbersList,
    //                 alreadyUsedNumbers,
    //                 unlockedNumbersInList);
    //            }
    //        }

    //        if (operationNumber != 0)
    //        {
    //            numeroValido = true;
    //        }
    //        else
    //        {
    //            alreadyUsedNumbers.Clear(); // reset si no hay solución
    //        }
    //    }

    //    if (!numeroValido)
    //    {
    //        Debug.LogError("No se pudo encontrar un número válido para esta ronda.");
    //        return;
    //    }

    //    alreadyUsedNumbers.Add(operationNumber);

    //    // Asignar sprite de operación
    //    if(operationIndex == 1)
    //    {
    //        AssignNumberPrefab(enemyNumber, enemyTransf, false, operationNumberParentTransf);
    //        AssignNumberPrefab(operationNumber, operationNumberTransf, true, operationNumberParentTransf);
    //    }
    //    RestoreNumberToSlot(operationIndex);
    //}

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
                if (LevelData.instance != null)
                {
                    DataLevels.Instance.CompleteLevel(LevelData.instance.levelId);
                    LevelData.instance.levelComplete = true;
                }
                SceneManager.LoadScene("MapScene");
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
                Debug.LogError("No se pudo castear bossBehavior a BossBou");
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





}
