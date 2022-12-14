using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGravityControl : MonoBehaviour
{
    private GameObject Target; // which target to focus around

    //these are public to allow the camera systems to sync with this infoation to rotate with the player smoothly
    [HideInInspector] public Vector2[] gravityDirection = new Vector2[4]; 
    public float gravityStrength = 0.98f;
    private string[] gravityDirectionString = new string[4];

    public int chosenDirection = 0;
    public int currentRotation = 0;
    public float rotationSpeed = 1;
    public bool InFlip = false;

    public float distanceFromFloor;

    PlayerMovement playerMovement;

    private void Start()
    {
        Target = gameObject;
        playerMovement = GetComponent<PlayerMovement>();

        gravityDirection[0] = new Vector2(0, -gravityStrength); // Red
        gravityDirection[1] = new Vector2(-gravityStrength, 0); // Yellow
        gravityDirection[2] = new Vector2(0, gravityStrength); // Green
        gravityDirection[3] = new Vector2(gravityStrength, 0); // Blue

        // since Top/Bottem/Left/Right cant be used as an accurate way to tell which rotation you are facing when in game, colors are used instead
        gravityDirectionString[0] = "Gravity Red";
        gravityDirectionString[1] = "Gravity Yellow";
        gravityDirectionString[2] = "Gravity Green";
        gravityDirectionString[3] = "Gravity Blue";
    }
    void FixedUpdate()
    {
        CheckDistanceFromGround(); // calulates the distance between the target and the floor below to rotate smoothly before the target lands to the ground
    }

    public void FlipRight() // rotates target right
    {
        chosenDirection--;
        if (chosenDirection < 0)
            chosenDirection = 3;

        Debug.Log("Left");
        if ((currentRotation + 90) > 360)
        {
            currentRotation = 0;
        }
        currentRotation += 90;


        StartCoroutine(Flip(chosenDirection));
    }

    public void FlipLeft() //  rotates target left
    {
        chosenDirection++;
        if (chosenDirection > 3)
            chosenDirection = 0;

        Debug.Log("Right");
        if ((currentRotation - 90) < 0)
        {
            currentRotation = 360;
        }

        currentRotation -= 90;


        StartCoroutine(Flip(chosenDirection));
    }

    private IEnumerator Flip(int Direction) // prevents multiple flips while mid air until target hits the ground
    {
        InFlip = true;
        //Debug.Log(gravityDirectionString[Direction]);

        yield return new WaitUntil(() => playerMovement.onGround);
        InFlip = false;
    }

    private void CheckDistanceFromGround()
    {
        //RaycastHit2D hit = Physics2D.Raycast(Target.transform.position, gravityDirection[chosenDirection], 10);

        Vector2 endPos = Target.transform.position * Vector2.right * 10;
        RaycastHit2D hit = new RaycastHit2D(); 
        
        // since the targets rotation keeps changing, the direction of the GROUND keeps changing, as such the check system must also change
        switch (chosenDirection)
        {
            case 0:
                hit = Physics2D.Raycast(Target.transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
                break;
            case 1:
                hit = Physics2D.Raycast(Target.transform.position, Vector2.left, Mathf.Infinity, LayerMask.GetMask("Ground"));
                break;
            case 2:
                hit = Physics2D.Raycast(Target.transform.position, Vector2.up, Mathf.Infinity, LayerMask.GetMask("Ground"));
                break;
            case 3:
                hit = Physics2D.Raycast(Target.transform.position, Vector2.right, Mathf.Infinity, LayerMask.GetMask("Ground"));
                break;
        }

        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.name);
            //Debug.DrawRay(Target.transform.position, hit.point, Color.green);
            distanceFromFloor = Vector2.Distance(Target.transform.position, hit.point);
            rotationSpeed = Mathf.Clamp((21 - distanceFromFloor) / 2, 2, 25); // this'll calaculate the speed of rotation for the player and camera based on the distance between the player and ground
        }

    }


}
