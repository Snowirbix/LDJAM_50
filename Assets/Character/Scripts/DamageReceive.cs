using UnityEngine;
using UnityEngine.VFX;

public class DamageReceive : MonoBehaviour
{
    public Health health;
    public VisualEffect hitEffect;

    public float invicibility = 0.5f;
    private float lastHit = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("SWORD"))
        {
            if (Time.time > lastHit + invicibility)
            {
                if (!health.isDead)
                {
                    var hit = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    hitEffect.transform.position = hit + new Vector3(0, 0, 0.2f);
                    hitEffect.Play();
                    lastHit = Time.time;
                    health.Damage(10);
                }
            }
        }
    }
}
