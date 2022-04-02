using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_Rythm : MonoBehaviour
{
    private Rigidbody rigidbody;
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Character_Controls inputActions;
    private Vector2 movement;
    public float jumpHeight = 5f;
    public float jump = 5f;
    public float gravity = -9.8f;

    private float y;

    [Range(1,10)]
    public float speed = 5f;

    public float delayJump = 0.1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        inputActions = new Character_Controls();
        inputActions.Enable();
        inputActions.Player.Jump.performed += _ => Jump();
        y = gravity;
    }

    public void Jump()
    {
        Debug.Log("Jump");
        if (characterController.isGrounded)
        {
            y = jump;
        }
    }

    private void Update()
    {
        if(transform.position.y > jumpHeight)
        {
            y = gravity;
        }
        movement = inputActions.Player.Movement.ReadValue<Vector2>();
        characterController.Move(new Vector3(movement.x, y, movement.y) * speed * Time.deltaTime);
        if (characterController.isGrounded)
        {

        }
    }
}
