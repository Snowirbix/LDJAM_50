using UnityEngine;

public class DamageReceive : MonoBehaviour
{
    public Health health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("SWORD"))
        {
            health.Damage(10);
        }
    }
}
