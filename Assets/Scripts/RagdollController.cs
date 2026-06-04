using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    public Animator animator;

    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void DisableRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.gameObject == gameObject)
                continue;

            rb.isKinematic = true;
        }

        foreach (Collider col in colliders)
        {
            if (col.gameObject == gameObject)
                continue;

            col.enabled = false;
        }
    }

    public void EnableRagdoll()
    {
        if (animator != null)
            animator.enabled = false;

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}