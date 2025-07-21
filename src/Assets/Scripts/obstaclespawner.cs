using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefab & Positioning")]
    [Tooltip("Your rectangular obstacle prefab")]
    public GameObject obstaclePrefab;
    [Tooltip("World‑space offset from the mimic where obstacles appear")]
    public Vector2 spawnOffset = new Vector2(5f, 0f);

    [Header("Timing")]
    [Tooltip("How many seconds to wait on start")]
    public float initialDelay = 2f;
    [Tooltip("How many seconds to wait (no spawns) before each burst)")]
    public float pauseDuration = 3f;
    [Tooltip("How many seconds to keep spawning in one burst")]
    public float spawnDuration = 3f;
    [Tooltip("Time between each obstacle spawn during active window")]
    public float spawnInterval = 0.5f;

    private void Start()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("ObstacleSpawner: no obstaclePrefab assigned!", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnCycle());
    }

    private IEnumerator SpawnCycle()
    {
        // Initial delay before anything happens
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // === PAUSE WINDOW ===
            yield return new WaitForSeconds(pauseDuration);

            // === SPAWN WINDOW ===
            float elapsed = 0f;
            while (elapsed < spawnDuration)
            {
                SpawnOnce();
                yield return new WaitForSeconds(spawnInterval);
                elapsed += spawnInterval;
            }
        }
    }

    private void SpawnOnce()
    {
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
    }
}
