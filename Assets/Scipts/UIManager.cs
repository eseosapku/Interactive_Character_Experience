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
        ShowScreen(screen1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen(GameObject nextScreen)
    {
        if (activeScreen != null)
        {
            activeScreen.GetComponent<Animator>()
                        .SetBool("IsVisible", false);
        }
        activeScreen = nextScreen;
        activeScreen.GetComponent<Animator>()
                    .SetBool("IsVisible", true);
    }
}
