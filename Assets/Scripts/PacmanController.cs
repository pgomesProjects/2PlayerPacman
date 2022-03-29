using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PacmanController : PlayerController

{
    //adding life counter 
    public int life;
    private Vector3 oldPosition;
    private Vector3 previousDifferences;
    private int notMovingFrames;
    private float currentWakaTimer;
    private float wakaTimer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        playerType = GameManager.instance.currentPlayerSetup[0];
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        rotDegree = transform.rotation.z;
        canRotateSprite = false;
        secondsUntilStart = LevelManager.Level.secondsUntilStart;
        canMove = false;
        oldPosition = transform.position;
        StartCoroutine(MovementCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
        if (canMove)
        {
            if (previousDifferences.x == Mathf.Abs(oldPosition.x - transform.position.x) 
                && previousDifferences.y == Mathf.Abs(oldPosition.y - transform.position.y))
            {
                notMovingFrames++;
                if(notMovingFrames > 5)
                {
                    GetComponent<Animator>().SetFloat("Speed", 0);
                    GetComponent<Animator>().SetBool("IsIdle", true);
                }
            }
            else
            {
                notMovingFrames = 0;
                GetComponent<Animator>().SetBool("IsIdle", false);
                GetComponent<Animator>().SetFloat("Speed", 1);
            }
            previousDifferences.x = Mathf.Abs(oldPosition.x - transform.position.x);
            previousDifferences.y = Mathf.Abs(oldPosition.y - transform.position.y);
            previousDifferences.z = Mathf.Abs(oldPosition.z - transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the player collides with a pellet
        if (collision.collider.CompareTag("Pellet"))
        {
            //Figure out where the player collided
            List<ContactPoint2D> allContactPoints = new List<ContactPoint2D>();
            collision.GetContacts(allContactPoints);
            Tilemap pelletMap = collision.collider.GetComponent<Tilemap>();
            Grid layoutGrid = pelletMap.layoutGrid;
            ContactPoint2D pacManColliderPoint = new ContactPoint2D();
            for (int i = 0; i < allContactPoints.Count; i++)
            {
                if (allContactPoints[i].collider != null)
                {
                    if (allContactPoints[i].otherCollider.CompareTag("Pacman"))
                    {
                        pacManColliderPoint = allContactPoints[i];
                        break;
                    }
                }
                else
                    continue;
            }
            //Find the position on the map of the collided pellet and destroy it
            Vector3Int pelletPosition = layoutGrid.WorldToCell(pacManColliderPoint.point);

            //If Pacman collects a power pellet, make the ghost vulnerable
            if (pelletMap.GetTile(pelletPosition).name == "power_pellet")
            {
                Debug.Log("Power Pellet Collected!");
                GhostController ghostPlayer = FindObjectOfType<GhostController>();
                ghostPlayer.MakeVulnerable();
            }

            pelletMap.SetTile(pelletPosition, null);
            LevelManager.Level.RemovePellet();

            //If the waka sound is playing, reset the waka timer
            if(currentWakaTimer > 0)
            {
                currentWakaTimer = wakaTimer;
            }
            //If the waka sound is not playing, start playing it
            else
            {
                FindObjectOfType<AudioManager>().Play("Waka", GameManager.sfxVolume);
                StartCoroutine(WakaTimer());
            }

            //If there are no more pellets, game is over and return to the main menu
            if (LevelManager.Level.GetTotalPellets() == 0)
                LevelManager.Level.EndAnimation();

        }
    }

    IEnumerator WakaTimer()
    {
        currentWakaTimer = wakaTimer;
        FindObjectOfType<AudioManager>().Play("Waka", GameManager.sfxVolume);

        while (currentWakaTimer > 0 && !GameManager.instance.isGameAnimationActive)
        {
            currentWakaTimer -= Time.deltaTime;
            yield return null;
        }
        FindObjectOfType<AudioManager>().Stop("Waka");
    }

    public override void OnRotation()
    {
        switch (axis)
        {
            case 0:
                FlipSprite(axis, (int)direction.x);
                break;
            case 1:
                FlipSprite(axis, (int)direction.y);
                break;
        }
    }

    public void FlipSprite(int axis, int direction)
    {
        //Rotate back to zero degrees
        gameObject.transform.Rotate(0.0f, 0.0f, -rotDegree, Space.World);
        switch (axis)
        {
            case 0:
                rotDegree = 90 - (direction * 90);
                break;
            case 1:
                rotDegree = direction * 90;
                break;
        }
        //Rotate object based on the direction that they are moving
        gameObject.transform.Rotate(0.0f, 0.0f, rotDegree, Space.World);

        //Flip sprite for left rotation
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, -1, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }
    }//end of FlipSprite

}
