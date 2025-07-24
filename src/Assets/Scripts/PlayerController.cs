using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;
    public GameObject player;
    bool gravityFlip = false;
    Vector2 startPos;   //Set respawn position
    bool waitingForInput = false;   //Bool for waiting after death
    public ObstacleGenerator obstacleMods;

    private Rigidbody2D rb;
    private GameController gc;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        startPos = transform.position;  //Get start position of player
    }

    void Update()
    {
        if (waitingForInput)    //If true then game is paused till they press space or leftmouse button
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) //Add Space function
            {
                waitingForInput = false;
                Time.timeScale = 1; //Unpause game
                Flap();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) //Flip gravity by pressing the "Shift" key
            {
                Flip();
            }
            if (Input.GetKeyDown(KeyCode.Z)) //Makes the player move left if they press "Z"
            {
                obstacleMods.Speed -= 2.5f;
                obstacleMods.updateSpeed();
            }

            if (Input.GetKeyDown(KeyCode.X)) //Makes the player move right if they press "X"
            {
                obstacleMods.Speed += 2.5f;
                obstacleMods.updateSpeed();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) //Add Space function
            {
                Flap();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) //Flip gravity by pressing the "Shift" key
            {
                Flip();
            }
            if (Input.GetKeyDown(KeyCode.Z)) //Makes the player move left if they press "Z"
            {
                obstacleMods.Speed -= 2.5f;
                obstacleMods.updateSpeed();
            }

            if (Input.GetKeyDown(KeyCode.X)) //Makes the player move right if they press "X"
            {
                obstacleMods.Speed += 2.5f;
                obstacleMods.updateSpeed();
            }
        }

    }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            //Activate Respawn and life counter goes down 1
            Respawn();
            gc.LoseLife();
            
        }
    }

    public void Flap()
    {
        float direction = gravityFlip ? -1f : 1f;
        //rb.AddForce(Vector2.up * Force);  //Original jump/flap formula
        //rb.linearVelocity = Vector2.up * Force; //Should be more responsive than before
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Force * direction);    //New formula to add direction
    }
    /*public void Flap()
    {
        float direction = gravityFlip ? -1f : 1f;
        rb.AddForce(Vector2.up * Force * direction);
    }*/

    public void Flip()
    {

        gravityFlip = !gravityFlip;
        rb.gravityScale = rb.gravityScale * -1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
        
        Vector3 s = transform.localScale;
        s.y *= -1f;
        transform.localScale = s;
    }
    //Once player hits boundary the player is moved to the center of the screen
    /*void Respawn()
    {
        transform.position = startPos;
        Time.timeScale = 0; //Pause game after respawn
        waitingForInput = true;
    }*/

    void Respawn()
    {
        Vector2 foundPosition = FindSafeSpot(startPos, 2.5f, 0.5f);
        transform.position = foundPosition;
        Time.timeScale = 0; //Pause game after respawn
        waitingForInput = true;
    }

    Vector2 FindSafeSpot(Vector2 center, float radius, float buffer)
    {
        int maxAttempts = 100;
        //int attempts = 0;

        /*while (attempts < maxAttempts)
        {
            //Choose a random point in a circle around the center
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector2 candidatePos = center + randomOffset;

            //Check for obstacles
            Collider2D hit = Physics2D.OverlapCircle(candidatePos, checkRadius, LayerMask.GetMask("Obstacle"));
            if (hit == null)
            {
                //No obstacle detected
                return candidatePos;
            }

            attempts++;
        }
        */

        for(int i = 0; i < maxAttempts; i++)
        {
            //Choose a random point in a circle around the center
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector2 candidatePos = center + randomOffset;

            if (IsSafeSpot(candidatePos, buffer))
                return candidatePos;
        }

        //Fallback to original start position if nothing found
        return center;
    }

    bool IsSafeSpot(Vector2 position, float buffer)
    {
        LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");

        //check above
        if(Physics2D.OverlapCircle(position + Vector2.up * buffer, buffer * 0.5f, obstacleLayer))
            return false;

        //check left
        if(Physics2D.OverlapCircle(position + Vector2.left * buffer, buffer * 0.5f, obstacleLayer))
            return false;

        //check right
        if(Physics2D.OverlapCircle(position + Vector2.right * buffer, buffer * 0.5f, obstacleLayer))
            return false;

        //check left and up (diagnol)
        Vector2 leftup = (Vector2.up + Vector2.left).normalized;
        if (Physics2D.OverlapCircle(position + leftup * buffer, buffer * 0.5f, obstacleLayer))
            return false;

        //check right and up (diagnol)
        Vector2 rightup = (Vector2.up + Vector2.right).normalized;
        if (Physics2D.OverlapCircle(position + rightup * buffer, buffer * 0.5f, obstacleLayer))
            return false;

        //check current spot
        if (Physics2D.OverlapCircle(position, buffer * 0.5f, obstacleLayer))
            return false;

        return true;
    }
}
