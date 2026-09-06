using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance;

    [Header("Characters")]
    public Animator marcusAnimator;
    public Animator jamieAnimator;
    public Animator alexAnimator;

    [Header("Scene Objects")]
    public GameObject diagramBoard;
    public GameObject marcusCharacter;
    public GameObject jamieCharacter;
    public GameObject alexCharacter;

    [Header("UI")]
    public TextMeshProUGUI stepTitleText;
    public Slider timelineSlider;

    [Header("Audio")]
    public AudioSource marcusAudioSource;
    public AudioClip marcusSpeechClip;

    // Each marcus section defined by
    // title, animation trigger, audio start, audio end
    private MarcusSection[] marcusSections;
    private int currentSection = 0;
    private bool isPaused = false;
    private Coroutine activeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (diagramBoard != null) diagramBoard.SetActive(false);
        if (marcusCharacter != null) marcusCharacter.SetActive(false);
        if (jamieCharacter != null) jamieCharacter.SetActive(false);
        if (alexCharacter != null) alexCharacter.SetActive(false);

        // Define each section
        marcusSections = new MarcusSection[]
        {
            new MarcusSection {
                stepTitle = "Step 1 — Scene Safety",
                trigger = "OnTalk",
                audioStart = 0f,
                audioEnd = 19f
            },
            new MarcusSection {
                stepTitle = "Step 2 — Check Responsiveness",
                trigger = "OnPoint",
                audioStart = 19f,
                audioEnd = 34f
            },
            new MarcusSection {
                stepTitle = "Step 3 — Call for Help",
                trigger = "OnTalk",
                audioStart = 34f,
                audioEnd = 54.1f
            },
            new MarcusSection {
                stepTitle = "Step 4 — CPR Technique",
                trigger = "OnPoint",
                audioStart = 54.1f,
                audioEnd = 74f
            },
            new MarcusSection {
                stepTitle = "Step 5 — Rescue Breaths",
                trigger = "OnTalk",
                audioStart = 74f,
                audioEnd = 95f
            },
            new MarcusSection {
                stepTitle = "Now watch the demonstration...",
                trigger = "OnWalk",
                audioStart = 95f,
                audioEnd = 101f
            }
        };
    }

    public void StartTraining()
    {
        diagramBoard.SetActive(true);
        marcusCharacter.SetActive(true);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        currentSection = 0;
        isPaused = false;

        PlaySection(currentSection);
    }

    void PlaySection(int index)
    {
        if (index >= marcusSections.Length) return;

        MarcusSection section = marcusSections[index];

        // Stop any running coroutine
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        // Reset triggers
        marcusAnimator.speed = 1f;
        marcusAnimator.ResetTrigger("OnTalk");
        marcusAnimator.ResetTrigger("OnPoint");
        marcusAnimator.ResetTrigger("OnWalk");

        // Update UI
        stepTitleText.text = section.stepTitle;
        timelineSlider.value = (float)index / (marcusSections.Length - 1);

        // Play animation
        marcusAnimator.SetTrigger(section.trigger);

        // Play audio from correct timestamp
        marcusAudioSource.clip = marcusSpeechClip;
        marcusAudioSource.time = section.audioStart;
        marcusAudioSource.Play();

        // Start coroutine to auto advance
        float duration = section.audioEnd - section.audioStart;
        activeCoroutine = StartCoroutine(
            WaitThenAdvance(duration, index));
    }

    IEnumerator WaitThenAdvance(float duration, int index)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isPaused)
                elapsed += Time.deltaTime;

            yield return null;
        }

        // Stop audio at end of section
        marcusAudioSource.Stop();

        // If last section — go to demonstration
        if (index >= marcusSections.Length - 1)
        {
            StartDemonstration();
        }
        // Otherwise wait for user to press Next
        // Auto advance removed — user controls it
    }

    void StartDemonstration()
    {
        marcusCharacter.SetActive(false);
        diagramBoard.SetActive(false);
        jamieCharacter.SetActive(true);
        alexCharacter.SetActive(true);

        stepTitleText.text = "Demonstration — Watch Carefully";
        timelineSlider.value = 1f;

        // Jamie walks in then collapses
        StartCoroutine(DemonstrationSequence());
    }

    IEnumerator DemonstrationSequence()
    {
        // Jamie walks in
        jamieAnimator.SetTrigger("OnCollapse");
        yield return new WaitForSeconds(3f);

        // Alex walks in and kneels
        alexAnimator.SetTrigger("OnKneel");
        yield return new WaitForSeconds(2f);

        // Alex performs CPR
        alexAnimator.SetTrigger("OnCPR");
        yield return new WaitForSeconds(5f);

        // Jamie wakes up
        jamieAnimator.SetTrigger("OnRevived");
        yield return new WaitForSeconds(3f);

        // Go to completion screen
        UIManager.Instance.GoToCompletion();
    }

    // ── Button handlers ────────────────────────

    public void OnPlayPressed()
    {
        if (isPaused)
        {
            isPaused = false;
            marcusAnimator.speed = 1f;
            marcusAudioSource.UnPause();
        }
    }

    public void OnPausePressed()
    {
        isPaused = true;
        marcusAnimator.speed = 0f;
        marcusAudioSource.Pause();
    }

    public void OnNextPressed()
    {
        if (currentSection < marcusSections.Length - 1)
        {
            currentSection++;
            isPaused = false;
            PlaySection(currentSection);
        }
    }

    public void OnPreviousPressed()
    {
        if (currentSection > 0)
        {
            currentSection--;
            isPaused = false;
            PlaySection(currentSection);
        }
    }
}

// Data container for each section
[System.Serializable]
public class MarcusSection
{
    public string stepTitle;
    public string trigger;
    public float audioStart;
    public float audioEnd;
}