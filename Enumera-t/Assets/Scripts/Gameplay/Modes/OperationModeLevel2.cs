using Unity.VisualScripting;
using UnityEngine;

public class OperationModeLevel2 : IOperationMode
{
    private GameplayManager manager;

    int lastNumber = 0;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
    }

    public void GenerateOperation()
    {


        manager.sums = Random.value > 0.5f;
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>(
            manager.sums ? "Sprites/Ui/Gameplay/Suma" : "Sprites/Ui/Gameplay/Resta"
        );

        if (manager.sums)
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
            lastNumber);
        }
        else
        {
            manager.enemyNumber = Random.Range(1, 6);
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
            lastNumber);
        }

        lastNumber = manager.enemyNumber;

        Transform parentTransf = manager.FindChildRecursive(manager.level2.transform, "1rstOperation").transform;

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, true, parentTransf);
        manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf, true, parentTransf);

        manager.PlayOperationEntryAnimation(parentTransf.gameObject);
    }

    public bool CheckAnswer(int number, int operationIndex)
    {
        if(manager.sums)
        {
            return manager.operationNumber + number == manager.enemyNumber;
        }
        else
        {
            return manager.operationNumber - number == manager.enemyNumber;
        }
        
    }
}
