using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusic;
    public AudioSource trainingAudio;

    [Header("Music Clips")]
    public AudioClip menuMusicClip;

    [Header("UI Controls")]
    public Slider volumeSlider;
    public UnityEngine.UI.Button muteButton;

    [Header("Mute Button Images")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool isMuted = false;
    private float lastVolume = 1f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Make sure nothing plays on start
        if (backgroundMusic != null)
        {
            backgroundMusic.playOnAwake = false;
            backgroundMusic.loop = true;
            backgroundMusic.Stop();
        }

        if (trainingAudio != null)
        {
            trainingAudio.playOnAwake = false;
            trainingAudio.Stop();
        }

        // Set slider to full volume
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Play menu music on start
        PlayMenuMusic();
    }

    // ── Music controls ─────────────────────────

    public void PlayMenuMusic()
    {
        if (backgroundMusic == null || menuMusicClip == null) return;

        backgroundMusic.clip = menuMusicClip;
        backgroundMusic.volume = isMuted ? 0f : lastVolume;
        backgroundMusic.Play();
    }

    public void StopMenuMusic()
    {
        if (backgroundMusic == null) return;
        StartCoroutine(FadeOut(backgroundMusic, 1f));
    }

    public void StopAllAudio()
    {
        if (backgroundMusic != null) backgroundMusic.Stop();
        if (trainingAudio != null) trainingAudio.Stop();
    }

    // ── Volume controls ────────────────────────

    public void OnVolumeChanged(float value)
    {
        lastVolume = value;

        if (isMuted && value > 0)
        {
            // User moved slider while muted — unmute
            isMuted = false;
            UpdateMuteButton();
        }

        ApplyVolume();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            lastVolume = volumeSlider != null ?
                volumeSlider.value : 1f;
        }
        else
        {
            if (volumeSlider != null)
                volumeSlider.value = lastVolume;
        }

        ApplyVolume();
        UpdateMuteButton();
    }

    void ApplyVolume()
    {
        float targetVolume = isMuted ? 0f : lastVolume;

        if (backgroundMusic != null)
            backgroundMusic.volume = targetVolume;

        if (trainingAudio != null)
            trainingAudio.volume = targetVolume;

        // Also apply to global audio
        AudioListener.volume = isMuted ? 0f : lastVolume;
    }

    void UpdateMuteButton()
    {
        if (muteButton == null) return;

        Image btnImage = muteButton.GetComponent<Image>();
        if (btnImage == null) return;

        btnImage.sprite = isMuted ?
            soundOffSprite : soundOnSprite;
    }

    // ── Fade helpers ───────────────────────────

    public IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(
                startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    public IEnumerator FadeIn(AudioSource source, float duration)
    {
        float targetVolume = isMuted ? 0f : lastVolume;
        source.volume = 0f;
        source.Play();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(
                0f, targetVolume, elapsed / duration);
            yield return null;
        }
    }
}