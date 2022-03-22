using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GhostController : PlayerController
{
    public float secondsUntilActive = 3;
    public TextMeshProUGUI gameOverText;

    public Image[] allliveSprites;
    public Image lifeTwo;
    public Image lifeThree;
    public GameObject Pacman;
    public GameObject Pacman1;
    public GameObject Pacman2;
    private Vector3 warp;
    private Vector3 teleport;

    // Start is called before the first frame update
    void Start()
    {
        playerType = GameManager.instance.currentPlayerSetup[1];
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        canMove = false;
        rotDegree = transform.rotation.z;
        canRotateSprite = false;
        StartCoroutine(MovementCooldown());
       
        
        gameOverText.gameObject.SetActive(false);
      //settng warp/teleport cords
        warp = new Vector3(13, 7, 0);
        teleport = new Vector3(-13, 7, 0);
        //setting other Pacmen false until called upon after death of orginal
        Pacman1.gameObject.SetActive(false);
        Pacman2.gameObject.SetActive(false);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the ghost collides with a pellet, ignore the collision
        if (collision.collider.CompareTag("Pellet"))
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);

        //Destroying Pacman Starting Pacman1
        if (collision.gameObject.tag == "Pacman")
        {
            Destroy(collision.gameObject);
            allliveSprites[0].gameObject.SetActive(false);

            Instantiate(Pacman1, new Vector3(0, -1, 0), Quaternion.identity);
            Pacman1.gameObject.SetActive(true);
        }
       //Destroying Pacman1 Starting Pacman2
        if (collision.gameObject.tag == "Pacman1")
        {
            Destroy(collision.gameObject);
           lifeTwo.gameObject.SetActive(false);
                      
            Instantiate(Pacman2, new Vector3(0, -1, 0), Quaternion.identity);
            Pacman2.gameObject.SetActive(true);
        }
        //Destorying Pacman2 and End Game Text
        if (collision.gameObject.tag == "Pacman2")
        {
            Destroy(collision.gameObject);
            lifeThree.gameObject.SetActive(false);

            gameOverText.gameObject.SetActive(true);
            canMove = false;
        }
        
    }

    protected override void OnRotation()
    {
        //TODO: change the eye sprites
    }

}
