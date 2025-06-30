using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;
    public float speed = 5f;

    bool gravityFlip = false;
    public float movement = 1f; //Sets movement to the right
    private Rigidbody2D rb;
    private GameController gc;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //Add Space function
        {
            Flap();
        }

        if (Input.GetKeyDown(KeyCode.Z)) //Makes the player move left if they press "Z"
        {
            movement = -1f;
        }
        else if (Input.GetKeyDown(KeyCode.X)) //Makes the player move right if they press "X"
        {
            movement = 1f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) //Flip gravity by pressing the "Shift" key
        {
            Flip();
        }
    }

    void FixedUpdate() //
    {
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);
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
        rb.AddForce(Vector2.up * Force);
    }

    public void Flip()
    {
        gravityFlip = !gravityFlip;
        rb.gravityScale = gravityFlip ? -1f : 1f;
    }
}
