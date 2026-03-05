using System.Collections;
using UnityEngine;

public class GunVisual : MonoBehaviour
{
    [SerializeField] private float fireAnimationDuration = 0.01f; // Duration of the fire animation in seconds
    private const string FIRE = "Fire";
    private Animator animator;
    private Coroutine fireAnimationCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayFireAnimationCoroutine()
    {
        if (animator != null)
        {
            animator.SetTrigger(FIRE);
            yield return new WaitForSeconds(fireAnimationDuration);
            animator.ResetTrigger(FIRE);
        }
    }

    public void PlayFireAnimation()
    {
        fireAnimationCoroutine = StartCoroutine(PlayFireAnimationCoroutine());
    }
}
