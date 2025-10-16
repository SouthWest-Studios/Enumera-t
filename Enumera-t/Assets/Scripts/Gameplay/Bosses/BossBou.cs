using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using static GameplayManager;

public class BossBou : IBossBehavior
{
    private GameplayManager manager;

    private int lastSolution = 0; // Guarda el n�mero correcto de la operaci�n anterior
    private int lastZPosition = -1; // 0 = X, 1 = Y, 2 = Z (posici�n anterior de la inc�gnita)
    private GameObject temporalNumber;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.FindChildRecursive(
            manager.FindChildRecursive(manager.level2.transform, "1rstOperation"),
            "BossExtra"
        ).gameObject.SetActive(true);
        manager.solutionSlot.SetActive(false);
    }

    public void GenerateOperation()
    {
        bool isSumOperation = Random.value > 0.5f;
        manager.sums = isSumOperation;

        // Actualizar s�mbolos
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/plus");
        manager.operationSymbolImage2.sprite = Resources.Load<Sprite>(
            isSumOperation ? "Sprites/plus" : "Sprites/minus"
        );

        const int minVal = 1;
        const int maxVal = 9;
        const int maxIntentos = 300;

        bool found = false;

        int x = 0, y = 0, z = 0;
        int enemy = 0;
        int zPosition = 0; // 0 = X, 1 = Y, 2 = Z

        // Elegimos una nueva posici�n para la inc�gnita, diferente a la anterior
        do
        {
            zPosition = Random.Range(0, 3);
        }
        while (zPosition == lastZPosition);

        for (int intento = 0; intento < maxIntentos && !found; intento++)
        {
            // Generar aleatoriamente tres n�meros base
            int a = Random.Range(minVal, maxVal + 1);
            int b = Random.Range(minVal, maxVal + 1);
            int c = Random.Range(minVal, maxVal + 1);

            // Asignar los valores seg�n si hay una soluci�n previa
            if (lastSolution != 0)
            {
                // El n�mero previo reemplaza una posici�n diferente a la actual inc�gnita
                int fixedPos;
                do { fixedPos = Random.Range(0, 3); }
                while (fixedPos == zPosition);

                if (fixedPos == 0) a = lastSolution;
                else if (fixedPos == 1) b = lastSolution;
                else c = lastSolution;
            }

            // Probar con todos los Z posibles de la lista
            for (int i = 0; i < manager.unlockedNumbersInList; i++)
            {
                int zCandidate = manager.numbersList[i];

                // Colocar la inc�gnita en la posici�n correspondiente
                if (zPosition == 0)
                {
                    x = zCandidate;
                    y = b;
                    z = c;
                }
                else if (zPosition == 1)
                {
                    x = a;
                    y = zCandidate;
                    z = c;
                }
                else
                {
                    x = a;
                    y = b;
                    z = zCandidate;
                }

                // Calcular el resultado
                enemy = isSumOperation ? (x + y + z) : (x + y - z);

                if (enemy >= 1 && enemy <= 9)
                {
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            Debug.LogError("No se pudo generar una operaci�n v�lida para BossBou.");
            return;
        }

        // Guardar la nueva operaci�n
        manager.operationNumber = x;
        manager.secondOperationNumber = y;
        manager.enemyNumber = enemy;

        lastZPosition = zPosition;
        // Mostrar visualmente
        Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
        string opSymbol = isSumOperation ? "+" : "-";

        if (zPosition == 0)
        {
            manager.solutionBossSlot.SetActive(true);
            manager.solutionBossSlot2.SetActive(false);
            manager.solutionBossSlot3.SetActive(false);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.solutionBossSlot4.transform, false, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.solutionBossSlot2.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.solutionBossSlot3.transform, true, parentTransf);

            Debug.Log($"[BossBou] Operaci�n generada: Z + {x} {opSymbol} {y} = {enemy} | Z posici�n: {zPosition} | Z correcto: {z}");
        }
        else if (zPosition == 1)
        {
            manager.solutionBossSlot.SetActive(false);
            manager.solutionBossSlot2.SetActive(true);
            manager.solutionBossSlot3.SetActive(false);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.solutionBossSlot4.transform, false, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.solutionBossSlot.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.solutionBossSlot3.transform, true, parentTransf);

            Debug.Log($"[BossBou] Operaci�n generada: {x} + z {opSymbol} {y} = {enemy} | Z posici�n: {zPosition} | Z correcto: {z}");
        }
        else
        {
            manager.solutionBossSlot.SetActive(false);
            manager.solutionBossSlot2.SetActive(false);
            manager.solutionBossSlot3.SetActive(true);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.solutionBossSlot4.transform, false, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.solutionBossSlot.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.solutionBossSlot2.transform, true, parentTransf);
            Debug.Log($"[BossBou] Operaci�n generada: {x} + {y} {opSymbol} Z = {enemy} | Z posici�n: {zPosition} | Z correcto: {z}");

        }

        

        

        

        
        
    }

    public bool CheckAnswer(int number, int operationIndex)
    {
        bool correct = false;

        if (lastZPosition == 0)
        {
            if (manager.sums)
            {
                if (number + manager.operationNumber + manager.secondOperationNumber == manager.enemyNumber)
                {
                    correct = true;
                    temporalNumber = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionBossSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
                    Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
                    temporalNumber.transform.SetParent(parentTransf, false);
                    temporalNumber.transform.position = manager.solutionBossSlot.transform.position;
                    manager.RestoreNumberToSlot(1);
                }
                    
            }
            else
            {
                if (number + manager.operationNumber - manager.secondOperationNumber == manager.enemyNumber)
                {
                    correct = true;
                    temporalNumber = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionBossSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
                    Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
                    temporalNumber.transform.SetParent(parentTransf, false);
                    temporalNumber.transform.position = manager.solutionBossSlot.transform.position;
                    manager.RestoreNumberToSlot(1);
                }
            }
        }
        else if (lastZPosition == 1)
        {
            if (manager.sums)
            {
                if (manager.operationNumber + number +  manager.secondOperationNumber == manager.enemyNumber)
                    correct = true;
            }
            else
            {
                if (manager.operationNumber + number - manager.secondOperationNumber == manager.enemyNumber)
                    correct = true;
            }
        }
        else
        {
            if (manager.sums)
            {
                if (manager.operationNumber + manager.secondOperationNumber + number == manager.enemyNumber)
                    correct = true;
            }
            else
            {
                if (manager.operationNumber + manager.secondOperationNumber - number == manager.enemyNumber)
                    correct = true;
            }
        }

        

        if (correct)
        {
            lastSolution = number; // Guardamos el n�mero correcto para la siguiente operaci�n
            Debug.Log($"[BossBou] Respuesta correcta. Nueva lastSolution = {lastSolution}");
        }

        return correct;
    }

    public void OnCorrectAnswer(int operationIndex) { }
    public void OnWrongAnswer() { Debug.Log("Respuesta incorrecta en BossBou."); }
    public void OnAnswer(int number, int operationIndex) { }
    public void Update() { }
}
