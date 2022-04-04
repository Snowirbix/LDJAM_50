using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnnemy : MonoBehaviour
{
    public Transform player;
    public Transform hachoir;

    public float offsetAttack = 0.3f;

    private Vector3 position;

    public enum State
    {
        chasingTarget,
        attackingTarget
    }


    [Range(0f,5f)]
    public float speed = 1f;

    public float backwardSpeedo = 2f;

    public float chaseRange = 1f;

    private CharacterController characterController;
    private Vector3 direction = Vector3.left;
    public State state;
    private Animator animator;


    private void Awake()
    {
        player = GameObject.Find("CHAMPI").transform;
        characterController = GetComponent<CharacterController>();
        state = State.chasingTarget;
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        position = new Vector3(hachoir.position.x, this.player.position.y, this.player.position.z);


        if ( Mathf.Abs(position.x - player.position.x) > chaseRange)
        {
            state = State.chasingTarget;
        }
        else
        {
            state = State.attackingTarget;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            switch (state)
            {
                case State.chasingTarget:
                    if (position.x > this.player.position.x)
                    {
                        direction = Vector3.left;
                    }
                    else
                    {
                        direction = Vector3.right * backwardSpeedo;
                    }
                    characterController.Move(direction * Time.deltaTime * speed);
                    break;
                case State.attackingTarget:
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        animator.SetTrigger("attack");
                    }
                    break;
            }
        }
    }
}
