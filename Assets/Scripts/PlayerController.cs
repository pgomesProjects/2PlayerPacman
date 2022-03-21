using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class PlayerController : MonoBehaviour
{
    public enum Player { PLAYERONE, PLAYERTWO };
    public Player playerType;
    public float speed = 3f;
    private float horizontal, vertical;
    private Vector2 direction = new Vector2(1, 0);
    protected bool canMove;
    protected BoxCollider2D playerCollider;
    protected bool canMoveUp, canMoveDown, canMoveLeft, canMoveRight;
    protected Rigidbody2D rb2D;
    [SerializeField] private LayerMask levelMask;

    protected void GetPlayerInput()
    {
        //Get input based on which player is who
        switch (playerType)
        {
            case Player.PLAYERONE:
                horizontal = Input.GetAxisRaw("PlayerOneHorizontal");
                vertical = Input.GetAxisRaw("PlayerOneVertical");
                break;
            case Player.PLAYERTWO:
                horizontal = Input.GetAxisRaw("PlayerTwoHorizontal");
                vertical = Input.GetAxisRaw("PlayerTwoVertical");
                break;
        }
    }

    private void FixedUpdate()
    {
        Vector2 boxPosUp = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 boxPosDown = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Vector2 boxPosLeft = new Vector2(transform.position.x - 0.5f, transform.position.y);
        Vector2 boxPosRight = new Vector2(transform.position.x + 0.5f, transform.position.y);
        Vector3 scale = playerCollider.bounds.size * 0.85f;
        canMoveUp = !Physics2D.BoxCast(boxPosUp, scale, 0, Vector2.up, 1f, levelMask);
        canMoveDown = !Physics2D.BoxCast(boxPosDown, scale, 0, Vector2.down, 1f, levelMask);
        canMoveLeft = !Physics2D.BoxCast(boxPosLeft, scale, 0, Vector2.left, 1f, levelMask);
        canMoveRight = !Physics2D.BoxCast(boxPosRight, scale, 0, Vector2.right, 1f, levelMask);

        //Move the player accordingly based on input
        if (horizontal != 0 && horizontal != direction.x)
        {
            if ((horizontal > 0 && canMoveRight) || (horizontal < 0 && canMoveLeft))
            {
                direction.x = horizontal;
                direction.y = 0;
            }
        }

        if (vertical != 0 && vertical != direction.y)
        {
            if ((vertical > 0 && canMoveUp) || (vertical < 0 && canMoveDown))
            {
                direction.x = 0;
                direction.y = vertical;
            }
        }

        if (canMove)
        {
            Vector2 position = transform.position;
            position.x = position.x + direction.x * speed * Time.deltaTime;
            position.y = position.y + direction.y * speed * Time.deltaTime;
            transform.position = position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 boxPosUp = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 boxPosDown = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Vector2 boxPosLeft = new Vector2(transform.position.x - 0.5f, transform.position.y);
        Vector2 boxPosRight = new Vector2(transform.position.x + 0.5f, transform.position.y);
        Vector2 scale = GetComponent<BoxCollider2D>().size * 0.85f;
        Gizmos.DrawWireCube(boxPosUp, scale);
        Gizmos.DrawWireCube(boxPosDown, scale);
        Gizmos.DrawWireCube(boxPosLeft, scale);
        Gizmos.DrawWireCube(boxPosRight, scale);
    }
#endif

  
}
