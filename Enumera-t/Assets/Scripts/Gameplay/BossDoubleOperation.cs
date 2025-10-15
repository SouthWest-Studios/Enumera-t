using Unity.VisualScripting;
using UnityEngine;
using static GameplayManager;

public class BossBessones : IBossBehavior
{
    private GameplayManager manager;
    public bool firstSolved = false;
    public bool secondSolved = false;
    private GameObject temporalNumber1;
    private GameObject temporalNumber2;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.secondOperationCanvas.SetActive(true);

        manager.AssignNumberPrefab(manager.enemyNumber2, manager.enemyTransf2, true, manager.secondOperationCanvas.transform);
        manager.operationSymbolImage2.sprite = manager.sums
            ? Resources.Load<Sprite>("Sprites/plus")
            : Resources.Load<Sprite>("Sprites/minus");

        GenerateSecondOperation();
    }

    public void OnCorrectAnswer(int operationIndex)
    {
        if (operationIndex == 1 && !firstSolved)
        {
            firstSolved = true;
            manager.victory1 = true;
            Debug.Log("Primera operaci�n correcta!");
            manager.solutionSlot.SetActive(false);
            manager.enemyNumber = manager.bossNumber;
            temporalNumber1 = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
            temporalNumber1.transform.SetParent(manager.firstOperationCanvas.transform, false);
            temporalNumber1.transform.position = manager.solutionSlot.transform.position;
        }
        else if (operationIndex == 2 && !secondSolved)
        {
            secondSolved = true;
            manager.victory2 = true;
            Debug.Log("Segunda operaci�n correcta!");
            manager.solutionSlot2.SetActive(false);
            temporalNumber2 = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionSlot2.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
            temporalNumber2.transform.SetParent(manager.secondOperationCanvas.transform, false);
            temporalNumber2.transform.position = manager.solutionSlot2.transform.position;
        }

        // Cuando ambas est�n resueltas:
        if (firstSolved && secondSolved)
        {
            manager.health -= 2;
            manager.healthBar.fillAmount = manager.health / 10f;

            firstSolved = false;
            secondSolved = false;
            manager.victory1 = false;
            manager.victory2 = false;

            // Regenerar ambas operaciones
            manager.solutionSlot.SetActive(true);
            manager.solutionSlot2.SetActive(true);
            UnityEngine.Object.Destroy(temporalNumber1);
            UnityEngine.Object.Destroy(temporalNumber2);
            if (manager.temporalPrefab.Count > 0)
            {
                foreach (GameObject go in manager.temporalPrefab)
                {
                    if (go != null)
                        UnityEngine.Object.Destroy(go);
                }
                manager.temporalPrefab.Clear();
            }
            manager.RoundCompleted(1);
            manager.RoundCompleted(2);
            GenerateSecondOperation();

            Debug.Log("�Ambas operaciones resueltas! Da�o al boss.");
        }
        else
        {
            Debug.Log("Una operaci�n lista, falta la otra.");
        }
    }


    public void OnWrongAnswer()
    {
        Debug.Log("Respuesta incorrecta en Boss Doble Operaci�n.");
    }

    public void Update() { }

    private void GenerateSecondOperation()
    {
        int intentosGlobales = 0;
        int maxIntentosGlobales = 50;
        bool numeroValido = false;

        if (manager.unlockedNumbersInList == 0 || manager.numbersList.Count == 0)
        {
            Debug.LogError("No hay n�meros disponibles.");
            return;
        }

        while (!numeroValido && intentosGlobales < maxIntentosGlobales)
        {
            intentosGlobales++;
            if (manager.sums)
            {
                manager.operationNumber2 = OperationGenerator.PosibleSolution(
                        manager.sums,
                        manager.operationNumber2,
                        true,
                        1,
                        6,
                        manager.enemyNumber2,
                        manager.numbersList,
                        manager.alreadyUsedNumbers,
                        manager.unlockedNumbersInList);
            }


            
            else
            {
                manager.operationNumber2 = OperationGenerator.PosibleSolution(
                        manager.sums,
                        manager.operationNumber2,
                        true,
                        manager.enemyNumber2,
                        10,
                        manager.enemyNumber2,
                        manager.numbersList,
                        manager.alreadyUsedNumbers,
                        manager.unlockedNumbersInList);
            }


            

            if (manager.operationNumber2 != 0)
                numeroValido = true;
        }

        if (!numeroValido)
        {
            Debug.LogError("No se pudo generar segunda operaci�n.");
            return;
        }

        manager.AssignNumberPrefab(manager.operationNumber2, manager.operationNumberTransf2, true, manager.secondOperationCanvas.transform);
        manager.AssignNumberPrefab(manager.enemyNumber2, manager.enemyTransf2, true, manager.secondOperationCanvas.transform);
    }

    public void OnAnswer(int number, int operationIndex)
    {
        bool correctOp1 = false;
        bool correctOp2 = false;

        if (operationIndex == 1)
            correctOp1 = (manager.sums) ? (manager.operationNumber + number == manager.enemyNumber)
                                        : (manager.operationNumber - number == manager.enemyNumber);
        else if (operationIndex == 2)
            correctOp2 = (manager.sums) ? (manager.operationNumber2 + number == manager.enemyNumber2)
                                        : (manager.operationNumber2 - number == manager.enemyNumber2);

        // Comprobamos si ya se resolvi� la operaci�n
        if ((correctOp1 && firstSolved && operationIndex == 1) ||
            (correctOp2 && secondSolved && operationIndex == 2))
        {
            Debug.Log("Esta operaci�n ya fue resuelta.");
            return;
        }

        if (correctOp1 || correctOp2)
            OnCorrectAnswer(operationIndex);
        else
        {
            OnWrongAnswer();
            manager.WrongNumberToSlot(operationIndex);
        }

        manager.RestoreNumberToSlot(operationIndex);
    }

}
