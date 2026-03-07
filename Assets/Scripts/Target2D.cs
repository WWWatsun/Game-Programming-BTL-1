using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class Target2D : MonoBehaviour
{
    private float timeToLive;
    private Coroutine ttlCoroutine;
    private IObjectPool<Target2D> pool;

    private float reactionTime = 0;
    private bool isHit = false;

    void Update()
    {
        reactionTime += Time.deltaTime;
        timeToLive -= Time.deltaTime;

        if (GameManager2D.Instance != null && GameManager2D.Instance.GetGameState() == GameManager2D.GameState.RESULTS)
            timeToLive = 0;
    }

    public void SetPool(IObjectPool<Target2D> targetPool) => pool = targetPool;
    public void SetTimeToLive(float ttl) => timeToLive = ttl;

    public void TargetHit()
    {
        if (GameManager2D.Instance == null) return;
        if (GameManager2D.Instance.GetGameState() != GameManager2D.GameState.PLAYING) return;

        timeToLive = 0;
        isHit = true;
        GameManager2D.Instance.RegisterHit(reactionTime);
    }

    void OnEnable()
    {
        isHit = false;
        reactionTime = 0;

        if (ttlCoroutine != null) { StopCoroutine(ttlCoroutine); ttlCoroutine = null; }
        ttlCoroutine = StartCoroutine(TargetCoroutine());
    }

    void OnDisable()
    {
        if (ttlCoroutine != null) { StopCoroutine(ttlCoroutine); ttlCoroutine = null; }
    }

    private IEnumerator TargetCoroutine()
    {
        yield return new WaitWhile(() => timeToLive > 0);

        if (!isHit && GameManager2D.Instance != null && GameManager2D.Instance.GetGameState() == GameManager2D.GameState.PLAYING)
            GameManager2D.Instance.RegisterMiss();

        if (pool != null) pool.Release(this);
        else gameObject.SetActive(false);
    }
}