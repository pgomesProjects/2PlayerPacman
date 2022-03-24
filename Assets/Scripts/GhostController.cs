using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GhostController : PlayerController
{
    public float secondsUntilActive = 3;
    public TextMeshProUGUI gameOverText;

    public Image[] allLiveSprites;
    private Vector3 warp;
    private Vector3 teleport;
    public float powerPelletTimer = 10;
    private float currentPelletTimer;

    private Color ghostColor, currentColor;
    private Color vulnerableColor = new Color(0.13f, 0.13f, 1);

    private bool isVulnerable;

    // Start is called before the first frame update
    void Start()
    {
        playerType = GameManager.instance.currentPlayerSetup[1];
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        isVulnerable = false;
        currentPelletTimer = powerPelletTimer;
        ghostColor = GetComponent<SpriteRenderer>().color;
        currentColor = ghostColor;
        canMove = false;
        rotDegree = transform.rotation.z;
        canRotateSprite = false;
        StartCoroutine(MovementCooldown());
       
        
        gameOverText.gameObject.SetActive(false);
        //settng warp/teleport cords
        warp = new Vector3(15, 7.5f, 0);
        teleport = new Vector3(-15, 7.5f, 0);

    }

    IEnumerator MovementCooldown()
    {
        //Wait a set amount of seconds before the start of the game before the ghost can move
        yield return new WaitForSeconds(secondsUntilActive);
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
        //Teleporting to other warp if you go too far left or right 
        if (transform.position.x > warp.x)
        {
            transform.position = teleport;
        }

        if (transform.position.x < teleport.x)
        {
            transform.position = warp;
        }
    }

    public void MakeVulnerable()
    {
        if (isVulnerable)
        {
            currentPelletTimer = powerPelletTimer;
        }
        else
        {
            isVulnerable = true;
            currentColor = vulnerableColor;
            GetComponent<SpriteRenderer>().color = currentColor;
            StartCoroutine(VulnerableTimer());
        }
    }

    IEnumerator VulnerableTimer()
    {
        currentPelletTimer = powerPelletTimer;

        while(currentPelletTimer > 0)
        {
            currentPelletTimer -= Time.deltaTime;
            yield return null;
        }

        isVulnerable = false;
        currentColor = ghostColor;
        GetComponent<SpriteRenderer>().color = currentColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the ghost collides with a pellet, ignore the collision
        if (collision.collider.CompareTag("Pellet"))
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);

        //Destroying Pacman
        if (collision.gameObject.tag == "Pacman" && !isVulnerable)
        {
            PacmanController pacmanController = collision.gameObject.GetComponent<PacmanController>();
            Debug.Log("Pacman Lives: " + (pacmanController.life - 1));
            pacmanController.life--;
            //If the player has no lives, end the game
            if (pacmanController.life == -1)
            {
                FindObjectOfType<AudioManager>().Stop("InGameMusic");
                gameOverText.gameObject.SetActive(true);
                canMove = false;
                pacmanController.gameObject.SetActive(false);
                GameManager.instance.EndGame("Titlescreen", 3);
            }
            //Else, take a life from the player and move both players to their spawn points
            else
            {
                allLiveSprites[pacmanController.life].gameObject.SetActive(false);
                pacmanController.transform.position = pacmanController.spawnPoint.transform.position;
                transform.position = spawnPoint.transform.position;
            }
        }
        else if(collision.gameObject.tag == "Pacman" && isVulnerable)
        {
            StopCoroutine(VulnerableTimer());
            currentColor = ghostColor;
            GetComponent<SpriteRenderer>().color = currentColor;
            transform.position = spawnPoint.transform.position;
        }
        
    }

    protected override void OnRotation()
    {
        //TODO: change the eye sprites
    }

}
