using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class TargetScript_2D : MonoBehaviour
{
    private IObjectPool<TargetScript_2D> pool;
    

    [SerializeField] private float targetTTL;
    private Coroutine ttlCoroutine;

    private void Update()
    {
        targetTTL -= Time.deltaTime;
    }

    public void SetPool(IObjectPool<TargetScript_2D> pool)
    {
        this.pool = pool;
    }

    public void SetTTL(float targetTTL)
    {
        this.targetTTL = targetTTL;
    }

    private void OnEnable()
    {
        if (ttlCoroutine != null)
        {
            StopCoroutine(ttlCoroutine);
            ttlCoroutine = null;
        }
        ttlCoroutine = StartCoroutine(Lifetime());
    }

    private void OnDisable()
    {
        if(ttlCoroutine != null)
        {
            StopCoroutine(ttlCoroutine);
            ttlCoroutine = null;
        }
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(targetTTL);
        pool.Release(this);
        GameManager2D.Instance.RegisterMiss();
    }

    public void OnHit()
    {
        Debug.Log("Hit");
        pool.Release(this);
        gameObject.SetActive(false);
        GameManager2D.Instance.RegisterHit();
    }
}
