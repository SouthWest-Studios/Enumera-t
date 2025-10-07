using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameplayManager;

public enum BossType { None, DoubleOperation, Timer, Normal }

public class GameplayManager : MonoBehaviour
{

    public int enemyNumber;

    public Image enemyImage;

    public GameObject solutionSlot;

    public Image operationNumberImage;

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

    [Header("Boss doble operación")]
    public GameObject secondOperationCanvas;
    public Image enemyImage2;
    public List<GameObject> slots2;
    public GameObject solutionSlot2;
    public Image operationNumberImage2;
    public Image operationSymbolImage2;

    public int enemyNumber2;
    [HideInInspector] public int operationNumber2;

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
        AssignNumberImage(enemyNumber, enemyImage);
        AssignNumberImage(operationNumber, operationNumberImage);

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
                AssignNumberImage(numbersList[i], slots[i].transform.GetChild(0).GetComponent<Image>());
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

    public void AssignNumberImage(int number, Image image)
    {
        // Construimos el path en base al número
        string path = "Sprites/numbers/number" + number;

        // Cargamos el sprite desde Resources
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("No se encontró sprite en: " + path);
        }
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
        else if (operationIndex == 2 && isBoss && bossType == BossType.DoubleOperation)
        {
            correctOp2 = (sums)
                ? (operationNumber2 + number == enemyNumber2)
                : (operationNumber2 - number == enemyNumber2);
        }

        // ──────────── MODO BOSS ─────────────
        if (isBoss && bossType == BossType.DoubleOperation)
        {
            var boss = bossBehavior as BossDoubleOperation;

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

            return;
        }

        // ──────────── MODO NORMAL ─────────────
        if (correctOp1)
        {
            print("BONA RESPOSTA!");

            roundsBeforeBoss++;
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
                ui.image.color = Color.red;
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
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().image.color = Color.white;
            }

            if (slots[i] != null && slots[i].transform.childCount == 0 && targetSlot.transform.childCount > 0)
            {


                targetSlot.transform.GetChild(0).SetParent(slots[i].transform);

            }
        }

        
    }



    public void RoundCompleted(int operationIndex)
    {
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
        AssignNumberImage(operationNumber, operationNumberImage);

        // Mover primer hijo de solutionSlot a un slot vacío y desbloquear numeros
        RestoreNumberToSlot(operationIndex);
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
            case BossType.DoubleOperation:
                bossBehavior = new BossDoubleOperation();
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
