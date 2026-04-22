using UnityEngine;

public class GroundWeapon : MonoBehaviour
{
    public WeaponSlot weaponSlot;

    
    private Rigidbody rb;
    private Collider[] colliders;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        colliders = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                
                bool pickedUp = inventory.PickupWeapon(this.gameObject, weaponSlot);

                if (pickedUp)
                {
                    
                    rb.isKinematic = true;
                    foreach (Collider col in colliders) col.enabled = false;
                }
            }
        }
    }

    
    public void EnablePhysics()
    {
        rb.isKinematic = false;
        foreach (Collider col in colliders) col.enabled = true;
    }
}