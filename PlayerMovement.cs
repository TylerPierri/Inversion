using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviour
{
    private PlayerHealthSystem healthSystem;

    [SerializeField]
    private PlayerInputSystem controls;
    private playerGravityControl gravity;

    private SpriteRenderer render;
    private Animator playerAnim;

    [SerializeField]
    private playerMovementData moveData;

    private PhotonView view;

    public float horizontalMovement;
    private bool justJumped = false;
    private float currentJumpTime;

    float currentVelocity = 0.0f;
    float maxVelocity = 1.0f;

    private float IdleTimer;

    public bool onGround = false;
    private Rigidbody2D rb;

    [SerializeField] private FootStepSFX_Data footSFX_DATA;
    private AudioSource footSource;
    private float footTimer;
    private bool landed;

    [HideInInspector] public int floorSFXIndex = 0;

    private void Start()
    {
        view = GetComponent<PhotonView>();

        playerAnim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gravity = GetComponent<playerGravityControl>();
        healthSystem = GetComponent<PlayerHealthSystem>();

        SetCollisions();
    }
    private void Update()
    {
        render.flipX = playerAnim.GetBool("Flipped");
        if (!view.IsMine)
        {
            //this.enabled = false;
            return;
        }

        animatorFlip();

        horizontalMovement = controls.Direction.x;
    }

    private void animatorFlip()
    {
        //render.color = managerSystem.manager.teamColorArray[managerSystem.PlayerTeam];

        if (horizontalMovement < 0)
        {
            //render.flipX = true;
            playerAnim.SetBool("Flipped", true);
        }

        if (horizontalMovement > 0)
        {
            //render.flipX = false;
            playerAnim.SetBool("Flipped", false);
        }

        onGround = gravity.distanceFromFloor < 0.4f;
        //transform.localEulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0, 0, gravity.currentRotation), 2 * Time.deltaTime);

        if (justJumped)
        {
            //Debug.Log(currentJumpTime);
            if (currentJumpTime > 0)
                currentJumpTime -= Time.deltaTime;
            else
                playerEndJump();
        }
    }

    private void FixedUpdate()
    {
        if (!view.IsMine)
            return;

        if (healthSystem.dead)
        {
            rb.velocity = new Vector2(0, 0);
            return;
        }

        rb.velocity = MovementDirection() + gravity.gravityDirection[gravity.chosenDirection];

        //transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(transform.rotation.z, gravity.currentRotation, Time.deltaTime), Vector3.forward);
        Quaternion targetRotation = Quaternion.AngleAxis(gravity.currentRotation, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, gravity.rotationSpeed * Time.deltaTime);
    }

    private void WalkSFX(int Index, float speedSFX)
    {
        if (footSFX_DATA == null)
            return;

        if (footSource == null)
        {
            footSource = gameObject.AddComponent<AudioSource>();
            footSource.playOnAwake = false;
            footSource.priority = 5;
        }

        if (footTimer <= 0)
        {
            footTimer = speedSFX;
            footSource.clip = footSFX_DATA.FootSFX[Index].WalkSFX[Random.Range(0, footSFX_DATA.FootSFX[Index].WalkSFX.Length)];
            footSource.volume = footSFX_DATA.FootSFX[Index].WalkVolume;
            footSource.pitch = (Random.Range(1.1f, 0.9f));
            footSource.Play();
        }
        else
            footTimer -= Time.deltaTime;
    }

    private void RunSFX(int Index, float speedSFX)
    {
        if (footSFX_DATA == null)
            return;

        if (footSource == null)
        {
            footSource = gameObject.AddComponent<AudioSource>();
            footSource.playOnAwake = false;
            footSource.priority = 5;
        }

        if (footTimer <= 0)
        {
            footTimer = speedSFX;
            footSource.clip = footSFX_DATA.FootSFX[Index].RunSFX[Random.Range(0, footSFX_DATA.FootSFX[Index].RunSFX.Length)];
            footSource.volume = footSFX_DATA.FootSFX[Index].RunVolume;
            footSource.pitch = (Random.Range(1.1f, 0.9f));
            footSource.Play();
        }
        else
            footTimer -= Time.deltaTime;
    }

    private void LandingSFX(int Index)
    {
        if (footSFX_DATA == null)
            return;

        if (footSource == null)
        {
            footSource = gameObject.AddComponent<AudioSource>();
            footSource.playOnAwake = false;
            footSource.priority = 5;
        }
        footSource.clip = footSFX_DATA.FootSFX[Index].LandingSFX[Random.Range(0, footSFX_DATA.FootSFX[Index].LandingSFX.Length)];
        footSource.volume = footSFX_DATA.FootSFX[Index].LandingVolume;
        footSource.Play();
        landed = true;
    }
    public void FlipPlayer()
    {
        if (gravity.InFlip || onGround || horizontalMovement == 0)
        {
            return;
        }

        if (horizontalMovement > 0)
            gravity.FlipRight();
        else
            gravity.FlipLeft();
    }

    public void playerFirstJump()
    {
        if (!justJumped && onGround)
        {
            justJumped = true;
            currentJumpTime = moveData.jumpTimer;
        }
    }
    public void playerEndJump()
    {

        if (justJumped)
            justJumped = false;
    }
    private Vector2 MovementDirection()
    {
        Vector2 Direction = new Vector2(0, 0);
        if (justJumped)
        {
            switch (gravity.chosenDirection)
            {
                case 0:
                    Direction = new Vector2(Acceleration(), rb.velocity.y + FallModifier().y + JumpDirection().y);
                    break;
                case 1:
                    Direction = new Vector2(rb.velocity.x + FallModifier().x + JumpDirection().x, Acceleration() * -1);
                    break;
                case 2:
                    Direction = new Vector2(Acceleration() * -1, rb.velocity.y - FallModifier().y + JumpDirection().y);
                    break;
                case 3:
                    Direction = new Vector2(rb.velocity.x - FallModifier().x + JumpDirection().x, Acceleration());
                    break;
            }
            return Direction;
        }

        switch (gravity.chosenDirection)
        {
            case 0:
                Direction = new Vector2(Acceleration(), rb.velocity.y + FallModifier().y);
                break;
            case 1:
                Direction = new Vector2(rb.velocity.x + FallModifier().x, Acceleration() * -1);
                break;
            case 2:
                Direction = new Vector2(Acceleration() * -1, rb.velocity.y - FallModifier().y);
                break;
            case 3:
                Direction = new Vector2(rb.velocity.x - FallModifier().x, Acceleration());
                break;
        }

        return Direction;
    }
    private float Acceleration()
    {
        maxVelocity = MaxSpeed();

        if (horizontalMovement > 0)
            currentVelocity = Mathf.Lerp(currentVelocity, maxVelocity, moveData.accelerationSpeed * Time.deltaTime);
        else if (horizontalMovement < 0)
            currentVelocity = Mathf.Lerp(currentVelocity, maxVelocity * -1, moveData.accelerationSpeed * Time.deltaTime);
        else
            currentVelocity = Mathf.Lerp(currentVelocity, 0, moveData.accelerationSpeed * Time.deltaTime);

        return currentVelocity;
    }

    private Vector2 JumpDirection()
    {

        Vector2 Direction = new Vector2(0, 0);
        switch (gravity.chosenDirection)
        {
            case 0:
                Direction = Vector2.up * moveData.jumpForce;
                break;
            case 1:
                Direction = Vector2.right * moveData.jumpForce;
                break;
            case 2:
                Direction = Vector2.down * moveData.jumpForce;
                break;
            case 3:
                Direction = Vector2.left * moveData.jumpForce;
                break;
        }

        return Direction;
    }

    private float MaxSpeed()
    {
        if (horizontalMovement == 0 && onGround)
        {
            IdleTime();
            return 0;
        }

        if (controls.isSprint && onGround)
        {
            if (horizontalMovement != 0 && onGround)
            {
                //playerAnim.Play("Run");
                playerAnim.SetBool("isRunning", true);

                if (landed)
                    RunSFX(floorSFXIndex, 0.2f);
                else
                    LandingSFX(floorSFXIndex);
            }

            playerAnim.SetBool("isJumping", false);
            return moveData.sprintMovementSpeed;
        }

        if (horizontalMovement != 0 && onGround)
        {
            //playerAnim.Play("Walk");
            playerAnim.SetBool("isRunning", false);
            playerAnim.SetBool("isMoving", true);

            if (landed)
                WalkSFX(floorSFXIndex, 0.3f);
            else
                LandingSFX(floorSFXIndex);
        }

        if (!onGround)
        {
            landed = false;
            //playerAnim.Play("Jump");
            playerAnim.SetBool("isJumping", true);
        }
        else
        {
            playerAnim.SetBool("isJumping", false);

            if (!landed)
                LandingSFX(floorSFXIndex);
        }


        return moveData.baseMovementSpeed;
    }

    private void IdleTime()
    {
        if (horizontalMovement == 0 && onGround)
        {
            if (IdleTimer > 0)
            {
                IdleTimer -= Time.deltaTime;
                return;
            }
        }
        IdleTimer = moveData.idleTime;


        //playerAnim.Play("Idle");
        playerAnim.SetBool("isRunning", false);
        playerAnim.SetBool("isJumping", false);
        playerAnim.SetBool("isMoving", false);
        controls.isSprint = false;
    }

    private Vector2 FallModifier()
    {
        Vector2 fallDirection = new Vector2(0, 0);

        switch (gravity.chosenDirection)
        {
            case 0:
                if (rb.velocity.y < 0)
                    fallDirection += Vector2.up * gravity.gravityDirection[gravity.chosenDirection].y * (moveData.fallMultiplier - 1) * Time.deltaTime;

                //else if (rb.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
                //fallDirection += Vector2.up * gravity.gravityDirection[gravity.chosenDirection].y * (lowJumpMultiplier - 1) * Time.deltaTime;

                break;
            case 1:
                if (rb.velocity.x < 0)
                    fallDirection += Vector2.right * gravity.gravityDirection[gravity.chosenDirection].x * (moveData.fallMultiplier - 1) * Time.deltaTime;

                //else if (rb.velocity.x > 0 && !Input.GetKeyDown(KeyCode.Space))
                //fallDirection += Vector2.right * gravity.gravityDirection[gravity.chosenDirection].x * (lowJumpMultiplier - 1) * Time.deltaTime;

                break;
            case 2:
                if (rb.velocity.y > 0)
                    fallDirection += Vector2.down * gravity.gravityDirection[gravity.chosenDirection].y * (moveData.fallMultiplier - 1) * Time.deltaTime;

                //else if (rb.velocity.y < 0 && !Input.GetKeyDown(KeyCode.Space))
                //fallDirection += Vector2.down * gravity.gravityDirection[gravity.chosenDirection].y * (lowJumpMultiplier - 1) * Time.deltaTime;

                break;
            case 3:
                if (rb.velocity.x > 0)
                    fallDirection += Vector2.left * gravity.gravityDirection[gravity.chosenDirection].x * (moveData.fallMultiplier - 1) * Time.deltaTime;

                //else if (rb.velocity.x < 0 && !Input.GetKeyDown(KeyCode.Space))
                //fallDirection += Vector2.left * gravity.gravityDirection[gravity.chosenDirection].x  * (lowJumpMultiplier - 1) * Time.deltaTime;

                break;
        }
        return fallDirection;
    }

    public void SetCollisions()
    {
        List<GameObject> playerList = new List<GameObject>();

        foreach (GameObject newPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (newPlayer != gameObject)
                playerList.Add(newPlayer);
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            Physics2D.IgnoreCollision(playerList[i].GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        //{
            //Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        //}
    }
}

