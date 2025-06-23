using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;
    public GameObject player;
    //Set respawn position
    Vector2 startPos;

    private Rigidbody2D rb;
    private GameController gc;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        //Get start position of player
        startPos = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) //Add Space function
        {
            Flap();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            //Activate Respawn and life counter goes down 1
            Respawn();
            gc.LoseLife();
            //gc.GameOver();
        }
    }

    public void Flap()
    {
        //rb.AddForce(Vector2.up * Force);  //Original jump/flap formula
        rb.linearVelocity = Vector2.up * Force; //Should be more responsive than before
    }

    //Once player hits boundary the player is moved to the center of the screen
    void Respawn()
    {
        transform.position = startPos;
    }
}
