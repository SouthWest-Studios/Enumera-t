using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Experimental.AI;

public class LevelInfoCanvasAnimations : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] CanvasGroup canvasGroup;          // En DialogueCanvas
    [SerializeField] RectTransform background;         // Background
    [SerializeField] RectTransform character;          // Character (Image)
    [SerializeField] RectTransform characterAnimatedSlot;
    [SerializeField] RectTransform dialogueBox;        // DialogueBox
    [SerializeField] List<RectTransform> options;      // DialogueOption1, DialogueOption2, ...

    [Header("Duraciones")]
    [SerializeField] float enterDuration = 0.6f;
    [SerializeField] float exitDuration = 0.4f;
    [SerializeField] float stagger = 0.06f;      // retardo entre elementos

    [Header("Hover")]
    [SerializeField] float hoverScale = 1.05f;
    [SerializeField] float hoverTime = 0.15f;

    [Header("Offsets de entrada")]
    [SerializeField] Vector2 bgEnterOffset = new Vector2(0, -60);
    [SerializeField] Vector2 characterEnterOffset = new Vector2(-80, 0);
    [SerializeField] Vector2 boxEnterOffset = new Vector2(0, -40);

    [Header("Easings")]
    [SerializeField] Ease enterEase = Ease.OutCubic;
    [SerializeField] Ease exitEase = Ease.InCubic;

    // Estados iniciales
    Vector2 bgStartPos, characterStartPos, boxStartPos;
    float bgStartAlpha = 1, characterStartAlpha = 1, boxStartAlpha = 1;

    // Tweens
    Sequence _enterSeq;
    Sequence _exitSeq;
    readonly Dictionary<Transform, Tween> _hoverTweens = new();

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        CacheInitials();
        HideInstant(); // arranca oculto por defecto
    }

    void OnDisable()
    {
        KillAllTweens();
    }

    void CacheInitials()
    {
        bgStartPos = background ? background.anchoredPosition : Vector2.zero;
        characterStartPos = character ? character.anchoredPosition : Vector2.zero;
        boxStartPos = dialogueBox ? dialogueBox.anchoredPosition : Vector2.zero;

        // Si usas imágenes separadas y quieres atenuarlas, añade CanvasGroup a cada una.
        var bgCg = background ? background.GetComponent<CanvasGroup>() : null;
        var chCg = character ? character.GetComponent<CanvasGroup>() : null;
        var boxCg = dialogueBox ? dialogueBox.GetComponent<CanvasGroup>() : null;

        bgStartAlpha = bgCg ? bgCg.alpha : 1f;
        characterStartAlpha = chCg ? chCg.alpha : 1f;
        boxStartAlpha = boxCg ? boxCg.alpha : 1f;
    }

    void KillAllTweens()
    {
        _enterSeq?.Kill();
        _exitSeq?.Kill();
        foreach (var kv in _hoverTweens) kv.Value?.Kill();
        _hoverTweens.Clear();
        DOTween.Kill(canvasGroup);
        if (background) DOTween.Kill(background);
        if (character) DOTween.Kill(character);
        if (dialogueBox) DOTween.Kill(dialogueBox);
        if (options != null)
            foreach (var o in options) if (o) DOTween.Kill(o);
    }

    // ---------- API PÚBLICA ----------

    public void PlayEnter()
    {
        KillAllTweens();
        PrepareForEnter();

        _enterSeq = DOTween.Sequence();

        // Canvas fade in
        _enterSeq.Join(canvasGroup.DOFade(1f, enterDuration).SetEase(enterEase));

        // Background
        if (background)
        {
            var bgCg = background.GetComponent<CanvasGroup>();
            if (bgCg) _enterSeq.Join(bgCg.DOFade(bgStartAlpha, enterDuration).From(0f).SetEase(enterEase));
            _enterSeq.Join(background.DOAnchorPos(bgStartPos, enterDuration).From(bgStartPos + bgEnterOffset).SetEase(enterEase));
        }

        // Character
        if (character)
        {
            var chCg = character.GetComponent<CanvasGroup>();
            if (chCg) _enterSeq.Join(chCg.DOFade(characterStartAlpha, enterDuration).From(0f).SetEase(enterEase));
            _enterSeq.Join(character.DOAnchorPos(characterStartPos, enterDuration).From(characterStartPos + characterEnterOffset).SetEase(enterEase));
            _enterSeq.Join(character.DOScale(1f, enterDuration).From(0.9f).SetEase(enterEase));
        }

        // Dialogue box
        if (dialogueBox)
        {
            var boxCg = dialogueBox.GetComponent<CanvasGroup>();
            if (boxCg) _enterSeq.Join(boxCg.DOFade(boxStartAlpha, enterDuration).From(0f).SetEase(enterEase));
            _enterSeq.Join(dialogueBox.DOAnchorPos(boxStartPos, enterDuration).From(boxStartPos + boxEnterOffset).SetEase(enterEase));
            _enterSeq.Join(dialogueBox.DOScale(1f, enterDuration).From(0.95f).SetEase(enterEase));
        }

        // Options (stagger)
        if (options != null)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var o = options[i];
                if (!o) continue;
                var cg = o.GetComponent<CanvasGroup>();
                if (cg) cg.alpha = 0f;
                o.localScale = Vector3.one * 0.95f;
                _enterSeq.Insert(stagger * i, o.DOScale(1f, enterDuration * 0.6f).SetEase(Ease.OutBack, 1.2f));
                if (cg) _enterSeq.Insert(stagger * i, cg.DOFade(1f, enterDuration * 0.6f).SetEase(enterEase));
            }
        }

        _enterSeq.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void PlayExit()
    {
        KillAllTweens();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _exitSeq = DOTween.Sequence();

        // Options out
        if (options != null)
        {
            for (int i = 0; i < options.Count; i++)
            {
                var o = options[i];
                if (!o) continue;
                var cg = o.GetComponent<CanvasGroup>();
                _exitSeq.Insert(stagger * i, o.DOScale(0.95f, exitDuration * 0.5f).SetEase(exitEase));
                if (cg) _exitSeq.Insert(stagger * i, cg.DOFade(0f, exitDuration * 0.5f).SetEase(exitEase));
            }
        }

        // Box + character + bg
        if (dialogueBox)
        {
            var boxCg = dialogueBox.GetComponent<CanvasGroup>();
            if (boxCg) _exitSeq.Join(boxCg.DOFade(0f, exitDuration).SetEase(exitEase));
            _exitSeq.Join(dialogueBox.DOAnchorPos(boxStartPos + boxEnterOffset, exitDuration).SetEase(exitEase));
            _exitSeq.Join(dialogueBox.DOScale(0.95f, exitDuration).SetEase(exitEase));
        }

        if (character)
        {
            var chCg = character.GetComponent<CanvasGroup>();
            if (chCg) _exitSeq.Join(chCg.DOFade(0f, exitDuration).SetEase(exitEase));
            _exitSeq.Join(character.DOAnchorPos(characterStartPos + characterEnterOffset, exitDuration).SetEase(exitEase));
            _exitSeq.Join(character.DOScale(0.9f, exitDuration).SetEase(exitEase));
        }

        if (background)
        {
            var bgCg = background.GetComponent<CanvasGroup>();
            if (bgCg) _exitSeq.Join(bgCg.DOFade(0f, exitDuration).SetEase(exitEase));
            _exitSeq.Join(background.DOAnchorPos(bgStartPos + bgEnterOffset, exitDuration).SetEase(exitEase));
        }

        _exitSeq.Join(canvasGroup.DOFade(0f, exitDuration).SetEase(exitEase));
    }

    // Hover para opciones (llamar desde EventTrigger o OnPointerEnter/Exit)
    public void HoverEnter(RectTransform option)
    {
        if (!option) return;
        StopHover(option);
        _hoverTweens[option] = option.DOScale(hoverScale, hoverTime).SetEase(Ease.OutQuad);
    }

    public void HoverExit(RectTransform option)
    {
        if (!option) return;
        StopHover(option);
        _hoverTweens[option] = option.DOScale(1f, hoverTime).SetEase(Ease.OutQuad);
    }

    // Micro-animaciones agradables
    public void NudgeCharacter(float strength = 10f, float time = 0.3f)
    {
        if (!character) return;
        character.DOKill(true);
        character.DOPunchAnchorPos(new Vector2(0, strength), time, 6, 0.6f);
    }

    public void NudgeDialogueBox(float strength = 8f, float time = 0.25f)
    {
        if (!dialogueBox) return;
        dialogueBox.DOKill(true);
        dialogueBox.DOPunchScale(Vector3.one * 0.03f, time, 6, 0.6f);
    }

    public void ChangeCharacter(Sprite sprite, float time = 0.3f, float offsetX = 340f, bool fade = true)
    {
        if (!character) return;


        var img = character.GetComponent<Image>();
        if (!img) return;


        var startPos = character.anchoredPosition;


        CanvasGroup cg = character.GetComponent<CanvasGroup>();
        bool useCg = fade && cg != null;
        Color startColor = img.color;


        character.DOKill(true);
        if (useCg) DOTween.Kill(cg);
        else DOTween.Kill(img);


        var seq = DOTween.Sequence();


        seq.Append(character.DOAnchorPos(startPos + new Vector2(-offsetX, 0f), time * 0.5f).SetEase(Ease.InCubic));
        if (fade)
        {
            if (useCg) seq.Join(cg.DOFade(0f, time * 0.5f).SetEase(Ease.InCubic));
            else seq.Join(img.DOFade(0f, time * 0.5f).SetEase(Ease.InCubic));
        }


        seq.AppendCallback(() =>
        {
            img.sprite = sprite;
        });


        seq.Append(character.DOAnchorPos(startPos, time * 0.5f).SetEase(Ease.OutCubic));
        if (fade)
        {
            if (useCg) seq.Join(cg.DOFade(1f, time * 0.5f).SetEase(Ease.OutCubic));
            else seq.Join(img.DOFade(1f, time * 0.5f).SetEase(Ease.OutCubic));
        }
    }

    public void ChangeCharacter(GameObject prefab, float time = 0.3f, float offsetX = 340f, bool fade = true)
    {
        if (!characterAnimatedSlot) return;

        var rt = characterAnimatedSlot;
        var startPos = rt.anchoredPosition;

        var cg = rt.GetComponent<CanvasGroup>();
        bool canFade = fade && cg != null;

        rt.DOKill(true);
        if (canFade) DOTween.Kill(cg);

        var seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(startPos + new Vector2(-offsetX, 0f), time * 0.5f).SetEase(Ease.InCubic));
        if (canFade) seq.Join(cg.DOFade(0f, time * 0.5f).SetEase(Ease.InCubic));

        seq.AppendCallback(() =>
        {
            // destruir hijos actuales
            for (int i = rt.childCount - 1; i >= 0; i--)
                Object.Destroy(rt.GetChild(i).gameObject);

            // instanciar nuevo hijo
            var go = Object.Instantiate(prefab, rt);
            go.GetComponent<RectTransform>().localScale = new Vector2(7, 7);
        });

        seq.Append(rt.DOAnchorPos(startPos, time * 0.5f).SetEase(Ease.OutCubic));
        if (canFade) seq.Join(cg.DOFade(1f, time * 0.5f).SetEase(Ease.OutCubic));
    }

    // ---------- Helpers ----------

    void PrepareForEnter()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (background)
        {
            background.anchoredPosition = bgStartPos + bgEnterOffset;
            var cg = background.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (character)
        {
            character.anchoredPosition = characterStartPos + characterEnterOffset;
            character.localScale = Vector3.one * 0.9f;
            var cg = character.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (dialogueBox)
        {
            dialogueBox.anchoredPosition = boxStartPos + boxEnterOffset;
            dialogueBox.localScale = Vector3.one * 0.95f;
            var cg = dialogueBox.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (options != null)
        {
            foreach (var o in options)
            {
                if (!o) continue;
                var cg = o.GetComponent<CanvasGroup>();
                if (cg) cg.alpha = 0f;
                o.localScale = Vector3.one * 0.95f;
            }
        }
    }

    public void HideInstant()
    {
        KillAllTweens();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (background)
        {
            background.anchoredPosition = bgStartPos + bgEnterOffset;
            var cg = background.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (character)
        {
            character.anchoredPosition = characterStartPos + characterEnterOffset;
            character.localScale = Vector3.one * 0.9f;
            var cg = character.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (dialogueBox)
        {
            dialogueBox.anchoredPosition = boxStartPos + boxEnterOffset;
            dialogueBox.localScale = Vector3.one * 0.95f;
            var cg = dialogueBox.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
        if (options != null)
        {
            foreach (var o in options)
            {
                if (!o) continue;
                var cg = o.GetComponent<CanvasGroup>();
                if (cg) cg.alpha = 0f;
                o.localScale = Vector3.one * 0.95f;
            }
        }
    }

    void StopHover(RectTransform option)
    {
        if (_hoverTweens.TryGetValue(option, out var t) && t.IsActive()) t.Kill();
        option.DOKill(); // cancela cualquier scale previo
    }
}
