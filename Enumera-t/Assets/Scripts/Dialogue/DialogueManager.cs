using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject characterAnimatedSlot;
    public DialogueCanvasAnimations dialogueAnimations;

    private Queue<DialogueSentence> sentences;
    private UnityAction onDialogueFinish;
    private string currentCharacter = "";

    public static DialogueManager instance;

    private int letterCount = 0;
    public float letterDelay = 0.02f;

    private bool _active;       // evita reentradas
    private const int MaxSentences = 100000; // límite duro

    private bool hasTouched = false;

    private float dialogueCoolDown = 0;
    private float dialogueMaxCoolDown = 0.2f;

    private void Awake()
    {
        instance = this;
        sentences = new Queue<DialogueSentence>(64);
    }

    void Start() { /* vacío a propósito */ }

    public void StartDialogue(Dialogo dialogo, UnityAction onDialogueFinish = null)
    {
        if (_active) { Debug.LogWarning("[Dialogue] Reentrante ignorado"); return; }
        if (dialogo == null || dialogo.sentences == null || dialogo.sentences.Count() == 0)
        {
            Debug.LogError("[Dialogue] Dialogo vacío o nulo"); return;
        }

        // clamp para evitar reservar colas absurdas por datos corruptos
        int count = dialogo.sentences.Count();
        if (count > MaxSentences)
        {
            Debug.LogError($"[Dialogue] Count desmesurado: {count} > {MaxSentences}. Se trunca.");
            count = MaxSentences;
        }

        // limpiar/instanciar personaje
        foreach (Transform child in characterAnimatedSlot.transform) Destroy(child.gameObject);
        if (dialogo.sentences[0].characterAnimated == null)
        {
            Debug.LogError("[Dialogue] characterAnimated nulo en la primera frase"); return;
        }
        Instantiate(dialogo.sentences[0].characterAnimated, characterAnimatedSlot.transform);

        dialogueAnimations.PlayEnter();
        this.onDialogueFinish = onDialogueFinish;

        // pre-dimensionar para evitar SetCapacity durante Enqueue
        sentences = new Queue<DialogueSentence>(Mathf.Max(64, count));
        for (int i = 0; i < count; i++)
        {
            var s = dialogo.sentences[i];
            if (s.sentence == null) continue;
            sentences.Enqueue(s);
        }

        _active = true;
        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if (!_active) return;

        if (sentences.Count == 0) { EndDialogue(); return; }

        if (sentences.Peek().mistery)
        {
            AudioManager.Instance.PlayMapMystery();
        }


        var sentence = sentences.Dequeue();
        StopAllCoroutines();

        if (sentence.characterAnimated == null)
        {
            Debug.LogWarning("[Dialogue] characterAnimated nulo en frase. Se mantiene el actual.");
        }
        else if (currentCharacter != sentence.characterAnimated.name)
        {
            currentCharacter = sentence.characterAnimated.name;
            dialogueAnimations.ChangeCharacter(sentence.characterAnimated);
        }
        StartCoroutine(TypeSentence(sentence.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        yield return new WaitForSeconds(0.4f);
        letterCount = 0;

        foreach (char letter in sentence ?? string.Empty)
        {
            if (letter == ' ')
            {
                if (++letterCount >= 2)
                {
                    dialogueAnimations.NudgeCharacter();
                    letterCount = 0;
                }
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        dialogueAnimations.NudgeDialogueBox();
    }

    void EndDialogue()
    {
        dialogueAnimations.PlayExit();
        currentCharacter = "";
        _active = false;

        var cb = onDialogueFinish;
        onDialogueFinish = null;
        cb?.Invoke();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // Para PC/WebGL con mouse
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.PlayNextDialogueSound();
            DisplayNextSentences();
        }
#endif

        // Para móvil (iOS/Android)
#if UNITY_IOS || UNITY_ANDROID
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            AudioManager.Instance.PlayNextDialogueSound();
            DisplayNextSentences();
        }
    }
#endif
    }

}
