using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GhostController : PlayerController
{
    public float secondsUntilActive = 3;
    public TextMeshProUGUI gameOverText;
    public Image LifeOne;
    public Image LifeTwo;
    public Image LifeThree;
 
   

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        
        canMove = false;
        StartCoroutine(MovementCooldown());
        gameOverText.gameObject.SetActive(false);
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
      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the ghost collides with a pellet, ignore the collision
        if (collision.collider.CompareTag("Pellet"))
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
      
        //working on destroying pacman if ghost collides and updating lifecounter
        if (collision.gameObject.tag == "Pacman")
        {
            Destroy(collision.gameObject);
            Destroy(LifeOne.gameObject);
            
            //gameOverText.gameObject.SetActive(true);
        }

    }






}
