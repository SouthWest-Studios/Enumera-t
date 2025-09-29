using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Refs")]
    public AudioMixer mainAudioMixer;
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Params")]
    public string sfxParam = "sfxVolume";
    public string musicParam = "musicVolume";

    const string SfxKey = "SET_SFX";
    const string MusicKey = "SET_MUSIC";
    const float MinDb = -80f;     
    const float DefaultLinear = 0.75f;

    void Awake()
    {
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    void OnDestroy()
    {
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }

    void Start()
    {
        if (sfxSlider.minValue < 0f) sfxSlider.minValue = 0f;
        if (musicSlider.minValue < 0f) musicSlider.minValue = 0f;
        if (sfxSlider.maxValue > 1f) sfxSlider.maxValue = 1f;
        if (musicSlider.maxValue > 1f) musicSlider.maxValue = 1f;

        var sfx = PlayerPrefs.GetFloat(SfxKey, DefaultLinear);
        var music = PlayerPrefs.GetFloat(MusicKey, DefaultLinear);

        sfxSlider.SetValueWithoutNotify(sfx);
        musicSlider.SetValueWithoutNotify(music);

        ApplyVolume(sfxParam, sfx);
        ApplyVolume(musicParam, music);
    }

    void OnSfxChanged(float v)
    {
        ApplyVolume(sfxParam, v);
        PlayerPrefs.SetFloat(SfxKey, v);
    }

    void OnMusicChanged(float v)
    {
        ApplyVolume(musicParam, v);
        PlayerPrefs.SetFloat(MusicKey, v);
    }

    void ApplyVolume(string param, float linear01)
    {
        float dB = linear01 <= 0.0001f ? MinDb : Mathf.Log10(linear01) * 20f;
        mainAudioMixer.SetFloat(param, dB);
    }

    static float DbToLinear(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }
}
