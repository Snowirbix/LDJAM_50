using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly]
    public int health = 0;
    public int maxHealth = 100;

    [System.Serializable]
    public class DamageEvent  : UnityEvent<int, int, int> {};

    /// <summary>
    /// damage amount, health, max health
    /// </summary>
    public DamageEvent  damageEvent;
    public UnityEvent   deathEvent;

    private Animator animator;

    [ReadOnly]
    public bool isDead = false;

    private void Start()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void Damage (int value)
    {
        health -= value;
        health = Mathf.Max(health, 0);
        damageEvent.Invoke(value, health, maxHealth);

        if (health == 0 && !isDead)
        {
            isDead = true;
            deathEvent.Invoke();
            animator.SetTrigger("Die");
        }
    }

}
