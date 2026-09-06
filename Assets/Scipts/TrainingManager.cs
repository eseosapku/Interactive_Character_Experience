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

    private CPRStep[] steps;
    private int currentStep = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Hide everything at start
        if (diagramBoard != null) diagramBoard.SetActive(false);
        if (marcusCharacter != null) marcusCharacter.SetActive(false);
        if (jamieCharacter != null) jamieCharacter.SetActive(false);
        if (alexCharacter != null) alexCharacter.SetActive(false);

        steps = new CPRStep[]
        {
            new CPRStep {
                stepTitle = "Step 1 of 5 — Scene is Safe",
                character = "Marcus",
                triggerName = "OnTalk"
            },
            new CPRStep {
                stepTitle = "Step 2 of 5 — Check for Breathing",
                character = "Marcus",
                triggerName = "OnPoint"
            },
            new CPRStep {
                stepTitle = "Step 3 of 5 — Call for Help",
                character = "Marcus",
                triggerName = "OnTalk"
            },
            new CPRStep {
                stepTitle = "Step 4 of 5 — Patient Collapses",
                character = "Jamie",
                triggerName = "OnCollapse"
            },
            new CPRStep {
                stepTitle = "Step 5 of 5 — Perform CPR",
                character = "Alex",
                triggerName = "OnCPR"
            }
        };

        UpdateUI();
    }

    public void StartTraining()
    {
        diagramBoard.SetActive(true);
        marcusCharacter.SetActive(true);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        currentStep = 0;
        UpdateUI();
        StartCoroutine(PlayMarcusSequence());
    }

    public void OnPlayPressed()
    {
        PlayCurrentStep();
    }

    public void OnPausePressed()
    {
        marcusAnimator.speed = 0f;
        jamieAnimator.speed = 0f;
        alexAnimator.speed = 0f;
    }

    public void OnNextPressed()
    {
        if (currentStep < steps.Length - 1)
        {
            currentStep++;
            ResumeAnimators();
            PlayCurrentStep();
        }
        else
        {
            UIManager.Instance.GoToCompletion();
        }
    }

    public void OnPreviousPressed()
    {
        if (currentStep > 0)
        {
            currentStep--;
            ResumeAnimators();
            PlayCurrentStep();
        }
    }

    void PlayCurrentStep()
    {
        CPRStep step = steps[currentStep];

        marcusCharacter.SetActive(false);
        jamieCharacter.SetActive(false);
        alexCharacter.SetActive(false);

        switch (step.character)
        {
            case "Marcus":
                marcusCharacter.SetActive(true);
                StartCoroutine(PlayMarcusSequence());
                break;
            case "Jamie":
                jamieCharacter.SetActive(true);
                jamieAnimator.SetTrigger(step.triggerName);
                break;
            case "Alex":
                alexCharacter.SetActive(true);
                alexAnimator.SetTrigger("OnKneel");
                Invoke("PlayAlexCPR", 1.5f);
                break;
        }

        UpdateUI();
    }

    IEnumerator PlayMarcusSequence()
    {
        Debug.Log("Marcus Sequence Started");

        marcusAnimator.speed = 1f;
        marcusAnimator.ResetTrigger("OnTalk");
        marcusAnimator.ResetTrigger("OnPoint");
        marcusAnimator.ResetTrigger("OnWalk");

        // Play audio straight through from beginning
        marcusAudioSource.clip = marcusSpeechClip;
        marcusAudioSource.Play();

        // Section 1 — Talking (0 to 19 seconds)
        marcusAnimator.SetTrigger("OnTalk");
        yield return new WaitForSeconds(19f);

        // Section 2 — Pointing (19 to 34 seconds)
        marcusAnimator.SetTrigger("OnPoint");
        yield return new WaitForSeconds(15f);

        // Section 3 — Talking (34 to 54.1 seconds)
        marcusAnimator.SetTrigger("OnTalk");
        yield return new WaitForSeconds(20.1f);

        // Section 4 — Pointing (54.1 to 1min14sec)
        marcusAnimator.SetTrigger("OnPoint");
        yield return new WaitForSeconds(19.9f);

        // Section 5 — Talking (1min14sec to 1min35sec)
        marcusAnimator.SetTrigger("OnTalk");
        yield return new WaitForSeconds(21f);

        // Closing — Walk away (1min35sec to 1min41sec)
        marcusAnimator.SetTrigger("OnWalk");
        yield return new WaitForSeconds(6f);

        // Sequence complete — move to next step
        Debug.Log("Marcus Sequence Complete");
        OnNextPressed();
    }

    void PlayAlexCPR()
    {
        alexAnimator.SetTrigger("OnCPR");
    }

    void ResetAllTriggers(Animator anim)
    {
        anim.speed = 1f;
        foreach (AnimatorControllerParameter p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Trigger)
                anim.ResetTrigger(p.name);
        }
    }

    void ResumeAnimators()
    {
        marcusAnimator.speed = 1f;
        jamieAnimator.speed = 1f;
        alexAnimator.speed = 1f;
    }

    void UpdateUI()
    {
        if (stepTitleText != null)
            stepTitleText.text = steps[currentStep].stepTitle;

        if (timelineSlider != null)
            timelineSlider.value = (float)currentStep / (steps.Length - 1);
    }
}