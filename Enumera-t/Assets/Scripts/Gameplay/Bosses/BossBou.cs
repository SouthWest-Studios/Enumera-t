using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static GameplayManager;

public class BossBou : IBossBehavior
{
    private GameplayManager manager;

    private int lastSolution = 0;
    private int lastZPosition = -1; // 0 = X, 1 = Y, 2 = Z (posici�n anterior de la inc�gnita)
    private GameObject temporalNumber;
    public int damageTaken = 2;
    public Transform bouPosition;

    public int requiredZ;

    bool firstTime = true;



    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        manager.FindChildRecursive(
            manager.FindChildRecursive(manager.level2.transform, "1rstOperation"),
            "BossExtra"
        ).gameObject.SetActive(true);
        manager.RestoreNumberToSlot(manager.solutionSlot);
        manager.solutionSlot.SetActive(false);
        manager.damage = damageTaken;
        bouPosition = manager.bossList[2].GetComponent<Transform>();
        
        
    }

    public void GenerateOperation()
    {
        //Debug.Log("generate");
        //Debug.Log(firstTime);

        if (!firstTime)
        {
            Animator bossAnimator = bouPosition.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                // Inicia la animaci�n del boss
                bossAnimator.SetTrigger("Swall");

                // Inicia una corrutina que esperar� hasta que acabe la animaci�n
                manager.StartCoroutine(WaitForAnimationAndGenerate(bossAnimator));
            }
            else
            {
                //Debug.LogWarning("No se encontr� el Animator en bouPosition, generando operaci�n inmediatamente.");
                GenerateOperationInternal();
            }
        }
        else
        {
            GenerateOperationInternal();
            firstTime = false;
        }
    }


    private void GenerateOperationInternal()
    {

        bool isSumOperation = Random.value > 0.5f;
        manager.sums = isSumOperation;

        // Actualizar s�mbolos
        manager.OperationSymbolImage.sprite = Resources.Load<Sprite>("Sprites/Ui/Gameplay/Suma");
        manager.operationSymbolImage2.sprite = Resources.Load<Sprite>(
            isSumOperation ? "Sprites/Ui/Gameplay/Suma" : "Sprites/Ui/Gameplay/Resta"
        );

        const int minVal = 1;
        const int maxVal = 9;
        const int maxIntentos = 300;

        bool found = false;

        int a = 0, b = 0, c = 0;      // valores base
        int x = 0, y = 0, z = 0;      // valores finales (x,y,z-> operands)
        int enemy = 0;
        int zPosition = 0; // 0 = primera posici�n (Z + A + B), 1 = segunda, 2 = tercera


        do
        {
            zPosition = Random.Range(0, 3);
        } while (zPosition == lastZPosition);

        for (int intento = 0; intento < maxIntentos && !found; intento++)
        {
            // Generar tres n�meros base
            a = Random.Range(minVal, maxVal + 1);
            b = Random.Range(minVal, maxVal + 1);
            c = Random.Range(minVal, maxVal + 1);

            // Si existe lastSolution, colocarlo en alguna posici�n distinta de la inc�gnita
            if (lastSolution > 0 && lastZPosition != zPosition)
            {
                if (lastZPosition == 0) a = lastSolution;
                else if (lastZPosition == 1) b = lastSolution;
                else c = lastSolution;

                //Debug.Log($"Manteniendo lastSolution {lastSolution} en la posici�n {lastZPosition}");
            }
            int startIndex = Random.Range(0, manager.unlockedNumbersInList);


            for (int offset = 0; offset < manager.unlockedNumbersInList; offset++)
            {
                int i = (startIndex + offset) % manager.unlockedNumbersInList;
                int zCandidate = manager.numbersList[i];

                // Construir (x,y,z) seg�n la posici�n de la inc�gnita
                if (zPosition == 0)
                {
                    x = zCandidate; 
                    y = b;
                    z = c;
                }
                else if (zPosition == 1)
                {
                    x = a;
                    y = zCandidate; 
                    z = c;
                }
                else 
                {
                    x = a;
                    y = b;
                    z = zCandidate; 
                }


                enemy = isSumOperation ? (x + y + z) : (x + y - z);


                if (enemy >= 1 && enemy <= 9)
                {
                    found = true;


                    requiredZ = zCandidate;

                    break;
                }
            }
        }

        if (!found)
        {
            //Debug.LogError("No se pudo generar una operaci�n v�lida para BossBou.");
            lastSolution = 0;
            GenerateOperationInternal();
            return;
        }


        lastZPosition = zPosition;
        manager.enemyNumber = enemy;

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

        Transform parentTransf = manager.FindChildRecursive(manager.level3.transform, "1rstOperation").transform;
        string opSymbol = isSumOperation ? "+" : "-";

        if (zPosition == 0)
        {
            manager.solutionBossSlot.SetActive(true);
            manager.solutionBossSlot2.SetActive(false);
            manager.solutionBossSlot3.SetActive(false);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf2.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.operationNumberTransf3.transform, true, parentTransf);

            //Debug.Log($"[BossBou] Operaci�n generada: Z + {manager.operationNumber} {opSymbol} {manager.secondOperationNumber} = {manager.enemyNumber} | Z posici�n: {zPosition} | Z correcto: {requiredZ}");
        }
        else if (zPosition == 1)
        {
            manager.solutionBossSlot.SetActive(false);
            manager.solutionBossSlot2.SetActive(true);
            manager.solutionBossSlot3.SetActive(false);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.operationNumberTransf3.transform, true, parentTransf);

            //Debug.Log($"[BossBou] Operaci�n generada: {manager.operationNumber} + Z {opSymbol} {manager.secondOperationNumber} = {manager.enemyNumber} | Z posici�n: {zPosition} | Z correcto: {requiredZ}");
        }
        else
        {
            manager.solutionBossSlot.SetActive(false);
            manager.solutionBossSlot2.SetActive(false);
            manager.solutionBossSlot3.SetActive(true);
            manager.solutionBossSlot4.SetActive(false);

            manager.AssignNumberPrefab(manager.enemyNumber, manager.enemyTransf.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.operationNumber, manager.operationNumberTransf.transform, true, parentTransf);
            manager.AssignNumberPrefab(manager.secondOperationNumber, manager.operationNumberTransf2.transform, true, parentTransf);

            //Debug.Log($"[BossBou] Operaci�n generada: {manager.operationNumber} + {manager.secondOperationNumber} {opSymbol} Z = {manager.enemyNumber} | Z posici�n: {zPosition} | Z correcto: {requiredZ}");
        }
        if(temporalNumber)
        {
            UnityEngine.Object.Destroy(temporalNumber);
        }


        manager.PlayOperationEntryAnimation(parentTransf.gameObject);
    }


    public bool CheckAnswer(int number, int operationIndex)
    {

        GameObject solutionSlot;
        // Construimos a,b,c seg�n lastZPosition:
        int a, b, c;
        if (lastZPosition == 0)
        {
            a = number;                      // Z en primera posici�n
            b = manager.operationNumber;     // primer n�mero visible
            c = manager.secondOperationNumber;
            solutionSlot = manager.solutionBossSlot;
        }
        else if (lastZPosition == 1)
        {
            a = manager.operationNumber;
            b = number;                      // Z en segunda posici�n
            c = manager.secondOperationNumber;
            solutionSlot = manager.solutionBossSlot2;
        }
        else // lastZPosition == 2
        {
            a = manager.operationNumber;
            b = manager.secondOperationNumber;
            c = number;                      // Z en tercera posici�n
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
            temporalNumber = UnityEngine.Object.Instantiate(manager.numbersListPrefab[solutionSlot.transform.GetChild(0).GetComponent<NumberUi>().number - 1]);
            temporalNumber.transform.SetParent(parentTransf, false);
            
            temporalNumber.transform.localScale = solutionSlot.transform.GetChild(0).localScale;
            RectTransform tempRect = temporalNumber.GetComponent<RectTransform>();
            RectTransform targetRect = solutionSlot.transform.GetChild(0).GetComponent<RectTransform>();
            RectTransform parentRect = parentTransf.GetComponent<RectTransform>();


            Vector3 worldPos = targetRect.TransformPoint(new Vector3(30, 70, 0));


            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                null,
                out localPoint
            );

            tempRect.anchoredPosition = localPoint;

            manager.RestoreNumberToSlot(solutionSlot);
            solutionSlot.SetActive(false);

            lastSolution = number; 
            
        }
        else
        {
            manager.numberOfErrors++;
            manager.RestoreNumberToSlot(solutionSlot, true);
            //Debug.Log($"[BossBou] Respuesta INCORRECTA. Intentaste: {number}. Esperado: {requiredZ} (pero depende de la posici�n)");
        }

        return correct;
    }


    public void OnCorrectAnswer(int operationIndex) { }
    public void OnWrongAnswer() { /*Debug.Log("Respuesta incorrecta en BossBou.");*/ }
    public void OnAnswer(int number, int operationIndex) { }
    public void Update() { }

    public IEnumerator AnimateNumbersToBoss(Transform bossTarget, System.Action onComplete = null)
    {
        //Debug.Log("AnimateNumbersToBoss started");

        // Clonamos la lista de n�meros actuales
        List<GameObject> numbersToAnimate = new List<GameObject>(manager.bouOperationNumbers);
        manager.bouOperationNumbers.Clear();

        float duration = 1.2f;
        float rotationSpeed = 360f;

        // Guardamos posiciones iniciales y escalas
        List<Vector3> startLocalPositions = new List<Vector3>();
        List<Vector3> startScales = new List<Vector3>();

        foreach (var num in numbersToAnimate)
        {
            if (num == null) continue;

            RectTransform numRT = num.GetComponent<RectTransform>();
            startLocalPositions.Add(numRT.localPosition);
            startScales.Add(numRT.localScale);
        }

        // Buscamos el transform del Graphics (que se mueve)
        Transform graphicsTransform = bossTarget.Find("Graphics");
        if (graphicsTransform == null)
        {
            //Debug.LogWarning("No se encontr� 'Graphics' como hijo del bossTarget. Usando bossTarget directamente.");
            graphicsTransform = bossTarget;
        }

        Transform commonParent = numbersToAnimate[0].transform.parent;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);

            // Convertir posici�n del Graphics al espacio local del padre de los n�meros
            Vector3 graphicsWorldPos = graphicsTransform.position;
            Vector3 graphicsLocalPos = commonParent.InverseTransformPoint(graphicsWorldPos);

            foreach (var num in numbersToAnimate)
            {
                if (num == null) continue;
                RectTransform numRT = num.GetComponent<RectTransform>();

                int i = numbersToAnimate.IndexOf(num);
                numRT.localPosition = Vector3.Lerp(startLocalPositions[i], graphicsLocalPos, t);
                numRT.localScale = Vector3.Lerp(startScales[i], Vector3.zero, t);
                numRT.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }

        //Debug.Log("Animation finished. Destroying numbers...");

        foreach (var num in numbersToAnimate)
        {
            if (num != null) GameObject.Destroy(num);
        }

        //Debug.Log("All numbers destroyed. Calling onComplete callback.");
        onComplete?.Invoke();
    }


    public void OnSwallQuarter()
    {
        
        manager.StartCoroutine(AnimateNumbersToBoss(bouPosition, () => GenerateOperationInternal()));
    }

    private IEnumerator WaitForAnimationAndGenerate(Animator bossAnimator)
    {
        // Espera hasta que la animaci�n "Swall" comience realmente
        yield return null;
        yield return new WaitUntil(() => bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swall"));

        // Obtiene la duraci�n del clip activo
        float duration = bossAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Espera a que acabe la animaci�n
        yield return new WaitForSeconds(duration);

        // Ahora s�, genera la nueva operaci�n
        //GenerateOperationInternal();
    }

    


}


