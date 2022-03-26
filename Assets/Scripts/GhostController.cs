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
    public float powerPelletTimer = 10;
    public Sprite[] eyeSprites;
    private float currentPelletTimer;

    private Color ghostColor, currentColor;
    private Color vulnerableColor = new Color(0.13f, 0.13f, 1);

    private bool isVulnerable;
    private bool isRegularVulnerableSprite;
    private SpriteRenderer currentEyes;

    // Start is called before the first frame update
    void Start()
    {
        playerType = GameManager.instance.currentPlayerSetup[1];
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        currentEyes = transform.Find("Eyes").GetComponent<SpriteRenderer>();

        isVulnerable = false;
        currentPelletTimer = powerPelletTimer;
        ghostColor = GetComponent<SpriteRenderer>().color;

        secondsUntilStart = LevelManager.Level.secondsUntilStart + secondsUntilActive;
        currentColor = ghostColor;
        
        canMove = false;
        rotDegree = transform.rotation.z;
        canRotateSprite = true;
        StartCoroutine(MovementCooldown());

    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
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
            currentEyes.sprite = eyeSprites[4];
            GetComponent<SpriteRenderer>().color = currentColor;
            StartCoroutine(VulnerableTimer());
        }
    }

    IEnumerator VulnerableTimer()
    {
        currentPelletTimer = powerPelletTimer;
        float blinkingTimer = 0.5f;
        float currentBlinkingTimer = 0;
        isRegularVulnerableSprite = true;

        while (currentPelletTimer > 0)
        {
            currentPelletTimer -= Time.deltaTime;
            if(currentPelletTimer < 3)
            {
                currentBlinkingTimer += Time.deltaTime;
                if(currentBlinkingTimer > blinkingTimer)
                {
                    //Toggle between blue and white sprite
                    if (isRegularVulnerableSprite)
                    {
                        isRegularVulnerableSprite = false;
                        currentColor = new Color(1, 1, 1);
                        currentEyes.color = new Color(1, 0, 0);
                    }
                    else
                    {
                        isRegularVulnerableSprite = true;
                        currentColor = vulnerableColor;
                        currentEyes.color = new Color(1, 1, 1);
                    }
                    GetComponent<SpriteRenderer>().color = currentColor;
                    currentBlinkingTimer = 0;
                }
            }
            yield return null;
        }

        isVulnerable = false;
        currentColor = ghostColor;
        currentEyes.color = new Color(1, 1, 1);
        GetComponent<SpriteRenderer>().color = currentColor;
        OnRotation();
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
                canRotateSprite = false;
                FindObjectOfType<AudioManager>().Stop("InGameMusic");
                GameManager.instance.isGameAnimationActive = true;
                gameOverText.gameObject.SetActive(true);
                canMove = false;
                pacmanController.gameObject.SetActive(false);
                foreach (var i in LevelManager.Level.pausePrompts)
                    i.SetActive(false);
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
            currentPelletTimer = 0;
            StopCoroutine(VulnerableTimer());
            currentColor = ghostColor;
            currentEyes.color = new Color(1, 1, 1);
            GetComponent<SpriteRenderer>().color = currentColor;
            isVulnerable = false;
            OnRotation();
            transform.position = spawnPoint.transform.position;
        }
        
    }

    protected override void OnRotation()
    {
        if (!isVulnerable)
        {
            switch (axis)
            {
                case 0:
                    //Right
                    if (direction.x > 0)
                    {
                        currentEyes.sprite = eyeSprites[0];
                    }
                    //Left
                    else
                    {
                        currentEyes.sprite = eyeSprites[1];
                    }
                    break;
                case 1:
                    //Down
                    if (direction.y > 0)
                    {
                        currentEyes.sprite = eyeSprites[2];
                    }
                    //Up
                    else
                    {
                        currentEyes.sprite = eyeSprites[3];
                    }
                    break;
            }
        }
    }

}
