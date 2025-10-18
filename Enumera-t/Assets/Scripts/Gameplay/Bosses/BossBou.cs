using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using static GameplayManager;

public class BossBou : IBossBehavior
{
    private GameplayManager manager;

    private int lastSolution = 0; // Guarda el número correcto de la operación anterior
    private int lastZPosition = -1; // 0 = X, 1 = Y, 2 = Z (posición anterior de la incógnita)
    private GameObject temporalNumber;

    public int requiredZ;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.FindChildRecursive(
            manager.FindChildRecursive(manager.level2.transform, "1rstOperation"),
            "BossExtra"
        ).gameObject.SetActive(true);
        manager.RestoreNumberToSlot(manager.solutionSlot);
        manager.solutionSlot.SetActive(false);
    }

    public void GenerateOperation()
    {
        bool isSumOperation = Random.value > 0.5f;
        manager.sums = isSumOperation;

        // Actualizar símbolos
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/plus");
        manager.operationSymbolImage2.sprite = Resources.Load<Sprite>(
            isSumOperation ? "Sprites/plus" : "Sprites/minus"
        );

        const int minVal = 1;
        const int maxVal = 9;
        const int maxIntentos = 300;

        bool found = false;

        int a = 0, b = 0, c = 0;      // valores base
        int x = 0, y = 0, z = 0;      // valores finales (x,y,z-> operands)
        int enemy = 0;
        int zPosition = 0; // 0 = primera posición (Z + A + B), 1 = segunda, 2 = tercera

        // Elegimos nueva posición de Z distinta de la anterior
        do
        {
            zPosition = Random.Range(0, 3);
        } while (zPosition == lastZPosition);

        for (int intento = 0; intento < maxIntentos && !found; intento++)
        {
            // Generar tres números base
            a = Random.Range(minVal, maxVal + 1);
            b = Random.Range(minVal, maxVal + 1);
            c = Random.Range(minVal, maxVal + 1);

            // Si existe lastSolution, colocarlo en alguna posición distinta de la incógnita
            if (lastSolution != -1 && lastSolution != 0) // ajusta el sentinel según tu inicialización
            {
                int fixedPos;
                do { fixedPos = Random.Range(0, 3); } while (fixedPos == zPosition);

                if (fixedPos == 0) a = lastSolution;
                else if (fixedPos == 1) b = lastSolution;
                else c = lastSolution;
            }
            int startIndex = Random.Range(0, manager.unlockedNumbersInList);

            // Probar con cada candidato de la lista (este candidato será la incógnita)
            for (int offset = 0; offset < manager.unlockedNumbersInList; offset++)
            {
                int i = (startIndex + offset) % manager.unlockedNumbersInList;
                int zCandidate = manager.numbersList[i];

                // Construir (x,y,z) según la posición de la incógnita
                if (zPosition == 0)
                {
                    x = zCandidate; // incógnita primera
                    y = b;
                    z = c;
                }
                else if (zPosition == 1)
                {
                    x = a;
                    y = zCandidate; // incógnita segunda
                    z = c;
                }
                else // zPosition == 2
                {
                    x = a;
                    y = b;
                    z = zCandidate; // incógnita tercera
                }

                // Calcular enemy
                enemy = isSumOperation ? (x + y + z) : (x + y - z);

                // Comprueba rango (ajusta límites según juego)
                if (enemy >= 1 && enemy <= 9)
                {
                    // guardamos la solución encontrada
                    found = true;

                    // Guardar el z correcto para validación / debug
                    requiredZ = zCandidate;

                    break;
                }
            }
        }

        if (!found)
        {
            Debug.LogError("No se pudo generar una operación válida para BossBou.");
            return;
        }

        // Ahora asignar los números VISIBLES (los dos que NO son la incógnita)
        // y también guardar la posición de la incógnita para la comprobación posterior.
        lastZPosition = zPosition;
        manager.enemyNumber = enemy;

        // Dependiendo de la posición de Z, asignamos operationNumber y secondOperationNumber
        // a los números visibles en el orden que usa la UI.
        if (zPosition == 0)
        {
            // Visual: [ Z ] [ operationNumber ] [ secondOperationNumber ] = enemy
            manager.operationNumber = y;       // b
            manager.secondOperationNumber = z; // c
        }
        else if (zPosition == 1)
        {
            // Visual: [ operationNumber ] [ Z ] [ secondOperationNumber ] = enemy
            manager.operationNumber = x;       // a
            manager.secondOperationNumber = z; // c
        }
        else // zPosition == 2
        {
            // Visual: [ operationNumber ] [ secondOperationNumber ] [ Z ] = enemy
            manager.operationNumber = x;       // a
            manager.secondOperationNumber = y; // b
        }

        // Mostrar visualmente en la UI (tu código original adaptado por posiciones)
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

            Debug.Log($"[BossBou] Operación generada: Z + {manager.operationNumber} {opSymbol} {manager.secondOperationNumber} = {manager.enemyNumber} | Z posición: {zPosition} | Z correcto: {requiredZ}");
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

            Debug.Log($"[BossBou] Operación generada: {manager.operationNumber} + Z {opSymbol} {manager.secondOperationNumber} = {manager.enemyNumber} | Z posición: {zPosition} | Z correcto: {requiredZ}");
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

            Debug.Log($"[BossBou] Operación generada: {manager.operationNumber} + {manager.secondOperationNumber} {opSymbol} Z = {manager.enemyNumber} | Z posición: {zPosition} | Z correcto: {requiredZ}");
        }

    }


    public bool CheckAnswer(int number, int operationIndex)
    {
        GameObject solutionSlot;
        // Construimos a,b,c según lastZPosition:
        int a, b, c;
        if (lastZPosition == 0)
        {
            a = number;                      // Z en primera posición
            b = manager.operationNumber;     // primer número visible
            c = manager.secondOperationNumber;
            solutionSlot = manager.solutionBossSlot;
        }
        else if (lastZPosition == 1)
        {
            a = manager.operationNumber;
            b = number;                      // Z en segunda posición
            c = manager.secondOperationNumber;
            solutionSlot = manager.solutionBossSlot2;
        }
        else // lastZPosition == 2
        {
            a = manager.operationNumber;
            b = manager.secondOperationNumber;
            c = number;                      // Z en tercera posición
            solutionSlot = manager.solutionBossSlot3;
        }

        bool correct;
        if (manager.sums)
            correct = (a + b + c == manager.enemyNumber);
        else
            correct = (a + b - c == manager.enemyNumber);

        if (correct)
        {
            Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
            manager.RestoreNumberToSlot(solutionSlot);



            lastSolution = number; // guardar la solución para la siguiente ronda
            Debug.Log($"[BossBou] Respuesta correcta. lastSolution = {lastSolution}");
            // opcional: desactivar slot, instanciar temporalNumber, etc.
        }
        else
        {
            Debug.Log($"[BossBou] Respuesta INCORRECTA. Intentaste: {number}. Esperado: {requiredZ} (pero depende de la posición)");
        }

        return correct;
    }

    //public bool CheckAnswer(int number, int operationIndex)
    //{
    //    bool correct = false;

    //    if (lastZPosition == 0)
    //    {
    //        if (manager.sums)
    //        {
    //            if (number + manager.operationNumber + manager.secondOperationNumber == manager.enemyNumber)
    //            {
    //                correct = true;
    //                temporalNumber = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionBossSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
    //                Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
    //                temporalNumber.transform.SetParent(parentTransf, false);
    //                temporalNumber.transform.position = manager.solutionBossSlot.transform.position;
    //                manager.RestoreNumberToSlot(1);
    //            }

    //        }
    //        else
    //        {
    //            if (number + manager.operationNumber - manager.secondOperationNumber == manager.enemyNumber)
    //            {
    //                correct = true;
    //                temporalNumber = UnityEngine.Object.Instantiate(manager.numbersListPrefab[manager.solutionBossSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
    //                Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
    //                temporalNumber.transform.SetParent(parentTransf, false);
    //                temporalNumber.transform.position = manager.solutionBossSlot.transform.position;
    //                manager.RestoreNumberToSlot(1);
    //            }
    //        }
    //    }
    //    else if (lastZPosition == 1)
    //    {
    //        if (manager.sums)
    //        {
    //            if (manager.operationNumber + number + manager.secondOperationNumber == manager.enemyNumber)
    //                correct = true;
    //        }
    //        else
    //        {
    //            if (manager.operationNumber + number - manager.secondOperationNumber == manager.enemyNumber)
    //                correct = true;
    //        }
    //    }
    //    else
    //    {
    //        if (manager.sums)
    //        {
    //            if (manager.operationNumber + manager.secondOperationNumber + number == manager.enemyNumber)
    //                correct = true;
    //        }
    //        else
    //        {
    //            if (manager.operationNumber + manager.secondOperationNumber - number == manager.enemyNumber)
    //                correct = true;
    //        }
    //    }



    //    if (correct)
    //    {
    //        lastSolution = number; // Guardamos el número correcto para la siguiente operación
    //        Debug.Log($"[BossBou] Respuesta correcta. Nueva lastSolution = {lastSolution}");
    //    }

    //    return correct;
    //}


    public void OnCorrectAnswer(int operationIndex) { }
    public void OnWrongAnswer() { Debug.Log("Respuesta incorrecta en BossBou."); }
    public void OnAnswer(int number, int operationIndex) { }
    public void Update() { }
}
