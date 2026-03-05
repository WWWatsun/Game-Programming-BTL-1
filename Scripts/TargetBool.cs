using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using UnityEditor;

public class TargetBool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Target targetPrefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 50;
    [SerializeField] private float timeToLiveDecreaseRate = 0.25f;

    [SerializeField] private float minTargetSpawnDistance = 5f;
    [SerializeField] private float maxTargetSpawnDistance = 20f;

    [SerializeField] private float minTargetSpawnHeight = 1f;
    [SerializeField] private float maxTargetSpawnHeight = 5f;

    [SerializeField] private float minTargetTimeToLive = 0.1f;
    [SerializeField] private float maxTargetTimeToLive = 5f;

    [SerializeField] private float minTargetSpawnDelay = 0.1f;
    [SerializeField] private float maxTargetSpawnDelay = 0.25f;

    private float timeToLive = 5f;

    // Singleton instance
    public static TargetBool Instance { get; private set; }

    // The pool holds plain GameObjects
    private IObjectPool<Target> pool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        // Create a pool with the four core callbacks.
        pool = new ObjectPool<Target>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            defaultCapacity: this.defaultCapacity,
            maxSize: this.maxSize
        );
    }

    void Start()
    {
        timeToLive = maxTargetTimeToLive;
    }

    // Creates a new pooled Target the first time (and whenever the pool needs more).
    private Target CreateItem()
    {
        Target pooledTarget = GameObject.Instantiate(targetPrefab);
        pooledTarget.name = "PooledTarget";
        pooledTarget.SetPool(pool);
        pooledTarget.gameObject.SetActive(false);
        return pooledTarget;
    }

    // Called when an item is taken from the pool.
    private void OnGet(Target target)
    {
        target.transform.position = new Vector3(Random.Range(-maxTargetSpawnDistance, maxTargetSpawnDistance), 
                                                Random.Range(minTargetSpawnHeight, maxTargetSpawnHeight), 
                                                Random.Range(minTargetSpawnDistance, maxTargetSpawnDistance));
        target.SetTimeToLive(timeToLive);
        target.gameObject.SetActive(true);
        
    }

    // Called when an item is returned to the pool.
    private void OnRelease(Target target)
    {
        target.gameObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(Target target)
    {
        Destroy(target.gameObject);
    }

    private IEnumerator SpawnTargetRoutine()
    {
        Target target = pool.Get();
        yield return new WaitUntil(() => !target.gameObject.activeSelf);
        yield return new WaitForSeconds(Random.Range(minTargetSpawnDelay, maxTargetSpawnDelay)); // Random delay before spawning the next target
        StartCoroutine(SpawnTargetRoutine());
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnTargetRoutine());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public void ResetTimeToLive()
    {
        timeToLive = maxTargetTimeToLive;
    }

    public void ReduceTimeToLive()
    {
        timeToLive = Mathf.Max(minTargetTimeToLive, timeToLive - timeToLiveDecreaseRate);
    }
}
