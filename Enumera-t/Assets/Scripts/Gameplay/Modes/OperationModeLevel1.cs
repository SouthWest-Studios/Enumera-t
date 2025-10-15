using UnityEngine;

public class OperationModeLevel1 : IOperationMode
{
    private GameplayManager manager;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.sums = true; // Solo sumas
    }

    public void GenerateOperation()
    {
        manager.enemyNumber = Random.Range(5, 10);

        manager.operationNumber = OperationGenerator.PosibleSolution(
            manager.sums,
            manager.operationNumber,
            true,
            1,
            6,
            manager.enemyNumber,
            manager.numbersList,
            manager.alreadyUsedNumbers,
            manager.unlockedNumbersInList);

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, false, manager.operationNumberParentTransf);
        Debug.Log(manager.operationNumberTransf.position);
        manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf, true, manager.operationNumberParentTransf);
    }

    public bool CheckAnswer(int number, int operationIndex)
    {
        // Solo sumas simples
        return manager.operationNumber + number == manager.enemyNumber;
    }
}
