using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public float spawnRangeX = 8f;
    public float spawnRangeY = 4f;

    private GameObject currentTarget;

    public void SpawnTarget()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);

        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        currentTarget = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

        // Gửi reference của spawner vào target
        currentTarget.GetComponent<SphereTarget>().SetSpawner(this);
    }

    public void OnTargetDestroyed()
    {
        currentTarget = null;

        if (OtherUIManager.Instance.IsPlaying())
        {
            SpawnTarget();
        }
    }
    public void ResetSpawner()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }
    }
}