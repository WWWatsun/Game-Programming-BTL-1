using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Pool;

public class TargetPool : MonoBehaviour
{
    [SerializeField] private TargetScript_2D targetPrefab;

    [SerializeField] private float spawnHeightLimit = 3.5f;
    [SerializeField] private float spawnWidthLimit = 4.0f;

    [SerializeField] private float timeToLive = 5.0f;
    const float maxTTL = 5.0f;
    const float minTTL = 2.0f;
    const float rateOfTTLChange = 0.2f;

    //Singleton Pattern
    public static TargetPool Instance { get; private set; }
    private IObjectPool<TargetScript_2D> pool;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        pool = new ObjectPool<TargetScript_2D>(OnCreate, OnGet, OnRelease, OnDestroyPoolObject);
    }

    TargetScript_2D OnCreate()
    {
        TargetScript_2D target = GameObject.Instantiate(targetPrefab);
        target.SetPool(pool);
        target.gameObject.SetActive(false);
        return target;
    }

    void OnGet(TargetScript_2D target)
    {
        target.transform.position = new Vector3(Random.Range(-spawnWidthLimit, spawnWidthLimit), 
                                            Random.Range(-spawnHeightLimit, spawnHeightLimit),
                                            0);
        target.SetTTL(timeToLive);
        target.gameObject.SetActive(true);
    } 

    void OnRelease(TargetScript_2D target)
    {
        target.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(TargetScript_2D target)
    {
        Destroy(target.gameObject);
    }

    public TargetScript_2D StartSpawning()
    {
        TargetScript_2D target = pool.Get();
        return target;
    }

    public void CleanUp(TargetScript_2D target)
    {
        pool.Release(target);
    }

    public void DecreaseTTL()
    {
        timeToLive = Mathf.Max(minTTL, timeToLive - rateOfTTLChange);
    }

    public void IncreaseTTL()
    {
        timeToLive = Mathf.Min(maxTTL, timeToLive + rateOfTTLChange);
    }

    public void SetDefaultParameters()
    {
        timeToLive = maxTTL;
    }
}
