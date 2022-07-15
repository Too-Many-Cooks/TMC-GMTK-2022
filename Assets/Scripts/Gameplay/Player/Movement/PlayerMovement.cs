using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // (INPUT) Axis variables:
    float xValue, yValue;
    bool wantToJump;


    // Stored variables:
    bool isGrounded, canJump;
    float jumpTimer = 0f;
    Vector3 playerFallVelocity, movementXZValue, oldMovementXZValue = new Vector3(0, 0, 0);
    [HideInInspector] public float playerSpeedMultyplier = 1;

    // Serialized variables:
    [SerializeField] float playerSpeed = 13f, playerJumpPower = 10f, jumpReloadTimer = 1f, groundDistanceCheck = 0.4f;
    [SerializeField] [Range(0.1f, 3f)] float fallingGravityMultiplyer = 1.3f;
    [SerializeField] [Range(0.01f, 0.9f)] float airXZMovementMultiplier = 0.04f;
    [SerializeField] [Range(-30f, 30f)] float playerGravity = -9.81f;
    [SerializeField] CharacterController playerController;
    [SerializeField] Transform groundCheck;
    public LayerMask groundLayer;


    // Update is called once per frame
    void Update()
    {
        UpdateJumpTimer();


        CheckIfGrounded();
        

        // Adding the X and Y movement vectors based on player orientation:

        if (isGrounded)
        {
            movementXZValue = playerController.transform.right * xValue + playerController.transform.forward * yValue;
            movementXZValue *= playerSpeedMultyplier;
        }
        else
        {
            movementXZValue = (playerController.transform.right * xValue + playerController.transform.forward * yValue)
                                    * airXZMovementMultiplier + oldMovementXZValue * (1 - airXZMovementMultiplier);
            movementXZValue *= playerSpeedMultyplier;
        }

        oldMovementXZValue = movementXZValue;


        // Jumping:
        if (isGrounded & canJump & wantToJump)
        {
            canJump = false;

            playerFallVelocity.y = Mathf.Sqrt(playerJumpPower * -2f * playerGravity);
            // Math so that changing the gravity does not affect the player's jump.
        }


        // Moving the player: (Diferent if it is airbone that if it isn't)
        if (isGrounded)
            playerController.Move((movementXZValue * playerSpeed + playerFallVelocity) * Time.deltaTime);

        else
            playerController.Move((movementXZValue * playerSpeed + playerFallVelocity) * Time.deltaTime);
    }


    // Updates the timer that dictates if the character can jump.
    private void UpdateJumpTimer()
    {
        if (canJump == false)
            jumpTimer += Time.deltaTime;


        if (jumpTimer > jumpReloadTimer)
        {
            canJump = true;
            jumpTimer = 0f;
        }
    }


    private void OnJump()
    {
        wantToJump = true;
    }
    private void OnMove()
    {
        xValue = Input.GetAxis("Horizontal");
        yValue = Input.GetAxis("Vertical");
    }


    // Checks if the player isGrounded and, if they are, adds a small negative speed to it.
    private void CheckIfGrounded()
    {
        // Y movement (gravity, falls and jumps):
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundLayer);


        if (!isGrounded)
        {
            // If we are airborn, we want to check if we are falling to apply our multyplier.
            if(playerFallVelocity.y > 0)
                playerFallVelocity = new Vector3(0, playerFallVelocity.y + playerGravity * Time.deltaTime, 0);
            else
                playerFallVelocity = new Vector3(0, playerFallVelocity.y + playerGravity * Time.deltaTime * fallingGravityMultiplyer, 0);
        }
        else
        {
            if (playerFallVelocity.y < 0)
                playerFallVelocity.y = -2f; // Negative velocity just in case we are close but not exactly in the ground.
        }
    }
}
