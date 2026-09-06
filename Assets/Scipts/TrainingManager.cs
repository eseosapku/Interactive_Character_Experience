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

    private MarcusSection[] marcusSections;
    private int currentSection = 0;
    private bool isPaused = false;
    private bool isInDemonstration = false;
    private Coroutine activeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (marcusAudioSource != null)
        {
            marcusAudioSource.Stop();
            marcusAudioSource.playOnAwake = false;
        }

        if (diagramBoard != null) diagramBoard.SetActive(false);
        if (marcusCharacter != null) marcusCharacter.SetActive(false);
        if (jamieCharacter != null) jamieCharacter.SetActive(false);
        if (alexCharacter != null) alexCharacter.SetActive(false);

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
                audioEnd = 115f
            }
        };
    }

    public void StartTraining()
    {
        diagramBoard.SetActive(true);
        marcusCharacter.SetActive(true);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        isInDemonstration = false;
        currentSection = 0;
        isPaused = false;

        PlaySection(currentSection);
    }

    void PlaySection(int index)
    {
        if (index < 0 || index >= marcusSections.Length) return;

        // Coming back from demonstration
        // make sure correct objects visible
        isInDemonstration = false;
        diagramBoard.SetActive(true);
        marcusCharacter.SetActive(true);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        MarcusSection section = marcusSections[index];

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        marcusAudioSource.Stop();

        marcusAnimator.speed = 1f;
        marcusAnimator.ResetTrigger("OnTalk");
        marcusAnimator.ResetTrigger("OnPoint");
        marcusAnimator.ResetTrigger("OnWalk");

        stepTitleText.text = section.stepTitle;
        timelineSlider.value =
            (float)index / (marcusSections.Length - 1);

        marcusAnimator.SetTrigger(section.trigger);

        marcusAudioSource.clip = marcusSpeechClip;
        marcusAudioSource.time = section.audioStart;
        marcusAudioSource.Play();

        float duration = section.audioEnd - section.audioStart;
        activeCoroutine = StartCoroutine(SectionTimer(duration));
    }

    IEnumerator SectionTimer(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isPaused)
                elapsed += Time.deltaTime;
            yield return null;
        }

        marcusAudioSource.Stop();
        marcusAnimator.speed = 0f;

        stepTitleText.text = stepTitleText.text +
            " — Press Next Step to continue";

        if (currentSection >= marcusSections.Length - 1)
        {
            yield return new WaitForSeconds(1f);
            BeginDemonstration();
        }
    }

    void BeginDemonstration()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        isInDemonstration = true;
        isPaused = false;

        marcusAudioSource.Stop();
        marcusCharacter.SetActive(false);
        diagramBoard.SetActive(false);

        jamieCharacter.SetActive(true);
        alexCharacter.SetActive(true);

        // Reset animators to default state
        jamieAnimator.Rebind();
        jamieAnimator.Update(0f);
        alexAnimator.Rebind();
        alexAnimator.Update(0f);

        stepTitleText.text = "Demonstration — Watch Carefully";
        timelineSlider.value = 1f;

        activeCoroutine = StartCoroutine(DemonstrationSequence());
    }

    IEnumerator DemonstrationSequence()
    {
        // Both walking from default state
        stepTitleText.text = "Both walking in...";
        jamieAnimator.speed = 1f;
        alexAnimator.speed = 1f;

        // Give walk animation time to fully play
        yield return new WaitForSeconds(5f);
        while (isPaused) yield return null;

        // Jamie collapses
        stepTitleText.text = "Jamie has collapsed!";
        jamieAnimator.SetTrigger("OnCollapse");

        // Wait for full collapse animation to finish
        yield return new WaitForSeconds(4f);
        while (isPaused) yield return null;

        // Alex kneels
        stepTitleText.text = "Alex kneeling down...";
        alexAnimator.SetTrigger("OnKneel");
        yield return new WaitForSeconds(3f);
        while (isPaused) yield return null;

        // Alex does CPR — Jamie receives at same time
        stepTitleText.text = "Performing CPR — 30 compressions!";
        alexAnimator.SetTrigger("OnCPR");
        jamieAnimator.SetTrigger("OnReceiveCPR");
        yield return new WaitForSeconds(7f);
        while (isPaused) yield return null;

        // Jamie stands up
        stepTitleText.text = "Patient responding!";
        jamieAnimator.SetTrigger("OnRevived");
        yield return new WaitForSeconds(4f);
        while (isPaused) yield return null;

        stepTitleText.text = "Patient revived successfully!";
        yield return new WaitForSeconds(2f);

        UIManager.Instance.GoToCompletion();
    }

    // ── Button handlers ────────────────────────

    public void OnPlayPressed()
    {
        if (!isPaused) return;

        isPaused = false;

        if (isInDemonstration)
        {
            // Resume demonstration animators
            jamieAnimator.speed = 1f;
            alexAnimator.speed = 1f;
        }
        else
        {
            // Resume Marcus
            marcusAnimator.speed = 1f;
            marcusAudioSource.UnPause();
        }
    }

    public void OnPausePressed()
    {
        isPaused = true;

        if (isInDemonstration)
        {
            // Pause demonstration animators
            jamieAnimator.speed = 0f;
            alexAnimator.speed = 0f;
        }
        else
        {
            // Pause Marcus
            marcusAnimator.speed = 0f;
            marcusAudioSource.Pause();
        }
    }

    public void OnNextPressed()
    {
        // If in demonstration next does nothing
        // demonstration plays through automatically
        if (isInDemonstration) return;

        if (currentSection < marcusSections.Length - 1)
        {
            currentSection++;
            isPaused = false;
            PlaySection(currentSection);
        }
        else
        {
            isPaused = false;
            BeginDemonstration();
        }
    }

    public void OnPreviousPressed()
    {
        isPaused = false;

        if (isInDemonstration)
        {
            // Go back to last Marcus section
            isInDemonstration = false;
            currentSection = marcusSections.Length - 1;
            PlaySection(currentSection);
            return;
        }

        if (currentSection > 0)
        {
            currentSection--;
            PlaySection(currentSection);
        }
    }
}

[System.Serializable]
public class MarcusSection
{
    public string stepTitle;
    public string trigger;
    public float audioStart;
    public float audioEnd;
}