using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnnemy : MonoBehaviour
{
    public Transform player;


    private enum State
    {
        chasingTarget,
        attackingTarget
    }


    [Range(0f,5f)]
    public float speed = 1f;

    public float chaseRange = 3f;

    private CharacterController characterController;
    private Vector3 direction = Vector3.left;
    private State state;
    private Animator animator;


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        characterController = GetComponent<CharacterController>();
        state = State.chasingTarget;
        //animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(this.transform.position, player.position) > chaseRange)
        {
            state = State.chasingTarget;
        }
        else
        {
            state = State.attackingTarget;
        }


        switch (state)
        {
            case State.chasingTarget:
                    if (this.transform.position.x > this.player.position.x)
                    {
                        direction = Vector3.left;
                    }
                    else
                    {
                        direction = Vector3.right;
                    }
                    characterController.Move(direction * Time.deltaTime * speed);
            break;
            case State.attackingTarget:
                Debug.Log("Attack");
                animator.SetTrigger("attack");
            break;
        }

    }
}
