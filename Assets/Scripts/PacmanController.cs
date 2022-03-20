using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PacmanController : PlayerController

{
    //adding life counter 
    public int life;
    public TextMeshProUGUI lifeText;

// Start is called before the first frame update
void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
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
            for(int i = 0; i < allContactPoints.Count; i++)
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
            pelletMap.SetTile(pelletPosition, null);

          
        }

    }
   
    private void UpdateLife(int lifetoAdd)
    {
        life += lifetoAdd;
        lifeText.text = "Life" + life;
    }


}
