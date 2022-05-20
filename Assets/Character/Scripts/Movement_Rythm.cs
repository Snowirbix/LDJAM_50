using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Movement_Rythm : MonoBehaviour
{
    private Animator animator;
    private Health health;
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

    [Range(1, 10)]
    public float speed = 4f;

    public float delayJump = 0.1f;

    public float invicibility = 0.5f;
    private float lastHit = 0f;

    public VisualEffect attackSlash;
    public AudioSource slashSound;

    public Transform travellingPoint;
    public Vector3 travellingOrigin;
    
    public UnityEvent attackEvent;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        inputActions = new Character_Controls();
        inputActions.Player.Jump.performed += _ => Jump();

        inputActions.Player.Attack.performed += _ => Attack();

        forward = Vector3.left;
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        travellingOrigin = travellingPoint.position;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(1).IsName("ChampAttack"))
        {
            attackEvent.Invoke();
            animator.SetTrigger("Attack");
            slashSound.Play();
            attackSlash.Play();
            velocity.y = Mathf.Sqrt(jumpHeight * 0.5f * -2f * gravity);
        }
    }

    public void Die()
    {
        inputActions.Disable();
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Update()
    {
        movement = inputActions.Player.Movement.ReadValue<Vector2>();
        animator.SetFloat("Speed", Mathf.Abs(movement.x));
        Vector3 move = transform.right * movement.x;

        if (movement.x < 0 && forward.x < 0)
        {
            forward = Vector3.right;
        }

        if (movement.x > 0 && forward.x > 0)
        {
            forward = -forward;
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            characterController.Move(-move * speed * Time.deltaTime);
        }
        else
        {
            characterController.Move(-move * speed * (aerialControlPercentage / 100) * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        characterTransform.rotation = Quaternion.Lerp(characterTransform.rotation, Quaternion.LookRotation(forward), 0.1f);

        var posX = (characterTransform.position.x - travellingOrigin.x) * 0.5f;
        travellingPoint.position = new Vector3(travellingOrigin.x + posX, travellingPoint.position.y, travellingPoint.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Enemy"))
        {
            if (Time.time > lastHit + invicibility)
            {
                if (!health.isDead)
                {
                    lastHit = Time.time;
                    health.Damage(10);
                    animator.SetTrigger("Hit");
                }
            }
        }

    }
}
