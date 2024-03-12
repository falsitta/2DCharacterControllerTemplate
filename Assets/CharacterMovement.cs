using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Right, Left }

public class CharacterMovement : MonoBehaviour
{

    Rigidbody2D rigidbody;
    public float accelerationRate = 10;
    public float decelerationRate = 15;
    public float maxSpeed = 20;
    public float jumpForce = 15;

    public bool isGrounded;
    public bool canJump;
    public int jumpCount = 0;
    public int extraJumps = 3;
    //coyote time
    public float currentCoyoteTime = 0;
    public float coyoteTime = 0.5f;

    public float defaultGravityScale;

    Queue<Move> moveQueue = new Queue<Move>();
    double elapsedMoveTime = 0;

    SpriteRenderer spriteRenderer;
    public FacingDirection direction;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        defaultGravityScale = rigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        currentCoyoteTime -= Time.deltaTime;

        if (Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y), 0.1f, Vector2.down, 0, LayerMask.GetMask("Ground")))
        {
            currentCoyoteTime = coyoteTime;

            if (canJump)
            {
                jumpCount = 0;
            }
        }
        else
        {
            canJump = true;
        }


        if (currentCoyoteTime > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;

        }

        if (Input.GetKey(KeyCode.A))
        {
            if (rigidbody.velocity.x > 0)
            {
                //quick turn
                rigidbody.AddForce(Vector2.left * accelerationRate * 5 * Time.deltaTime, ForceMode2D.Impulse);
            }
            // go left
            rigidbody.AddForce(Vector2.left * accelerationRate * Time.deltaTime, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (rigidbody.velocity.x < 0)
            {
                //quick turn
                rigidbody.AddForce(Vector2.right * accelerationRate * 5 * Time.deltaTime, ForceMode2D.Impulse);
            }

            // go right
            rigidbody.AddForce(Vector2.right * accelerationRate * Time.deltaTime, ForceMode2D.Impulse);
        }
        else if (rigidbody.velocity.x != 0)
        {
            rigidbody.AddForce(new Vector2(-rigidbody.velocity.x, 0) * decelerationRate * Time.deltaTime, ForceMode2D.Impulse);
        }

        //buffer timeout
        if (moveQueue.Count > 0)
        {
            elapsedMoveTime += Time.deltaTime;
            Move current = moveQueue.Peek();
            if (elapsedMoveTime > current.Duration)
            {
                elapsedMoveTime -= current.Duration;
                moveQueue.Dequeue();
            }
        }
        else
        {
            elapsedMoveTime = 0;
        }

        //queue up a jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveQueue.Enqueue(new Move(MoveType.Jump));
        }

        //buffer execution
        if (moveQueue.Count > 0)
        {
            Move current = moveQueue.Peek();

            if (current.MoveType == MoveType.Jump && (isGrounded || canJump) && jumpCount < extraJumps)
            {
                jumpCount++;
                if (jumpCount > extraJumps)
                {
                    canJump = false;

                }
                rigidbody.gravityScale = 4;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                moveQueue.Dequeue();
            }

        }

        //sprite Direction
        if (rigidbody.velocity.x > 0.01f)
        {
            direction = FacingDirection.Right;
        }
        else if (rigidbody.velocity.x < -0.01f)
        {
            direction = FacingDirection.Left;
        }
        SetSpriteDirection();


        if (Input.GetKeyUp(KeyCode.Space))
        {
            rigidbody.gravityScale = defaultGravityScale;

        }

        //rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxSpeed);
        rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -maxSpeed, maxSpeed), rigidbody.velocity.y);
    }

    void SetSpriteDirection()
    {
        switch (direction)
        {
            case FacingDirection.Left:
                {
                    spriteRenderer.flipX = true;
                    break;
                }
            case FacingDirection.Right:
                {
                    spriteRenderer.flipX = false;
                    break;
                }

        
        }
    } 
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.gameObject.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    public enum MoveType
    {
        Fireball, Jump, Block, Punch, Dash
    }

    public class Move
    {
        public MoveType MoveType { get; private set; }
        public double Duration { get; private set; }
        //...

        public Move(MoveType moveType)
        {
            this.MoveType = moveType;
            switch (moveType)
            {
                case MoveType.Fireball:
                    this.Duration = 1.0;
                    break;
                case MoveType.Block:
                    this.Duration = 0.5;
                    break;
                case MoveType.Jump:
                    this.Duration = 0.1;
                    break;
            }
        }
    }

}
