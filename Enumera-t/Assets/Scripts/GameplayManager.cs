using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{

    public int enemyNumber;

    public Image enemyImage;

    public GameObject solutionSlot;

    public Image operationNumberImage;

    public int operationNumber;

    public Image OperationSymbolImage;

    public bool sums = true;

    public List<NumbersSlot> slots;

    public List<int> numbersList;

    public int unlockedNumbersInList;

    private List<int> alreadyUsedNumbers = new List<int>();

    void Start()
    {
        RoundCompleted();
        AssignNumberImage(enemyNumber, enemyImage);
        AssignNumberImage(operationNumber, operationNumberImage);

        if(sums)
        {
            OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/plus");
        }
        else
        {
            OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/minus");
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if(i < unlockedNumbersInList)
            {
                slots[i].transform.GetChild(0).GetComponent<NumberUi>().number = numbersList[i];
                AssignNumberImage(numbersList[i], slots[i].transform.GetChild(0).GetComponent<Image>());
            }
            else
            {
                slots[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }

    private void AssignNumberImage(int number, Image image)
    {
        // Construimos el path en base al número
        string path = "Sprites/numbers/number" + number;

        // Cargamos el sprite desde Resources
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("No se encontró sprite en: " + path);
        }
    }

    public void AnswerGuess(int number)
    {
        bool victory = false;
        if(sums)
        {
            if (operationNumber + number == enemyNumber)
            {
                victory = true;
            }
            else
            {
                victory= false;
            }
        }
        else
        {
            if (operationNumber - number == enemyNumber)
            {
                victory = true;
                
            }
            else
            {
                victory = false;
                
            }
        }

        if (victory)
        {
            print("BONA RESPOSTA!");
            RoundCompleted();
        }
        else
        {
            print("INCORRECTE");
        }
        
    }

    public void RoundCompleted()
    {
        // Límite de intentos global para evitar bucle infinito
        int intentosGlobales = 0;
        int maxIntentosGlobales = 50;
        bool numeroValido = false;

        if (unlockedNumbersInList == 0 || numbersList.Count == 0)
        {
            Debug.LogError("No hay números disponibles en numbersList o unlockedNumbersInList es 0.");
            return;
        }

        while (!numeroValido && intentosGlobales < maxIntentosGlobales)
        {
            intentosGlobales++;

            if (sums)
            {
                if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 5)
                {
                    operationNumber = PosibleSolution(operationNumber, false, 1, 6);
                }
                else
                {
                    operationNumber = PosibleSolution(operationNumber, true, 1, 6);
                }
            }
            else
            {
                if (alreadyUsedNumbers.Count > 0 && alreadyUsedNumbers.Count < 10 - enemyNumber)
                {
                    operationNumber = PosibleSolution(operationNumber, false, enemyNumber, 10);
                    print("aaaaaaaaaaa");
                }
                else
                {
                    operationNumber = PosibleSolution(operationNumber, true, enemyNumber, 10);
                    print("bbbbbbbbbbbb");
                }
            }

            if (operationNumber != 0)
            {
                numeroValido = true;
            }
            else
            {
                alreadyUsedNumbers.Clear(); // reset si no hay solución
            }
        }

        if (!numeroValido)
        {
            Debug.LogError("No se pudo encontrar un número válido para esta ronda.");
            return;
        }

        alreadyUsedNumbers.Add(operationNumber);

        // Asignar sprite de operación
        AssignNumberImage(operationNumber, operationNumberImage);

        // Mover primer hijo de solutionSlot a un slot vacío
        if (solutionSlot != null)
        {
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (slots[i] != null && slots[i].transform.childCount == 0 && solutionSlot.transform.childCount > 0)
                {
                    solutionSlot.transform.GetChild(0).SetParent(slots[i].transform);
                }
            }
        }
    }


    private int PosibleSolution(int numberTocheck, bool alreadyUsed, int initialRange, int finalRange)
    {
        if (unlockedNumbersInList == 0 || numbersList.Count == 0)
            return 0;
        print("cccccccccccccccc");
        // Comprobar si existe alguna solución posible
        bool existeSolucion = false;
        for (int candidate = initialRange; candidate < finalRange; candidate++)
        {
            print(candidate);
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (candidate + listNumber == enemyNumber)
                    {
                        existeSolucion = true;
                        break;
                    }
                }
                else
                {
                    print("nnnnnnnnnnnnnnnnnnnnn");
                    if (candidate - listNumber == enemyNumber)
                    {
                        existeSolucion = true;
                        break;
                    }
                }
            }
            if (existeSolucion) break;
        }

        if (!existeSolucion) return 0;

        // Buscar número válido
        int intentos = 0;
        int maxIntentos = 100;

        do
        {
            intentos++;

            if (alreadyUsed)
            {
                numberTocheck = Random.Range(initialRange, finalRange);
            }
            else
            {
                do
                {
                    numberTocheck = Random.Range(initialRange, finalRange);
                } while (alreadyUsedNumbers.Contains(numberTocheck));
            }

            // Comprobar si este número genera solución
            bool posibleSolution = false;
            for (int i = 0; i < unlockedNumbersInList; i++)
            {
                if (i >= numbersList.Count) break;
                int listNumber = numbersList[i];
                if (sums)
                {
                    if (numberTocheck + listNumber == enemyNumber)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
                else
                {
                    if (listNumber - numberTocheck == enemyNumber)
                    {
                        posibleSolution = true;
                        break;
                    }
                }
            }

            if (posibleSolution)
                return numberTocheck;

        } while (intentos < maxIntentos);

        return 0; // fallback seguro
    }






}
