using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;

    bool gravityFlip = false;
    private Rigidbody2D rb;
    private GameController gc;

    private ObstacleGenerator obstacleMods; //Allows for controlling of the obstacles

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        obstacleMods = FindObjectOfType<ObstacleGenerator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //Add Space function
        {
            Flap();
        }

        if (Input.GetKeyDown(KeyCode.Z)) //Makes the player move left if they press "Z"
        {
            obstacleMods.Speed -= 2.5f;
            obstacleMods.UpdateSpeed();
        }

        if (Input.GetKeyDown(KeyCode.X)) //Makes the player move right if they press "X"
        {
            obstacleMods.Speed += 2.5f;
            obstacleMods.UpdateSpeed();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) //Flip gravity by pressing the "Shift" key
        {
            Flip();
        }
    }

    void FixedUpdate() //
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gc.GameOver();
        }
    }

    public void Flap()
    {
        float direction = gravityFlip ? -1f : 1f;
        rb.AddForce(Vector2.up * Force * direction);
    }

    public void Flip()
    {
        gravityFlip = !gravityFlip;
        rb.gravityScale = gravityFlip ? -1f : 1f;
    }
}
//hehe
