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

    [Header("2D Spawn Settings")]
    [SerializeField] private Camera spawnCamera;

    public Target ActiveTarget { get; private set; }


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
    void OnGet(Target target)
{
    if (spawnCamera == null) spawnCamera = Camera.main;

    // Estimate radius from sprite bounds (world units) so target doesn't clip screen edges.
    float radius = 0.5f;
    var sr = target.GetComponentInChildren<SpriteRenderer>();
    if (sr != null) radius = Mathf.Max(sr.bounds.extents.x, sr.bounds.extents.y);

    Vector3 pos = GetRandomVisiblePosition(radius);
    target.transform.position = pos;

    target.SetTimeToLive(timeToLive);
    target.gameObject.SetActive(true);

    ActiveTarget = target;
}

private Vector3 GetRandomVisiblePosition(float r)
{
    float z = 0f; // 2D scene z plane
    Vector3 camPos = spawnCamera.transform.position;

    float vertExtent = spawnCamera.orthographicSize;
    float horzExtent = vertExtent * spawnCamera.aspect;

    float minX = camPos.x - horzExtent + r;
    float maxX = camPos.x + horzExtent - r;
    float minY = camPos.y - vertExtent + r;
    float maxY = camPos.y + vertExtent - r;

    float x = Random.Range(minX, maxX);
    float y = Random.Range(minY, maxY);

    return new Vector3(x, y, z);
}


    // Called when an item is returned to the pool.
    void OnRelease(Target target)
{
    if (ActiveTarget == target) ActiveTarget = null;
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
