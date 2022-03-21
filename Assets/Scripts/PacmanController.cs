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
        rotDegree = transform.rotation.z;
        canRotateSprite = true;
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

            LevelManager.Level.RemovePellet();
            //If there are no more pellets, game is over and return to the main menu
            if (LevelManager.Level.GetTotalPellets() == 0)
                LevelManager.Level.EndAnimation();

        }

    }

    protected override void OnRotation()
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

    private void FlipSprite(int axis, int direction)
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
    }//end of FlipSprite
   
    private void UpdateLife(int lifetoAdd)
    {
        life += lifetoAdd;
        lifeText.text = "Life" + life;
    }
}
