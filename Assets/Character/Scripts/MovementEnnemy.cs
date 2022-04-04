using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementEnnemy : MonoBehaviour
{
    public Transform player;
    public Transform hachoir;

    public float delayAttack = 3f;
    private float attackTime;

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

    public List<string> attacksNames = new List<string>();

    private void Awake()
    {
        player = GameObject.Find("CHAMPI").transform;
        characterController = GetComponent<CharacterController>();
        state = State.chasingTarget;
        animator = GetComponentInChildren<Animator>();

        attacksNames.Add("Attack");
        attacksNames.Add("TchakTchakTchak");
        attackTime = 0f;
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

        if (IsNotAttacking())
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
                    if(Time.time > attackTime+delayAttack)
                    {
                        attackTime = Time.time;
                        switch (Random.Range(1, 3))
                        {
                            case 1:
                                animator.SetTrigger("attack");
                                break;
                            case 2:
                                animator.SetTrigger("tchaktchaktchak");
                                break;
                        }
                    }
                    break;
            }
        }
    }

    private bool IsNotAttacking()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        string name = attacksNames.Find(attackName => animatorStateInfo.IsName(attackName));

        return name == null;
    }
}
