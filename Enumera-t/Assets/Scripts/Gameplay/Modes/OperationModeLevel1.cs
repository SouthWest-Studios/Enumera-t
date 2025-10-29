using System.Collections;
using UnityEngine;

public class OperationModeLevel1 : IOperationMode
{
    private GameplayManager manager;

    int lastNumber;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.sums = true;
    }

    public void GenerateOperation()
    {
        manager.enemyNumber = Random.Range(5, 10);

        manager.operationNumber = OperationGenerator.PosibleSolution(
            manager.sums,
            manager.operationNumber,
            true,
            1,
            10,
            manager.enemyNumber,
            manager.numbersList,
            manager.alreadyUsedNumbers,
            manager.unlockedNumbersInList,
            lastNumber
        );

        lastNumber = manager.operationNumber;

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, true, manager.operationNumberParentTransf);
        manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf, true, manager.operationNumberParentTransf);

        manager.PlayOperationEntryAnimation(manager.firstOperationCanvas);
    }

    private IEnumerator GenerateOperationDelayed()
    {
        yield return new WaitForSeconds(0.2f); // espera 1 segundo antes de generar

        
    }



    public bool CheckAnswer(int number, int operationIndex)
    {
        // Solo sumas simples
        return manager.operationNumber + number == manager.enemyNumber;
    }
}
