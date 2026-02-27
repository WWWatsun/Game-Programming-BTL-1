using UnityEngine;

public class SphereTarget : MonoBehaviour
{
    private TargetSpawner spawner;

    private float ttl = 3f;           // Time to live
    private float spawnTime;          // When target appeared

    public void SetSpawner(TargetSpawner s)
    {
        spawner = s;
    }

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - spawnTime >= ttl)
        {
            // Timeout → Miss
            OtherUIManager.Instance.AddMiss();
            spawner.OnTargetDestroyed();
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!OtherUIManager.Instance.IsPlaying()) return;

        float reactionTime = Time.time - spawnTime;

        OtherUIManager.Instance.AddScoreWithBonus(ttl, reactionTime);

        spawner.OnTargetDestroyed();

        Destroy(gameObject);
    }
}