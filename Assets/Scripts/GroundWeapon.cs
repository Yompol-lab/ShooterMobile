using UnityEngine;

public class GroundWeapon : MonoBehaviour
{
    [Header("Configuración")]
    public WeaponSlot weaponSlot;

    [Header("Posición al equipar")]
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public Vector3 holdScale = Vector3.one;

    [Header("Colliders")]
    public Collider pickupTrigger;

    private Rigidbody rb;
    private Collider[] allColliders;
    private bool pickedUp = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        allColliders = GetComponentsInChildren<Collider>();

        if (pickupTrigger != null)
            pickupTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryPickup(other);
    }

    void TryPickup(Collider other)
    {
        if (pickedUp) return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
        {
            if (inventory.PickupWeapon(gameObject, weaponSlot))
            {
                pickedUp = true;

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                foreach (Collider col in allColliders)
                {
                    col.enabled = false;
                }
            }
        }
    }

    public void EnablePhysics()
    {
        pickedUp = false;

        foreach (Collider col in allColliders)
        {
            col.enabled = true;
        }

        if (pickupTrigger != null)
            pickupTrigger.isTrigger = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}