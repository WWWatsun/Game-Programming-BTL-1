using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class TargetBool2D : MonoBehaviour
{
    public static TargetBool2D Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Target2D targetPrefab;
    [SerializeField] private Camera spawnCamera;

    [Header("Pooling")]
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    [Header("Difficulty (Normal preset)")]
    [SerializeField] private float gameDuration = 60f;

    // TTL decreases smoothly over time (NOT per hit)
    [SerializeField] private float ttlStart = 1.35f; // early game
    [SerializeField] private float ttlEnd = 0.65f;   // late game

    // Target2D shrinks smoothly over time
    [SerializeField] private float scaleStart = 1.00f;
    [SerializeField] private float scaleEnd = 0.70f;

    [Header("Spawn")]
    [SerializeField] private float spawnDelayAfterDisappear = 0.05f; // small breathing room

    public Target2D ActiveTarget { get; private set; }

    private IObjectPool<Target2D> pool;
    private Coroutine spawnRoutine;
    private float elapsed; // seconds since PLAYING started

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (spawnCamera == null) spawnCamera = Camera.main;

        pool = new ObjectPool<Target2D>(
            createFunc: CreateTarget,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyTarget,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private void Update()
    {
        if (GameManager2D.Instance == null) return;
        if (GameManager2D.Instance.GetGameState() == GameManager2D.GameState.PLAYING)
            elapsed += Time.deltaTime;
    }

    private Target2D CreateTarget()
    {
        var t = Instantiate(targetPrefab);
        t.SetPool(pool);
        t.gameObject.SetActive(false);
        return t;
    }

    private void OnDestroyTarget(Target2D t)
    {
        if (t != null) Destroy(t.gameObject);
    }

    private void OnGet(Target2D t)
    {
        if (spawnCamera == null) spawnCamera = Camera.main;

        float difficultyT = Mathf.Clamp01(elapsed / gameDuration);

        float ttl = Mathf.Lerp(ttlStart, ttlEnd, difficultyT);
        float scale = Mathf.Lerp(scaleStart, scaleEnd, difficultyT);
        // Bán kính random nhẹ ~±10% mỗi lần spawn để gameplay zui hơn
        float jitter = Random.Range(0.90f, 1.10f);
        scale *= jitter;

        t.transform.localScale = new Vector3(scale, scale, 1f);

        float radius = GetRadiusWorld(t);
        t.transform.position = GetRandomVisiblePosition(radius);

        t.SetTimeToLive(ttl);
        t.gameObject.SetActive(true);

        ActiveTarget = t;
    }

    private void OnRelease(Target2D t)
    {
        if (ActiveTarget == t) ActiveTarget = null;
        if (t != null) t.gameObject.SetActive(false);
    }

    private float GetRadiusWorld(Target2D t)
    {
        float radius = 0.5f;
        var sr = t.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) radius = Mathf.Max(sr.bounds.extents.x, sr.bounds.extents.y);
        return radius;
    }

    private Vector3 GetRandomVisiblePosition(float r)
    {
        Vector3 camPos = spawnCamera.transform.position;

        float vert = spawnCamera.orthographicSize;
        float horz = vert * spawnCamera.aspect;

        float minX = camPos.x - horz + r;
        float maxX = camPos.x + horz - r;
        float minY = camPos.y - vert + r;
        float maxY = camPos.y + vert - r;

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        return new Vector3(x, y, 0f);
    }

    public void StartSpawning()
    {
        elapsed = 0f;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        // clean up active Target2D
        if (ActiveTarget != null)
        {
            pool.Release(ActiveTarget);
            ActiveTarget = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        // wait until PLAYING
        while (GameManager2D.Instance == null || GameManager2D.Instance.GetGameState() != GameManager2D.GameState.PLAYING)
            yield return null;

        while (GameManager2D.Instance != null && GameManager2D.Instance.GetGameState() == GameManager2D.GameState.PLAYING)
        {
            // spawn only when none active
            if (ActiveTarget == null)
            {
                pool.Get();
            }

            yield return null;
        }
    }

    public void RequestRespawnWithDelay()
    {
        if (spawnRoutine != null) StartCoroutine(RespawnDelayRoutine());
    }

    private IEnumerator RespawnDelayRoutine()
    {
        yield return new WaitForSeconds(spawnDelayAfterDisappear);

        if (GameManager2D.Instance != null &&
            GameManager2D.Instance.GetGameState() == GameManager2D.GameState.PLAYING &&
            ActiveTarget == null)
        {
            pool.Get();
        }
    }
}