using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Reference Points")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private GunVisual gunVisual;
    [SerializeField] private ParticleSystem muzzleFlash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to handle shooting logic when the gun is fired
    public void Fire()
    {
        Debug.DrawRay(raycastPoint.position, raycastPoint.forward, Color.red);
        gunVisual.PlayFireAnimation();
        muzzleFlash.Play();
        if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out RaycastHit hitInfo, Mathf.Infinity, targetLayerMask))
        {
            if (hitInfo.collider.TryGetComponent<Target>(out Target target))
            {
                target.TargetHit();
            }
        }
        else
        {
            GameManager.Instance.RegisterWhip();
        }
    }
}
