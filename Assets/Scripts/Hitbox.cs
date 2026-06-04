using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health health;

    [Range(0.1f, 5f)]
    public float damageMultiplier = 1f;

   
    public void TakeDamage(float baseDamage, Vector3 hitPosition)
    {
        if (health == null) return;

        
        health.RPC_TakeDamage(baseDamage * damageMultiplier, hitPosition);
    }
}