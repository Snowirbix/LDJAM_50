using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceive : MonoBehaviour
{
    public Health health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.tag.Contains("Player"))
        {
            Debug.Log("touch by the player");
            health.Damage(10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
