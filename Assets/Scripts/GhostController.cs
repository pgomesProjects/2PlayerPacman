using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostController : PlayerController
{
    public float secondsUntilActive = 0;

    public float powerPelletTimer = 10;
    public Sprite[] eyeSprites;
    private float currentPelletTimer;

    private Color ghostColor, currentColor;
    private Color vulnerableColor = new Color(0.13f, 0.13f, 1);

    private bool isVulnerable;
    private bool isRegularVulnerableSprite;
    private SpriteRenderer currentEyes;

    [HideInInspector] public bool deathAniComplete = false;
    [HideInInspector] public bool restartAniComplete = false;

    private float normalSpeed;
    private float vulnerableMultiplier = 0.85f;

    // Start is called before the first frame update
    void Start()
    {
        playerType = GameManager.instance.currentPlayerSetup[1];
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        currentEyes = transform.Find("Eyes").GetComponent<SpriteRenderer>();

        isVulnerable = false;
        currentPelletTimer = powerPelletTimer;
        ghostColor = GameManager.instance.ghostColor;

        secondsUntilStart = LevelManager.Level.secondsUntilStart + secondsUntilActive;
        currentColor = ghostColor;
        canRotateSprite = false;

        normalSpeed = speed;

        direction = new Vector2(0, 1);

        canMove = false;
        rotDegree = transform.rotation.z;
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
            currentColor = vulnerableColor;
            currentEyes.color = new Color(1, 1, 1);
            GetComponent<SpriteRenderer>().color = currentColor;
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("PowerPelletSFX", GameManager.sfxVolume * 0.55f);
            isVulnerable = true;
            currentColor = vulnerableColor;
            currentEyes.sprite = eyeSprites[4];
            GetComponent<SpriteRenderer>().color = currentColor;
            StartCoroutine(VulnerableTimer());
            speed = speed * vulnerableMultiplier;
        }
    }

    IEnumerator VulnerableTimer()
    {
        currentPelletTimer = powerPelletTimer;
        float blinkingTimer = 0.5f;
        float currentBlinkingTimer = 0;
        isRegularVulnerableSprite = true;

        while (currentPelletTimer > 0 && !GameManager.instance.isGameAnimationActive)
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
        FindObjectOfType<AudioManager>().Stop("PowerPelletSFX");
        currentColor = ghostColor;
        currentEyes.color = new Color(1, 1, 1);
        GetComponent<SpriteRenderer>().color = currentColor;
        speed = normalSpeed;
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
            StartCoroutine(LevelManager.Level.DeathAnimation());
        }
        else if(collision.gameObject.tag == "Pacman" && isVulnerable)
        {
            currentPelletTimer = 0;
            FindObjectOfType<AudioManager>().Play("GhostEat", GameManager.sfxVolume);
            StopCoroutine(VulnerableTimer());
            currentColor = ghostColor;
            currentEyes.color = new Color(1, 1, 1);
            GetComponent<SpriteRenderer>().color = currentColor;
            isVulnerable = false;
            OnRotation();
            FindObjectOfType<AudioManager>().Stop("PowerPelletSFX");
            speed = normalSpeed;
            transform.position = spawnPoint.transform.position;
        }
        
    }

    public void ChangeEyes(int eyeNum)
    {
        currentEyes.sprite = eyeSprites[eyeNum];
    }

    public override void OnRotation()
    {
        if (!isVulnerable)
        {
            switch (axis)
            {
                case 0:
                    //Right
                    if (direction.x > 0)
                    {
                        ChangeEyes(0);
                    }
                    //Left
                    else
                    {
                        ChangeEyes(1);
                    }
                    break;
                case 1:
                    //Down
                    if (direction.y > 0)
                    {
                        ChangeEyes(2);
                    }
                    //Up
                    else
                    {
                        ChangeEyes(3);
                    }
                    break;
            }
        }
    }

}
