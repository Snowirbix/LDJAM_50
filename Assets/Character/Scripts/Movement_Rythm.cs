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
    public float jumpHeight = 0.5f;

    [Range(0, 100)]
    public float aerialControlPercentage = 50;

    private float gravity = Physics.gravity.y;

    private Vector3 velocity;

    public Transform characterTransform;

    private Vector3 forward;

    [Range(1,10)]
    public float speed = 4f;

    public float delayJump = 0.1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        inputActions = new Character_Controls();
        inputActions.Enable();
        inputActions.Player.Jump.performed += _ => Jump();

        forward = Vector3.forward;
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Update()
    {
        movement = inputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * movement.x;

        

        if(movement.x > 0 && forward.z < 0)
        {
            forward = Vector3.forward;
        }

        if(movement.x < 0 && forward.z > 0)
        {
            forward = -forward;
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            characterController.Move(move * speed * Time.deltaTime);
        }
        else
        {
            characterController.Move(move * speed * (aerialControlPercentage/100) * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
        //characterTransform.rotation = characterTransform.rotation * Quaternion.AngleAxis(-1f,Vector3.up);

        characterTransform.rotation = Quaternion.Lerp(characterTransform.rotation, Quaternion.LookRotation(forward), 0.05f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Ennemy"))
        {
            Debug.Log("touch� par un ennemy");
        }
        
    }
}
