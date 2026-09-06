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
    private Coroutine activeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Stop audio playing on Unity play button
        if (marcusAudioSource != null)
        {
            marcusAudioSource.Stop();
            marcusAudioSource.playOnAwake = false;
        }

        // Hide everything at start
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

    // ── Called when user enters Screen3 ───────
    public void StartTraining()
    {
        diagramBoard.SetActive(true);
        marcusCharacter.SetActive(true);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        // Reset positions
        jamieCharacter.transform.position =
            new Vector3(1f, 0f, 0f);
        alexCharacter.transform.position =
            new Vector3(-1f, 0f, 0f);
        jamieCharacter.transform.rotation =
            Quaternion.Euler(0, 180, 0);
        alexCharacter.transform.rotation =
            Quaternion.Euler(0, 180, 0);

        currentSection = 0;
        isPaused = false;

        PlaySection(currentSection);
    }

    // ── Plays a specific section ───────────────
    void PlaySection(int index)
    {
        if (index < 0 || index >= marcusSections.Length) return;

        MarcusSection section = marcusSections[index];

        // Stop any running coroutine
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        // Stop audio
        marcusAudioSource.Stop();

        // Reset animator speed and triggers
        marcusAnimator.speed = 1f;
        marcusAnimator.ResetTrigger("OnTalk");
        marcusAnimator.ResetTrigger("OnPoint");
        marcusAnimator.ResetTrigger("OnWalk");

        // Update UI
        stepTitleText.text = section.stepTitle;
        timelineSlider.value =
            (float)index / (marcusSections.Length - 1);

        // Play animation trigger
        marcusAnimator.SetTrigger(section.trigger);

        // Play audio from correct timestamp
        marcusAudioSource.clip = marcusSpeechClip;
        marcusAudioSource.time = section.audioStart;
        marcusAudioSource.Play();

        // Start section timer
        float duration = section.audioEnd - section.audioStart;
        activeCoroutine = StartCoroutine(SectionTimer(duration));
    }

    // ── Counts down section duration ──────────
    IEnumerator SectionTimer(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isPaused)
                elapsed += Time.deltaTime;

            yield return null;
        }

        // Stop audio and freeze animation
        marcusAudioSource.Stop();
        marcusAnimator.speed = 0f;

        // Prompt user to continue
        stepTitleText.text = stepTitleText.text +
            " — Press Next Step to continue";

        // If last section start demonstration
        if (currentSection >= marcusSections.Length - 1)
        {
            yield return new WaitForSeconds(1f);
            StartDemonstration();
        }
    }

    // ── Starts Jamie and Alex demonstration ───
    void StartDemonstration()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        marcusAudioSource.Stop();

        // Hide Marcus and diagram
        marcusCharacter.SetActive(false);
        diagramBoard.SetActive(false);

        // Show Jamie and Alex
        jamieCharacter.SetActive(true);
        alexCharacter.SetActive(true);

        // Reset positions side by side
        jamieCharacter.transform.position =
            new Vector3(1f, 0f, 0f);
        alexCharacter.transform.position =
            new Vector3(-1f, 0f, 0f);
        jamieCharacter.transform.rotation =
            Quaternion.Euler(0, 180, 0);
        alexCharacter.transform.rotation =
            Quaternion.Euler(0, 180, 0);

        stepTitleText.text = "Demonstration — Watch Carefully";
        timelineSlider.value = 1f;

        activeCoroutine = StartCoroutine(DemonstrationSequence());
    }

    // ── Full CPR demonstration sequence ───────
    IEnumerator DemonstrationSequence()
    {
        // Both walking together
        stepTitleText.text = "Both walking in...";
        jamieAnimator.Play("Standard Walk");
        alexAnimator.Play("Standard Walk");
        yield return new WaitForSeconds(3f);

        // Jamie collapses
        stepTitleText.text = "Jamie has collapsed!";
        jamieAnimator.SetTrigger("OnCollapse");
        yield return new WaitForSeconds(2f);

        // Alex walks over to Jamie
        stepTitleText.text = "Alex rushing to help...";
        alexAnimator.Play("Standard Walk");
        yield return StartCoroutine(MoveToPosition(
            alexCharacter,
            jamieCharacter.transform.position +
                new Vector3(0.5f, 0f, 0f),
            2f
        ));

        // Alex faces Jamie
        alexCharacter.transform.rotation =
            Quaternion.Euler(0, 90, 0);

        // Alex kneels
        stepTitleText.text = "Alex kneeling down...";
        alexAnimator.SetTrigger("OnKneel");
        yield return new WaitForSeconds(2f);

        // Alex performs CPR
        stepTitleText.text = "Performing CPR — 30 compressions!";
        alexAnimator.SetTrigger("OnCPR");
        yield return new WaitForSeconds(6f);

        // Jamie responds
        stepTitleText.text = "Patient responding!";
        jamieAnimator.SetTrigger("OnRevived");
        yield return new WaitForSeconds(2f);

        // Jamie stands up
        stepTitleText.text = "Patient revived successfully!";
        yield return new WaitForSeconds(2f);

        // Go to completion
        UIManager.Instance.GoToCompletion();
    }

    // ── Smoothly moves character to position ──
    IEnumerator MoveToPosition(
        GameObject character,
        Vector3 targetPos,
        float duration)
    {
        Vector3 startPos = character.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            character.transform.position =
                Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        character.transform.position = targetPos;
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

[System.Serializable]
public class MarcusSection
{
    public string stepTitle;
    public string trigger;
    public float audioStart;
    public float audioEnd;
}