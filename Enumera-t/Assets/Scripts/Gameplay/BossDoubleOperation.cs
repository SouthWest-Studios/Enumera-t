using UnityEngine;
using static GameplayManager;

public class BossDoubleOperation : IBossBehavior
{
    private GameplayManager manager;
    public bool firstSolved = false;
    public bool secondSolved = false;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.secondOperationCanvas.SetActive(true);

        manager.AssignNumberImage(manager.enemyNumber2, manager.enemyImage2);
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
            Debug.Log("Primera operación correcta!");
            manager.firstOperationCanvas.SetActive(false);
        }
        else if (operationIndex == 2 && !secondSolved)
        {
            secondSolved = true;
            manager.victory2 = true;
            Debug.Log("Segunda operación correcta!");
            manager.secondOperationCanvas.SetActive(false);
        }

        // Cuando ambas estén resueltas:
        if (firstSolved && secondSolved)
        {
            manager.health -= 2;
            manager.healthBar.fillAmount = manager.health / 10f;

            firstSolved = false;
            secondSolved = false;
            manager.victory1 = false;
            manager.victory2 = false;

            // Regenerar ambas operaciones
            manager.firstOperationCanvas.SetActive(true);
            manager.secondOperationCanvas.SetActive(true);
            manager.RoundCompleted(1);
            manager.RoundCompleted(2);
            GenerateSecondOperation();

            Debug.Log("¡Ambas operaciones resueltas! Daño al boss.");
        }
        else
        {
            Debug.Log("Una operación lista, falta la otra.");
        }
    }


    public void OnWrongAnswer()
    {
        Debug.Log("Respuesta incorrecta en Boss Doble Operación.");
    }

    public void Update() { }

    private void GenerateSecondOperation()
    {
        int intentosGlobales = 0;
        int maxIntentosGlobales = 50;
        bool numeroValido = false;

        if (manager.unlockedNumbersInList == 0 || manager.numbersList.Count == 0)
        {
            Debug.LogError("No hay números disponibles.");
            return;
        }

        while (!numeroValido && intentosGlobales < maxIntentosGlobales)
        {
            intentosGlobales++;
            if (manager.sums)
                manager.operationNumber2 = manager.PosibleSolution(manager.operationNumber2, true, 1, 6, manager.enemyNumber2);
            else
                manager.operationNumber2 = manager.PosibleSolution(manager.operationNumber2, true, manager.enemyNumber2, 10, manager.enemyNumber2);

            if (manager.operationNumber2 != 0)
                numeroValido = true;
        }

        if (!numeroValido)
        {
            Debug.LogError("No se pudo generar segunda operación.");
            return;
        }

        manager.AssignNumberImage(manager.operationNumber2, manager.operationNumberImage2);
    }
}
