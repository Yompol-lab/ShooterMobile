using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;

   
    [Networked]
    public float currentHealth { get; set; }

    [Networked]
    public NetworkBool isDead { get; set; }

    [Header("Muerte")]
    public RagdollController ragdoll;

    [Header("Sangre")]
    public GameObject bloodPrefab;

    public override void Spawned()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (ragdoll != null)
            ragdoll.DisableRagdoll();
    }

   
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(float damage, Vector3 hitPosition)
    {
        if (isDead) return;

        currentHealth -= damage;

        
        RPC_PlayBloodEffect(hitPosition);

        if (currentHealth <= 0)
        {
            isDead = true;
            RPC_Die(); 
        }
    }

    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayBloodEffect(Vector3 pos)
    {
        if (bloodPrefab != null)
        {
            Instantiate(bloodPrefab, pos, Quaternion.identity);
        }
    }

    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Die()
    {
        Debug.Log(gameObject.name + " murió");

        if (ragdoll != null)
            ragdoll.EnableRagdoll();

        
    }
}