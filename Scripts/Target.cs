using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class Target : MonoBehaviour
{
    // References
    private float timeToLive;
    private Coroutine ttlCoroutine;
    private IObjectPool<Target> pool;
    private float reactionTime = 0;
    private bool isHit = false;

    private void Update()
    {
        reactionTime += Time.deltaTime;
        timeToLive -= Time.deltaTime;

        if (GameManager.Instance != null && GameManager.Instance.GetGameState() == GameManager.GameState.RESULTS)
        {
            timeToLive = 0;
        }
    }

    // Method to set the pool reference
    public void SetPool(IObjectPool<Target> targetPool)
    {
        pool = targetPool;
    }

    // Method to set the time to live
    public void SetTimeToLive(float timeToLive)
    {
        this.timeToLive = timeToLive;
    }

    public void TargetHit()
    {
        Debug.Log("Target hit!");
        timeToLive = 0;
        isHit = true;
        GameManager.Instance.RegisterHit(reactionTime);
    }

    private void OnEnable()
    {
        isHit = false;
        reactionTime = 0;

        // Start TTL coroutine and keep its reference so we can stop the exact coroutine.
        if (ttlCoroutine != null)
        {
            StopCoroutine(ttlCoroutine);
            ttlCoroutine = null;
        }
        ttlCoroutine = StartCoroutine(TargetCoroutine());
    }

    private void OnDisable()
    {
        if (ttlCoroutine != null)
        {
            StopCoroutine(ttlCoroutine);
            ttlCoroutine = null;
        }
    }

    private IEnumerator TargetCoroutine()
    {
        yield return new WaitWhile(() => timeToLive > 0);

        // If the target expired without being hit, register a miss.
        if (!isHit && GameManager.Instance != null && GameManager.Instance.GetGameState() == GameManager.GameState.PLAYING)
        {
            GameManager.Instance.RegisterMiss();
            TargetBool.Instance.ResetTimeToLive();
        }

        // Give it back to the pool, or disable as a fallback.
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
