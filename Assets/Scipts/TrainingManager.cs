using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance;

    [Header("Characters")]
    public Animator marcusAnimator;
    public Animator jamieAnimator;
    public Animator alexAnimator;

    [Header("UI")]
    public TextMeshProUGUI stepTitleText;
    public Slider timelineSlider;

    [Header("Steps")]
    private CPRStep[] steps;

    private int currentStep = 0;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
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

    public void OnPlayPressed()
    {
        isPlaying = true;
        PlayCurrentStep();
    }

    public void OnPausePressed()
    {
        isPlaying = false;
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

        switch (step.character)
        {
            case "Marcus":
                ResetAllTriggers(marcusAnimator);
                marcusAnimator.SetTrigger(step.triggerName);
                break;
            case "Jamie":
                ResetAllTriggers(jamieAnimator);
                jamieAnimator.SetTrigger(step.triggerName);
                break;
            case "Alex":
                ResetAllTriggers(alexAnimator);
                alexAnimator.SetTrigger("OnKneel");
                Invoke("PlayAlexCPR", 1.5f);
                break;
        }

        UpdateUI();
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