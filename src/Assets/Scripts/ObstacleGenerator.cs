using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public static Queue<GameObject> Obstacles;
    public PlayerController PC;

    [Header("Pool & Movement")]
    public int PoolSize;
    public float Speed;
    public float Smooth;
    public Vector2 WidthRange;
    public Vector2 HeightRange;
    public float Delta;
    public int D;
    public Transform ObstaclesContainer;
    public GameObject Obstacle;
    public float Force;
    public bool mimicPower = false;

    [Header("Mimic Settings")]
    public Transform mimic;               // Reference to mimic GameObject
    public GameObject mimicBlockPrefab;   // Mimic block prefab

    [Header("Speed‐Control Tuning")]
    public float SpeedStep = 0.5f;
    public float MinSpeed = 1f;
    public float MaxSpeed = 10f;

    public Rigidbody2D rb;
    public GameController gc;

    private Vector3 startPos;
    private GameObject top;
    private GameObject bottom;
    private float topHeight;
    private float topWidth;
    private float bottomHeight;
    private float bottomWidth;

    private enum SpawnMode { Obstacles, Mimic }
    private SpawnMode currentMode = SpawnMode.Obstacles;

    public float modeDuration = 3f; // 3 seconds per mode

    // Re‐calculate these whenever Speed or size changes:
    private float topInterval => (topWidth - Smooth / Speed) / Speed;
    private float bottomInterval => (bottomWidth - Smooth / Speed) / Speed;

    private Vector3 topScale => new Vector3(topWidth, topHeight, 1);
    private Vector3 bottomScale => new Vector3(bottomWidth, bottomHeight, 1);

    void Awake()
    {
        startPos = new Vector3(15f, 0f, 0f);
        FillPool();
    }

    void Start()
    {
        StartCoroutine(topRandGen());
        StartCoroutine(bottomRandGen());
        StartCoroutine(SpawnMimicBlocks(2f));
        StartCoroutine(ModeSwitcher());
    }

    /// <summary>
    /// Increase obstacle speed by SpeedStep (clamped at MaxSpeed).
    /// Call this from your input script on X‑press.
    /// </summary>
    public void SpeedUp()
    {
        Speed = Mathf.Min(Speed + SpeedStep, MaxSpeed);
        updateSpeed();
        Debug.Log($"Obstacle Speed → {Speed}");
    }

    /// <summary>
    /// Decrease obstacle speed by SpeedStep (clamped at MinSpeed).
    /// Call this from your input script on Z‑press.
    /// </summary>
    public void SpeedDown()
    {
        Speed = Mathf.Max(Speed - SpeedStep, MinSpeed);
        updateSpeed();
        Debug.Log($"Obstacle Speed → {Speed}");
    }

    /// <summary>
    /// Applies the new Speed to all movers and recalculates spawn startPos.
    /// </summary>
    public void updateSpeed()
    {
        Mover.Speed = Speed;
        //UpdateSpawn();
    }

    /*private void UpdateSpawn()
    {
        // If you later want to flip spawn direction based on sign, this handles it
        float x = Speed >= 0 ? 30f : -30f;
        startPos = new Vector3(x, 0f, 0f);
    }*/

    private void FillPool()
    {
        Obstacles = new Queue<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject clone = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
            clone.SetActive(false);
            Obstacles.Enqueue(clone);
        }
    }

    private GameObject GetObstacle()
    {
        GameObject clone = Obstacles.Dequeue();
        clone.transform.position = startPos;
        updateSpeed();
        return clone;
    }

    private void updateTopTransform()
    {
        top.transform.localScale = topScale;
        top.transform.position = new Vector3(
            top.transform.position.x,
            5f - top.transform.localScale.y / 2f,
            0f
        );
    }

    private void updateBottomTransform()
    {
        bottom.transform.localScale = bottomScale;
        bottom.transform.position = new Vector3(
            bottom.transform.position.x,
            -5f + bottom.transform.localScale.y / 2f,
            0f
        );
    }

    private IEnumerator topRandGen()
    {
        topWidth = WidthRange.x;

        while (true)
        {
            yield return new WaitUntil(() => currentMode == SpawnMode.Obstacles);

            top = GetObstacle();
            topHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateTopTransform();
            yield return new WaitForSeconds(topInterval);
            top.SetActive(true);
        }
    }

    private IEnumerator bottomRandGen()
    {
        bottomWidth = WidthRange.x;

        while (true)
        {
            yield return new WaitUntil(() => currentMode == SpawnMode.Obstacles);

            bottom = GetObstacle();
            bottomHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateBottomTransform();
            yield return new WaitForSeconds(bottomInterval);
            bottom.SetActive(true);
        }
    }

    private IEnumerator SpawnMimicBlocks(float interval)
    {
        int numberOfBlocks = 5;
        float spacing = 1f;

        while (true)
        {
            if (currentMode == SpawnMode.Mimic && mimic != null && mimicBlockPrefab != null)
            {
                Vector3 mStart = mimic.position;
                for (int i = 0; i < numberOfBlocks; i++)
                {
                    Vector3 spawnPos = mStart + new Vector3(i * spacing, 0f, 0f);
                    GameObject block = Instantiate(mimicBlockPrefab, spawnPos, Quaternion.identity, ObstaclesContainer);
                    block.SetActive(true);
                }
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ModeSwitcher()
    {
        while (true)
        {
            currentMode = SpawnMode.Obstacles;
            Debug.Log("Mode: Obstacles (Top/Bottom blocks spawning)");
            yield return new WaitForSeconds(modeDuration);

            currentMode = SpawnMode.Mimic;
            Debug.Log("Mode: Mimic blocks spawning");
            yield return new WaitForSeconds(modeDuration);
        }
    }

    // (Keep any other coroutines like gen1(), gen2() unchanged below…)
}
