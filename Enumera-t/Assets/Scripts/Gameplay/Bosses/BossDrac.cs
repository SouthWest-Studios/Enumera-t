using Unity.VisualScripting;
using UnityEngine;
using static GameplayManager;

public class BossDrac : IBossBehavior
{
    public int damageTaken = 2;
    private GameplayManager manager;

    private int MYx;
    private int MYy;
    private int MYz;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.FindChildRecursive(manager.FindChildRecursive(manager.level2.transform, "1rstOperation"), "BossExtra").gameObject.SetActive(true);
        manager.FindChildRecursive(manager.FindChildRecursive(manager.level2.transform, "1rstOperation"), "BackGroundOperation").gameObject.SetActive(false);
        manager.FindChildRecursive(manager.FindChildRecursive(manager.level2.transform, "1rstOperation"), "BackGroundOperation2").gameObject.SetActive(true);
        RectTransform rt = manager.FindChildRecursive(manager.level2.transform, "1rstOperation").GetComponent<RectTransform>();
        rt.anchoredPosition += new Vector2(60, 0);
        manager.damage = damageTaken;
        manager.number4.SetActive(true);
    }

    public void GenerateOperation()
    {
        
        // Elegimos si la operación será de tipo suma o resta
        bool isSumOperation = Random.value > 0.5f;
        manager.sums = isSumOperation;

        // Actualizamos el sprite del símbolo
        //manager.OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/Ui/Gameplay/Suma");
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>(
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
            int y = 4;


            int startIndex = Random.Range(0, manager.unlockedNumbersInList);

            // Probar con cada candidato de la lista (este candidato será la incógnita)
            for (int offset = 0; offset < manager.unlockedNumbersInList; offset++)
            {
                int i = (startIndex + offset) % manager.unlockedNumbersInList;
                int z = manager.numbersList[i];

                // Calcular el resultado (enemyNumber) según el tipo de operación
                int enemy = isSumOperation ? (x + y + z) : (x + y - z);

                // Validar que el resultado esté dentro de los límites del juego
                if (enemy >= 1 && enemy <= 9)
                {
                    found = true;
                    foundX = x;
                    foundY = y;
                    foundZ = z;
                    foundEnemy = enemy;
                    break;
                }
            }
        }

        if (!found)
        {
            Debug.LogError("No se pudo generar una operación válida con los números disponibles.");
            return;
        }

        // Asignar los valores al manager
        manager.operationNumber = foundX;          // X
        manager.secondOperationNumber = foundY;    // Y
        manager.enemyNumber = foundEnemy;          // C (resultado)

        Transform parentTransf = manager.FindChildRecursive(manager.level2.transform, "1rstOperation").transform;
        Transform parentTransf2 = manager.FindChildRecursive(manager.FindChildRecursive(manager.level2.transform, "1rstOperation"), "BossExtra").transform;

        manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf, true, parentTransf);
        manager.AssignNumberPrefab(manager.operationNumber, manager.secondOperationNumberTransf, true, parentTransf2);
        //manager.AssignNumberPrefab(manager.secondOperationNumber, manager.operationNumberTransf, true, parentTransf);

        manager.PlayOperationEntryAnimation(parentTransf.gameObject);

        //string opSymbol = isSumOperation ? "+" : "-";
        //Debug.Log($"Operación generada: {foundX} + {foundY} {opSymbol} Z = {foundEnemy}  |  Z válido: {foundZ}");
    }

    public void OnCorrectAnswer(int operationIndex)
    {
     
    }


    public void OnWrongAnswer()
    {
        Debug.Log("Respuesta incorrecta en Boss Doble Operación.");
    }

    public void Update() { }

    private void GenerateBossOperation()
    {
       
    }

    public bool CheckAnswer(int number, int operationIndex)
    {
        GameObject solutionSlot = manager.level2.transform.Find("1rstOperation").Find("SolutionSlot").gameObject;
        if (manager.sums)
        {
            if(manager.operationNumber + manager.secondOperationNumber + number == manager.enemyNumber)
            {
                manager.number4.GetComponent<Animator>().SetTrigger("attack");
                return true;
            }
            else
            {
                manager.numberOfErrors++;
                manager.RestoreNumberToSlot(solutionSlot, true);
            }
            
        }
        else
        {
            if (manager.operationNumber + manager.secondOperationNumber - number == manager.enemyNumber)
            {
                manager.number4.GetComponent<Animator>().SetTrigger("attack");
                return true;
            }
            else
            {
                manager.numberOfErrors++;
                manager.RestoreNumberToSlot(solutionSlot, true);
            }
        }
        return false;
    }

    public void OnAnswer(int number, int operationIndex)
    {
        
    }

}
