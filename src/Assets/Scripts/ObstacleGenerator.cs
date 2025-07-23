using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public static Queue<GameObject> Obstacles;
    public PlayerController PC;

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

    public Rigidbody2D rb;
    public GameController gc;

    public Transform mimic; // Reference to mimic GameObject
    public GameObject mimicBlockPrefab; // Mimic block prefab

    private Vector3 startPos;
    private GameObject top;
    private GameObject bottom;
    private float topHeight;
    private float topWidth;
    private float bottomHeight;
    private float bottomWidth;

    private float topInterval => (topWidth - Smooth / Mathf.Abs(Speed)) / Mathf.Abs(Speed);
    private float bottomInterval => (bottomWidth - Smooth / Mathf.Abs(Speed)) / Mathf.Abs(Speed);

    private Vector3 topScale => new Vector3(topWidth, topHeight, 1);
    private Vector3 bottomScale => new Vector3(bottomWidth, bottomHeight, 1);

    private enum SpawnMode { Obstacles, Mimic }
    private SpawnMode currentMode = SpawnMode.Obstacles;

    public float modeDuration = 3f; // 3 seconds per mode

    void Awake()
    {
        UpdateSpawn();
    }

    void Start()
    {
        StartCoroutine(topRandGen());
        StartCoroutine(bottomRandGen());
        StartCoroutine(SpawnMimicBlocks(2f));
        StartCoroutine(ModeSwitcher());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rb.AddForce(Vector2.up * Force);
        }
    }

    public void UpdateSpeed()
    {
        Mover.Speed = Speed;
        UpdateSpawn();
    }

    private void UpdateSpawn()
    {
        float x = Speed >= 0 ? 30f : -30f;
        startPos = new Vector3(x, 0f, 0f);                                          //New add
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
        top.transform.position = new Vector3(top.transform.position.x, 5 - top.transform.localScale.y / 2, 0f);
    }

    private void updateBottomTransform()
    {
        bottom.transform.localScale = bottomScale;
        bottom.transform.position = new Vector3(bottom.transform.position.x, -5 + bottom.transform.localScale.y / 2, 0f);
    }

    private IEnumerator topRandGen()
    {
        topWidth = WidthRange.x;
        float timer = 0f;

        while (true)
        {
            if (Speed == 0f) //Stops obstacles from generating if Speed = 0.
            {
                yield return null;
                continue;
            }

            if (timer <= 0f)
            {
                // Wait for Obstacle mode
                yield return new WaitUntil(() => currentMode == SpawnMode.Obstacles);

                top = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
                topHeight = Random.Range(HeightRange.x, HeightRange.y);
                updateTopTransform();
                top.SetActive(true);
            }

            timer -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator bottomRandGen()
    {
        bottomWidth = WidthRange.x;

        while (true)
        {
            if (Speed == 0f) //Stops obstacles from generating if Speed = 0.
            {
                yield return null;
                continue;
            }

            // Wait for Obstacle mode
            yield return new WaitUntil(() => currentMode == SpawnMode.Obstacles);

            bottom = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
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
            // Only spawn mimic blocks during Mimic mode
            if (currentMode == SpawnMode.Mimic && mimic != null && mimicBlockPrefab != null)
            {
                Vector3 startPos = mimic.position;

                for (int i = 0; i < numberOfBlocks; i++)
                {
                    Vector3 spawnPos = startPos + new Vector3(i * spacing, 0f, 0f);
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
            // Set to Obstacles mode
            currentMode = SpawnMode.Obstacles;
            Debug.Log("Mode: Obstacles (Top/Bottom blocks spawning)");
            yield return new WaitForSeconds(modeDuration);

            // Set to Mimic mode
            currentMode = SpawnMode.Mimic;
            Debug.Log("Mode: Mimic blocks spawning");
            yield return new WaitForSeconds(modeDuration);
        }
    }


    private IEnumerator generator()
    {
        Speed = 9;
        HeightRange = new Vector2(0.5f, 2);
        WidthRange = new Vector2(3, 3);
        D = 1;
        Smooth = 2;
        Delta = 0.5f;

        while (true)
        {
            WidthRange = new Vector2(3, 3);
            Delta = 0.5f;

            yield return StartCoroutine(gen1());
            yield return StartCoroutine(gen2());

            if (HeightRange.y < 4f)
            {
                HeightRange.x += 0.5f;
                HeightRange.y += 0.5f;
            }

            if (Speed < 15)
            {
                Speed += 1f;
                Smooth += 0.5f;
            }

            if (D < 7)
            {
                D++;
            }
        }
    }

    private IEnumerator gen1()
    {
        float height = HeightRange.x;
        float width = WidthRange.x;
        bool h = true;
        int T = (int)((HeightRange.y - HeightRange.x) / Delta) * 2;
        int t = 0;

        while (t < D * T)
        {
            if (h)
                height = Mathf.MoveTowards(height, HeightRange.y, Delta);
            else
                height = Mathf.MoveTowards(height, HeightRange.x, Delta);

            if (height <= HeightRange.x)
                h = true;
            else if (height >= HeightRange.y)
                h = false;

            width = Random.Range(WidthRange.x, WidthRange.y);

            topHeight = bottomHeight = height;
            topWidth = bottomWidth = width;
            updateTopTransform();
            updateBottomTransform();

            yield return new WaitForSeconds(topInterval);
            t++;

            top.SetActive(true);
            bottom.SetActive(true);
        }
    }

    private IEnumerator gen2()
    {
        topHeight = HeightRange.x;
        bottomHeight = HeightRange.y;

        float width = WidthRange.x;
        bool h = true;
        int T = (int)((HeightRange.y - HeightRange.x) / Delta) * 2;
        int t = 0;

        while (t < D * T)
        {
            if (h)
            {
                topHeight = Mathf.MoveTowards(topHeight, HeightRange.y, Delta);
                bottomHeight = Mathf.MoveTowards(bottomHeight, HeightRange.x, Delta);
            }
            else
            {
                topHeight = Mathf.MoveTowards(topHeight, HeightRange.x, Delta);
                bottomHeight = Mathf.MoveTowards(bottomHeight, HeightRange.y, Delta);
            }

            if (topHeight <= HeightRange.x)
            {
                h = true;
            }
            else if (topHeight >= HeightRange.y)
            {
                h = false;
            }

            width = Random.Range(WidthRange.x, WidthRange.y);

            topWidth = bottomWidth = width;
            updateTopTransform();
            updateBottomTransform();

            yield return new WaitForSeconds(topInterval);
            t++;

            top.SetActive(true);
            bottom.SetActive(true);
        }
    }
}
    // (Keep your other coroutines like gen1(), gen2() if you want, unchanged)


