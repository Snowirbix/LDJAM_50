using UnityEngine;
using UnityEngine.VFX;

public class DamageReceive : MonoBehaviour
{
    public Health health;
    public VisualEffect hitEffect;

    public float invicibility = 0.5f;
    private float lastHit = 0f;

    private static string SWORD_OBJECT_NAME = "SWORD";
    private int damageOnHit = 10;
    private Vector3 effectOffsetPosition = new Vector3(0, 0, 0.2f);

    private void OnTriggerEnter(Collider other)
    {
        if (IsSwordCollider(other) && IsVulnerable() && health.IsAlive())
        {
            PlayHitEffect(other.ClosestPointOnBounds(transform.position));
            ResetLastHitTime();
            ApplyDamageOnHealth(damageOnHit);
        }
    }

    private bool IsSwordCollider(Collider collider)
    {
        return collider.name.Contains(SWORD_OBJECT_NAME);
    }

    private bool IsVulnerable()
    {
        return Time.time > lastHit + invicibility;
    }

    private void PlayHitEffect(Vector3 hitPosition)
    {
        hitEffect.transform.position = hitPosition + effectOffsetPosition;
        hitEffect.Play();
    }

    private void ResetLastHitTime()
    {
        lastHit = Time.time;
    }

    private void ApplyDamageOnHealth(int damageOnHit)
    {
        health.Damage(damageOnHit);
    }
}
