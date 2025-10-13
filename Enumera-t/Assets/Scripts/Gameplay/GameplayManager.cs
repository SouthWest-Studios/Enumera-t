using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameplayManager;

public enum BossType { None, Drac, Timer, Normal }

public class GameplayManager : MonoBehaviour
{

    public int enemyNumber;

    public int bossNumber;

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
    public int health = 10;
    public Image healthBar;
    public BossType bossType = BossType.None;
    private IBossBehavior bossBehavior;
    [HideInInspector]  public bool victory1;
    [HideInInspector]  public bool victory2;

    [HideInInspector] public List<GameObject> temporalPrefab = new List<GameObject>();

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

    public interface IBossBehavior
    {
        void Init(GameplayManager manager);
        void OnCorrectAnswer(int index);
        void OnWrongAnswer();  
        void Update();    
    }

    void Start()
    {
        healthBar.fillAmount = health / 10f;

        


        RoundCompleted(1);
        //AssignNumberPrefab(enemyNumber,enemyTransf, false, operationNumberParentTransf);
        //AssignNumberPrefab(operationNumber, operationNumberParentTransf, true, operationNumberParentTransf);

        if(sums)
        {
            OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/plus");
        }
        else
        {
            OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/minus");
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if(i < unlockedNumbersInList)
            {
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().number = numbersList[i];
                //AssignNumberPrefab(numbersList[i], slots[i].transform.GetChild(0).GetComponent<Image>());
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

    public void AssignNumberPrefab(int number,Transform transf, bool good, Transform parentTransform)
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

        temporalPrefab.Add(numberPrefab);
    }

    public void AnswerGuess(int number, int operationIndex)
    {
        bool correctOp1 = false;
        bool correctOp2 = false;

        // Verificamos según el slot que llamó
        if (operationIndex == 1)
        {
            correctOp1 = (sums)
                ? (operationNumber + number == enemyNumber)
                : (operationNumber - number == enemyNumber);
        }
        else if (operationIndex == 2 && isBoss && bossType == BossType.Drac)
        {
            correctOp2 = (sums)
                ? (operationNumber2 + number == enemyNumber2)
                : (operationNumber2 - number == enemyNumber2);
        }

        // ──────────── MODO BOSS ─────────────
        if (isBoss && bossType == BossType.Drac)
        {
            var boss = bossBehavior as BossDrac;

            if ((boss.firstSolved && operationIndex == 1) ||
                (boss.secondSolved && operationIndex == 2))
            {
                Debug.Log("Esta operación ya fue resuelta.");
                return;
            }
          

            if (correctOp1 || correctOp2)
            {
                bossBehavior?.OnCorrectAnswer(operationIndex);
            }
            else
            {
                bossBehavior?.OnWrongAnswer();
                WrongNumberToSlot(operationIndex);
            }
            RestoreNumberToSlot(operationIndex);

            return;
        }

        // ──────────── MODO NORMAL ─────────────
        if (correctOp1)
        {
            print("BONA RESPOSTA!");

            roundsBeforeBoss++;
            if(temporalPrefab.Count > 0)
            {
                foreach (GameObject go in temporalPrefab)
                {
                    if (go != null)
                        Destroy(go);
                }
                temporalPrefab.Clear();
            }
                
            RoundCompleted(operationIndex);

            if (roundsBeforeBoss >= maxRoundsBeforeBoss && !isBoss)
            {
                ActivateBoss();
            }
        }
        else
        {
            print("INCORRECTE");
            WrongNumberToSlot(1);
        }
    }



    // Función auxiliar para devolver número al slot
    private void WrongNumberToSlot(int operationIndex)
    {
        GameObject targetSlot = (operationIndex == 2) ? solutionSlot2 : solutionSlot;

        if (targetSlot == null) return;

        for (int i = 0; i < unlockedNumbersInList; i++)
        {
            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {
                var ui = targetSlot.transform.GetChild(0).GetComponent<NumberUi>();
                ui.locked = false;
                //ui.image.color = Color.red;
                targetSlot.transform.GetChild(0).SetParent(slots[i].transform);
                break;
            }
        }
    }

    private void RestoreNumberToSlot(int operationIndex)
    {
        GameObject targetSlot = (operationIndex == 2) ? solutionSlot2 : solutionSlot;

        if (targetSlot == null) return;

        for (int i = 0; i < unlockedNumbersInList; i++)
        {
            if (slots[i] != null && slots[i].transform.childCount > 0)
            {
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().locked = true;
                //slots[i].transform.GetChild(0).GetComponent<NumberUi>().image.color = Color.white;
            }

            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {


                targetSlot.transform.GetChild(0).SetParent(slots[i].transform);

            }
        }

        
    }



    public void RoundCompleted(int operationIndex)
    {
        if(operationIndex == 2)
        {
            RestoreNumberToSlot(operationIndex);
            return;
        }
        if( isBoss && health == 0)
        {
           
            SceneManager.LoadScene("MapScene");
        }
        // Límite de intentos global para evitar bucle infinito
        int intentosGlobales = 0;
        int maxIntentosGlobales = 50;
        bool numeroValido = false;

        if (unlockedNumbersInList == 0 || numbersList.Count == 0)
        {
            Debug.LogError("No hay números disponibles en numbersList o unlockedNumbersInList es 0.");
            return;
        }

        while (!numeroValido && intentosGlobales < maxIntentosGlobales)
        {
            intentosGlobales++;

            if (sums)
            {
                if (isBoss)
                {
                    enemyNumber = bossNumber;
                }
                else
                {
                    enemyNumber = Random.Range(5, 10);
                }
                
                if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 5)
                {
                    operationNumber = PosibleSolution(operationNumber, false, 1, 6, enemyNumber);
                }
                else
                {
                    operationNumber = PosibleSolution(operationNumber, true, 1, 6, enemyNumber);
                }
            }
            else
            {
                if (isBoss)
                {
                    enemyNumber = bossNumber;
                }
                else
                {
                    enemyNumber = Random.Range(1, 6);
                }
                
                if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 10 - enemyNumber)
                {
                    operationNumber = PosibleSolution(operationNumber, false, enemyNumber, 10, enemyNumber);
                }
                else
                {
                    operationNumber = PosibleSolution(operationNumber, true, enemyNumber, 10, enemyNumber);
                }
            }

            if (operationNumber != 0)
            {
                numeroValido = true;
            }
            else
            {
                alreadyUsedNumbers.Clear(); // reset si no hay solución
            }
        }

        if (!numeroValido)
        {
            Debug.LogError("No se pudo encontrar un número válido para esta ronda.");
            return;
        }

        alreadyUsedNumbers.Add(operationNumber);

        // Asignar sprite de operación
        if(operationIndex == 1)
        {
            AssignNumberPrefab(enemyNumber, enemyTransf, false, operationNumberParentTransf);
            AssignNumberPrefab(operationNumber, operationNumberTransf, true, operationNumberParentTransf);
        }
        RestoreNumberToSlot(operationIndex);

        // Mover primer hijo de solutionSlot a un slot vacío y desbloquear numeros

        //if (solutionSlot != null)
        //{
        //    for (int i = 0; i < unlockedNumbersInList; i++)
        //    {
        //        if(slots[i] != null && slots[i].transform.childCount > 0)
        //        {
        //            slots[i].transform.GetChild(0).GetComponent<NumberUi>().locked = true;
        //            slots[i].transform.GetChild(0).GetComponent<NumberUi>().image.color = Color.white;
        //        }

        //        if (slots[i] != null && slots[i].transform.childCount == 0 && solutionSlot.transform.childCount > 0)
        //        {


        //            solutionSlot.transform.GetChild(0).SetParent(slots[i].transform);

        //        }
        //    }

        //}
    }


    public int PosibleSolution(int numberTocheck, bool alreadyUsed, int initialRange, int finalRange, int enemyNumberUsed)
    {
        if (unlockedNumbersInList == 0 || numbersList.Count == 0)
            return 0;
        // Comprobar si existe alguna solución posible
        bool existeSolucion = false;
        for (int candidate = initialRange; candidate < finalRange; candidate++)
        {
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (candidate + listNumber == enemyNumberUsed)
                    {
                        existeSolucion = true;
                        break;
                    }
                }
                else
                {

                    if (candidate - listNumber == enemyNumberUsed)
                    {
                        existeSolucion = true;
                        break;
                    }
                }

            }
            if (existeSolucion) break;
        }

        if (!existeSolucion) return 0;

        // Buscar número válido
        int intentos = 0;
        int maxIntentos = 100;

        do
        {
            intentos++;

            if (alreadyUsed)
            {
                numberTocheck = Random.Range(initialRange, finalRange);
            }
            else
            {
                do
                {
                    numberTocheck = Random.Range(initialRange, finalRange);
                } while (alreadyUsedNumbers.Contains(numberTocheck));
            }

            // Comprobar si este número genera solución
            bool posibleSolution = false;
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (numberTocheck + listNumber == enemyNumberUsed)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
                else
                {
                    if (numberTocheck - listNumber == enemyNumberUsed)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
            }

            if (posibleSolution)
                return numberTocheck;

        } while (intentos < maxIntentos);

        return 0; // fallback seguro
    }

    private void ActivateBoss()
    {
        isBoss = true;
        secondOperationCanvas.SetActive(true);
        roundsBeforeBoss = 0;

        switch (bossType)
        {
            case BossType.Drac:
                bossBehavior = new BossDrac();
                break;
            case BossType.Timer:
                // bossBehavior = new BossTimer();
                break;
            case BossType.Normal:
                // bossBehavior = new BossNormal();
                break;
        }

        bossBehavior?.Init(this);
    }




}
