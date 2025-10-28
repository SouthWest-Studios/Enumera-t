using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsCanvasAnimations : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] CanvasGroup canvasGroup;              // SettingsCanvas
    [SerializeField] RectTransform background;             // Background
    [SerializeField] RectTransform settingsPanel;          // SettingsPanel
    [SerializeField] Slider sfxVolume;                     // SFXVolume
    [SerializeField] Slider musicVolume;                   // MusicVolume
    [SerializeField] Button howToPlayButton;               // HowToPlayButton
    [SerializeField] Button closeSettingsButton;           // CloseSettingsButton
    [SerializeField] CanvasGroup credits;                   // Credits
    [SerializeField] CanvasGroup esborrarProgres;

    [Header("Durations")]
    [SerializeField] float openDuration = 0.6f;
    [SerializeField] float closeDuration = 0.4f;
    [SerializeField] float stagger = 0.06f;

    [Header("Hover / Press")]
    [SerializeField] float hoverScale = 1.05f;
    [SerializeField] float hoverTime = 0.15f;
    [SerializeField] float pressScale = 0.96f;
    [SerializeField] float pressTime = 0.08f;

    [Header("Offsets")]
    [SerializeField] Vector2 bgOffset = new Vector2(0, -60);
    [SerializeField] Vector2 panelOffset = new Vector2(0, -80);

    [Header("Easings")]
    [SerializeField] Ease openEase = Ease.OutCubic;
    [SerializeField] Ease closeEase = Ease.InCubic;

    // cache
    Vector2 bgStartPos, panelStartPos;
    Sequence _openSeq, _closeSeq;
    readonly Dictionary<Transform, Tween> _hoverTweens = new();

    public static SettingsCanvasAnimations instance;

    void Awake()
    {
        instance = this;
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        CacheInitials();
        HideInstant();
        WireButtonAnimations();
    }

    void OnDisable()
    {
        KillAllTweens();
    }

    void CacheInitials()
    {
        bgStartPos = background ? background.anchoredPosition : Vector2.zero;
        panelStartPos = settingsPanel ? settingsPanel.anchoredPosition : Vector2.zero;
    }

    void KillAllTweens()
    {
        _openSeq?.Kill();
        _closeSeq?.Kill();
        foreach (var kv in _hoverTweens) kv.Value?.Kill();
        _hoverTweens.Clear();

        DOTween.Kill(canvasGroup);
        if (background) DOTween.Kill(background);
        if (settingsPanel) DOTween.Kill(settingsPanel);
        if (howToPlayButton) DOTween.Kill(howToPlayButton.transform);
        if (closeSettingsButton) DOTween.Kill(closeSettingsButton.transform);
        if (sfxVolume) DOTween.Kill(sfxVolume.transform);
        if (musicVolume) DOTween.Kill(musicVolume.transform);
    }

    public void PlayOpenEsborrar()
    {
        Sequence openSeq = DOTween.Sequence();
        openSeq.Join(esborrarProgres.DOFade(1f, openDuration).SetEase(openEase));
        openSeq.OnComplete(() =>
        {
            esborrarProgres.interactable = true;
            esborrarProgres.blocksRaycasts = true;
        });
    }

    public void PlayCloseEsborrar()
    {
        esborrarProgres.interactable = false;
        esborrarProgres.blocksRaycasts = false;
        Sequence openSeq = DOTween.Sequence();
        openSeq.Join(esborrarProgres.DOFade(0f, openDuration).SetEase(openEase));

    }


    public void PlayOpenCredits()
    {
        Sequence openSeq = DOTween.Sequence();
        openSeq.Join(credits.DOFade(1f, openDuration).SetEase(openEase));
        openSeq.OnComplete(() =>
        {
            credits.interactable = true;
            credits.blocksRaycasts = true;
        });
    }

    public void PlayCloseCredits()
    {
        credits.interactable = false;
        credits.blocksRaycasts = false;
        Sequence openSeq = DOTween.Sequence();
        openSeq.Join(credits.DOFade(0f, openDuration).SetEase(openEase));
        
    }

    // ---------- API ----------
    public void PlayOpen()
    {
        AudioManager.Instance.PlayOpenPanel();
        KillAllTweens();
        PrepareForOpen();

        _openSeq = DOTween.Sequence();

        // fade canvas
        _openSeq.Join(canvasGroup.DOFade(1f, openDuration).SetEase(openEase));

        // background
        if (background)
        {
            var bgCg = GetOrAddCanvasGroup(background.gameObject);
            bgCg.alpha = 0f;
            _openSeq.Join(bgCg.DOFade(1f, openDuration).SetEase(openEase));
            _openSeq.Join(background.DOAnchorPos(bgStartPos, openDuration).From(bgStartPos + bgOffset).SetEase(openEase));
        }

        // panel
        if (settingsPanel)
        {
            var pCg = GetOrAddCanvasGroup(settingsPanel.gameObject);
            pCg.alpha = 0f;
            settingsPanel.localScale = Vector3.one * 0.95f;
            _openSeq.Join(pCg.DOFade(1f, openDuration).SetEase(openEase));
            _openSeq.Join(settingsPanel.DOAnchorPos(panelStartPos, openDuration).From(panelStartPos + panelOffset).SetEase(openEase));
            _openSeq.Join(settingsPanel.DOScale(1f, openDuration).SetEase(openEase));
        }

        // children stagger: sliders y botones
        var items = GetStaggerItems();
        for (int i = 0; i < items.Count; i++)
        {
            var rt = items[i];
            var cg = GetOrAddCanvasGroup(rt.gameObject);
            cg.alpha = 0f;
            rt.localScale = Vector3.one * 0.95f;

            _openSeq.Insert(stagger * i, cg.DOFade(1f, openDuration * 0.6f).SetEase(openEase));
            _openSeq.Insert(stagger * i, rt.DOScale(1f, openDuration * 0.6f).SetEase(Ease.OutBack, 1.2f));
        }

        _openSeq.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void PlayClose()
    {
        AudioManager.Instance.PlayClosePanel();
        KillAllTweens();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _closeSeq = DOTween.Sequence();

        // children out
        var items = GetStaggerItems();
        for (int i = 0; i < items.Count; i++)
        {
            var rt = items[i];
            var cg = GetOrAddCanvasGroup(rt.gameObject);
            _closeSeq.Insert(stagger * i, rt.DOScale(0.95f, closeDuration * 0.5f).SetEase(closeEase));
            _closeSeq.Insert(stagger * i, cg.DOFade(0f, closeDuration * 0.5f).SetEase(closeEase));
        }

        // panel out
        if (settingsPanel)
        {
            var pCg = GetOrAddCanvasGroup(settingsPanel.gameObject);
            _closeSeq.Join(pCg.DOFade(0f, closeDuration).SetEase(closeEase));
            _closeSeq.Join(settingsPanel.DOAnchorPos(panelStartPos + panelOffset, closeDuration).SetEase(closeEase));
            _closeSeq.Join(settingsPanel.DOScale(0.95f, closeDuration).SetEase(closeEase));
        }

        // background out
        if (background)
        {
            var bgCg = GetOrAddCanvasGroup(background.gameObject);
            _closeSeq.Join(bgCg.DOFade(0f, closeDuration).SetEase(closeEase));
            _closeSeq.Join(background.DOAnchorPos(bgStartPos + bgOffset, closeDuration).SetEase(closeEase));
        }

        _closeSeq.Join(canvasGroup.DOFade(0f, closeDuration).SetEase(closeEase));
    }

    // Hover genérico para botones o handles de slider
    public void HoverEnter(Transform t)
    {
        if (!t) return;
        StopHover(t);
        _hoverTweens[t] = t.DOScale(hoverScale, hoverTime).SetEase(Ease.OutQuad);
    }

    public void HoverExit(Transform t)
    {
        if (!t) return;
        StopHover(t);
        _hoverTweens[t] = t.DOScale(1f, hoverTime).SetEase(Ease.OutQuad);
    }

    // Pulso corto al hacer click o soltar
    public void PressPulse(Transform t)
    {
        if (!t) return;
        t.DOKill(true);
        t.DOPunchScale(Vector3.one * (1f - pressScale), pressTime * 2f, 6, 0.6f);
    }

    // Micro feedback al mover sliders
    public void SliderTick(RectTransform sliderHandle, float punch = 0.06f, float time = 0.12f)
    {
        if (!sliderHandle) return;
        sliderHandle.DOKill(true);
        sliderHandle.DOPunchScale(Vector3.one * punch, time, 4, 0.8f);
    }

    // ---------- Helpers ----------
    void PrepareForOpen()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (background)
        {
            background.anchoredPosition = bgStartPos + bgOffset;
            GetOrAddCanvasGroup(background.gameObject).alpha = 0f;
        }
        if (settingsPanel)
        {
            settingsPanel.anchoredPosition = panelStartPos + panelOffset;
            settingsPanel.localScale = Vector3.one * 0.95f;
            GetOrAddCanvasGroup(settingsPanel.gameObject).alpha = 0f;
        }

        foreach (var rt in GetStaggerItems())
        {
            if (!rt) continue;
            GetOrAddCanvasGroup(rt.gameObject).alpha = 0f;
            rt.localScale = Vector3.one * 0.95f;
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
            background.anchoredPosition = bgStartPos + bgOffset;
            GetOrAddCanvasGroup(background.gameObject).alpha = 0f;
        }
        if (settingsPanel)
        {
            settingsPanel.anchoredPosition = panelStartPos + panelOffset;
            settingsPanel.localScale = Vector3.one * 0.95f;
            GetOrAddCanvasGroup(settingsPanel.gameObject).alpha = 0f;
        }

        foreach (var rt in GetStaggerItems())
        {
            if (!rt) continue;
            GetOrAddCanvasGroup(rt.gameObject).alpha = 0f;
            rt.localScale = Vector3.one * 0.95f;
        }
    }

    List<RectTransform> GetStaggerItems()
    {
        var list = new List<RectTransform>(4);
        if (sfxVolume) list.Add(sfxVolume.transform as RectTransform);
        if (musicVolume) list.Add(musicVolume.transform as RectTransform);
        if (howToPlayButton) list.Add(howToPlayButton.transform as RectTransform);
        if (closeSettingsButton) list.Add(closeSettingsButton.transform as RectTransform);
        return list;
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        var cg = go.GetComponent<CanvasGroup>();
        if (!cg) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }

    void StopHover(Transform t)
    {
        if (_hoverTweens.TryGetValue(t, out var tw) && tw.IsActive()) tw.Kill();
        t.DOKill();
    }

    void WireButtonAnimations()
    {
        // Opcional: añade feedback sin escribir más código
        if (howToPlayButton)
        {
            howToPlayButton.onClick.AddListener(() => PressPulse(howToPlayButton.transform));
        }
        if (closeSettingsButton)
        {
            closeSettingsButton.onClick.AddListener(() => PressPulse(closeSettingsButton.transform));
        }
    }
}
