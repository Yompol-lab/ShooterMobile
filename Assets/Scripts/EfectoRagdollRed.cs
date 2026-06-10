using Fusion;
using UnityEngine;

public class EfectoRagdollRed : NetworkBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    private Rigidbody[] huesos;

    private void Awake()
    {
        huesos = GetComponentsInChildren<Rigidbody>();
        DesactivarRagdoll();
    }

    public void DesactivarRagdoll()
    {
        foreach (Rigidbody rb in huesos) rb.isKinematic = true;
    }

    public void Morir(Vector3 direccionDelTiro)
    {
        if (HasStateAuthority) RPC_ActivarRagdoll(direccionDelTiro);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ActivarRagdoll(Vector3 direccionImpacto)
    {
        if (animator != null) animator.enabled = false;
        if (characterController != null) characterController.enabled = false;

        foreach (Rigidbody rb in huesos)
        {
            rb.isKinematic = false;
            rb.AddForce(direccionImpacto * 50f, ForceMode.Impulse);
        }
    }
}