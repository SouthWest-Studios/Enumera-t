using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class OperationGenerator
{
    public static int PosibleSolution(
    bool sums,
    int numberToCheck,
    bool alreadyUsed,
    int initialRange,
    int finalRange,
    int enemyNumberUsed,
    List<int> numbersList,
    List<int> alreadyUsedNumbers,
    int unlockedNumbersInList,
    int numberToAvoid = 0
)
    {
        if (unlockedNumbersInList == 0 || numbersList.Count == 0)
            return 0;

        // Comprobar si existe alguna solución posible
        bool existeSolucion = false;
        for (int candidate = initialRange; candidate < finalRange; candidate++)
        {
            if (candidate == numberToAvoid) continue;

            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (candidate + listNumber == enemyNumberUsed)
                    {
                        existeSolucion = true;
                        break;
                    }
                }
                else
                {
                    if (candidate - listNumber == enemyNumberUsed)
                    {
                        existeSolucion = true;
                        break;
                    }
                }
            }

            if (existeSolucion) break;
        }

        if (!existeSolucion)
        {
            Debug.Log("no encontrado");
                return 0;
        }
            

        // Buscar número válido
        int intentos = 0;
        int maxIntentos = 100;

        do
        {
            intentos++;

            if (alreadyUsed)
            {
                numberToCheck = Random.Range(initialRange, finalRange);
            }
            else
            {
                do
                {
                    numberToCheck = Random.Range(initialRange, finalRange);
                } while (alreadyUsedNumbers.Contains(numberToCheck) || numberToCheck == numberToAvoid); //Evita el número prohibido
            }

            // Comprobar si este número genera solución
            bool posibleSolution = false;
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (numberToCheck + listNumber == enemyNumberUsed)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
                else
                {
                    if (numberToCheck - listNumber == enemyNumberUsed)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
            }

            if (posibleSolution && numberToCheck != numberToAvoid) //Verificación extra antes de retornar
                return numberToCheck;

        } while (intentos < maxIntentos);

        return 0;
    }
}
