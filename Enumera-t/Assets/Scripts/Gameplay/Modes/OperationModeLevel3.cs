using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class OperationModeLevel3 : IOperationMode
{
    private GameplayManager manager;

    private int MYx;
    private int MYy;
    private int MYz; // El n�mero que rellenar� el jugador
    int lastEnemy = 0;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;

    }

    public void GenerateOperation()
    {
        // Elegimos si la operaci�n ser� de tipo suma o resta
        bool isSumOperation = Random.value > 0.5f;
        manager.sums = isSumOperation;

        // Actualizamos el sprite del s�mbolo
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/Ui/Gameplay/Suma");
        manager.operationSymbolImage2.sprite = Resources.Load<Sprite>(
            isSumOperation ? "Sprites/Ui/Gameplay/Suma" : "Sprites/Ui/Gameplay/Resta"
        );

        const int minXY = 1;
        const int maxXY = 9;
        const int maxIntentos = 300;

        bool found = false;
        int foundX = 0;
        int foundY = 0;
        int foundZ = 0;
        int foundEnemy = 0;

        for (int intento = 0; intento < maxIntentos && !found; intento++)
        {
            // Generar x e y aleatoriamente dentro del rango permitido
            int x = Random.Range(minXY, maxXY + 1);
            int y = Random.Range(minXY, maxXY + 1);


            // Probar con cada z disponible en la lista del jugador
            int startIndex = Random.Range(0, manager.unlockedNumbersInList);

            // Probar con cada candidato de la lista (este candidato ser� la inc�gnita)
            for (int offset = 0; offset < manager.unlockedNumbersInList; offset++)
            {
                int i = (startIndex + offset) % manager.unlockedNumbersInList;
                int z = manager.numbersList[i];


                int enemy = isSumOperation ? (x + y + z) : (x + y - z);


                if (enemy >= 1 && enemy <= 9 && enemy != lastEnemy)
                {
                    found = true;
                    foundX = x;
                    foundY = y;
                    foundZ = z;
                    foundEnemy = enemy;
                    lastEnemy = enemy;
                    break;
                }
            }
        }

        if (!found)
        {
            //Debug.LogError("No se pudo generar una operaci�n v�lida con los n�meros disponibles.");
            return;
        }

        // Asignar los valores al manager
        manager.operationNumber = foundX;          // X
        manager.secondOperationNumber = foundY;    // Y
        manager.enemyNumber = foundEnemy;          // C (resultado)

        // Asignar visualmente los prefabs en la interfaz
        Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, true, parentTransf);
        manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf, true, parentTransf);
        manager.AssignNumberPrefab(manager.secondOperationNumber, manager.secondOperationNumberTransf, true, parentTransf);

        manager.PlayOperationEntryAnimation(parentTransf.gameObject);

        // Activar el slot donde el jugador pondr� el n�mero Z
        manager.solutionSlot.SetActive(true);

        // DEBUG
        //string opSymbol = isSumOperation ? "+" : "-";
        //Debug.Log($"Operaci�n generada: {foundX} + {foundY} {opSymbol} Z = {foundEnemy}  |  Z v�lido: {foundZ}"); 
    }



    public bool CheckAnswer(int number, int operationIndex)
    {
        if(manager.sums)
        {
            return manager.operationNumber + manager.secondOperationNumber + number == manager.enemyNumber;
        }
        else
        {
            return manager.operationNumber + manager.secondOperationNumber - number == manager.enemyNumber;
        }
        
    }
}
