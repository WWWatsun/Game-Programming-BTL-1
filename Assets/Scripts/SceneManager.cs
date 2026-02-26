using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private int startScreenIndex = 0;
    [SerializeField] private int osuScreenIndex = 1;
    [SerializeField] private int fpsScreenIndex = 2;

    // Singleton instance
    public static SceneManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Persist across scenes
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMenuScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(startScreenIndex);
    }

    public void LoadFPSScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(fpsScreenIndex);
    }

    public void LoadOSUScreen()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(osuScreenIndex);
    }

    // TODO: Add transition coroutines
}
