using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX")]
    public AudioClip correctSound;
    public AudioClip correctSound2;
    public AudioClip wrongSound;
    public AudioClip grabSound;
    public AudioClip dropSound;
    public AudioClip badDropSound;
    public AudioClip openPanelSound;
    public AudioClip closePanelSound;
    public AudioClip startLevelSound;
    public AudioClip OneStarSound;
    public AudioClip TwoStarSound;
    public AudioClip ThreeStarSound;
    [Header("Music")]
    public AudioClip battleExplorationMusic;
    public AudioClip mapChillMusic;
    public AudioClip mapMisteryMusic;
    public AudioClip introBossMusic;
    public AudioClip bossFightMusic;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Gameplay" && battleExplorationMusic != null)
        {
            PlayBattleExploration();

        }
        else if (SceneManager.GetActiveScene().name == "MapScene" && mapChillMusic != null)
        {
            PlayMapChill();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayCorrect() => PlaySFX(correctSound);
    public void PlayWrong() => PlaySFX(wrongSound);
    public void PlayGrab() => PlaySFX(grabSound);
    public void PlayDrop() => PlaySFX(dropSound);
    public void PlayBadDrop() => PlaySFX(badDropSound);
    public void PlayOpenPanel() => PlaySFX(openPanelSound);
    public void PlayClosePanel() => PlaySFX(closePanelSound);
    public void PlayLevelStart() => PlaySFX(startLevelSound);
    public void PlayOneStar() => PlaySFX(OneStarSound);
    public void PlayTwoStar() => PlaySFX(TwoStarSound);
    public void PlayThreeStar() => PlaySFX(ThreeStarSound);

    public void PlayBattleExploration() => PlayMusic(battleExplorationMusic);
    public void PlayMapChill() => PlayMusic(mapChillMusic);
    public void PlayMapMystery() => PlayMusic(mapMisteryMusic);
    public void PlayIntroBoss() => PlayMusic(introBossMusic);
    public void PlayBossFight() => PlayMusic(bossFightMusic);
    
}
