using Unity.VisualScripting;
using UnityEngine;

public class OperationModeLevel2 : IOperationMode
{
    private GameplayManager manager;

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
            6,
            manager.enemyNumber,
            manager.numbersList,
            manager.alreadyUsedNumbers,
            manager.unlockedNumbersInList);
        }
        else
        {
            manager.enemyNumber = Random.Range(1, 6);
            manager.operationNumber = OperationGenerator.PosibleSolution(
            manager.sums,
            manager.operationNumber,
            true,
            manager.enemyNumber,
            10,
            manager.enemyNumber,
            manager.numbersList,
            manager.alreadyUsedNumbers,
            manager.unlockedNumbersInList);
        }

        Transform parentTransf = manager.FindChildRecursive(manager.level2.transform, "1rstOperation").transform;

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, false, parentTransf);
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
