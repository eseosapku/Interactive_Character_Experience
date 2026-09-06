using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [Header("Screens")]
    public GameObject screen1;
    public GameObject screen2;
    public GameObject screen3;
    public GameObject screen4;
    public GameObject completionScreen;
    private GameObject activeScreen;

    public void GoToScreen1() => ShowScreen(screen1);
    public void GoToScreen2() => ShowScreen(screen2);
    public void GoToScreen3() => ShowScreen(screen3);
    public void GoToScreen4() => ShowScreen(screen4);
    public void GoToCompletion() => ShowScreen(completionScreen);

    void Awake()
    {
        Instance = this;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen1.SetActive(false);
        screen2.SetActive(false);
        screen3.SetActive(false);
        screen4.SetActive(false);
        completionScreen.SetActive(false);
        screen1.SetActive(true);
        SetVisible(screen1, true);
        activeScreen = screen1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen(GameObject nextScreen)
    {
        // Fade out current screen
        if (activeScreen != null)
            SetVisible(activeScreen, false);

        // Switch after outro plays
        StartCoroutine(SwitchAfterDelay(nextScreen, 0.3f));
    }

    System.Collections.IEnumerator SwitchAfterDelay(
        GameObject nextScreen, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (activeScreen != null)
            activeScreen.SetActive(false);

        activeScreen = nextScreen;
        activeScreen.SetActive(true);
        SetVisible(activeScreen, true);
    }

    void SetVisible(GameObject screen, bool visible)
    {
        Animator anim = screen.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
            anim.SetBool("IsVisible", visible);
    }
}
