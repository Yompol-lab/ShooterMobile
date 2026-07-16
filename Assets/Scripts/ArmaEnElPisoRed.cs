using Fusion;
using UnityEngine;

public class ArmaEnElPisoRed : NetworkBehaviour
{
    [Header("Configuración")]
    public WeaponSlot weaponSlot;

    [Header("Posición al equipar en la mano")]
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public Vector3 holdScale = Vector3.one;

    private Rigidbody rb;
    private Collider[] allColliders;
    private bool agarrada = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        allColliders = GetComponentsInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other) { IntentarAgarrar(other); }
    private void OnTriggerStay(Collider other) { IntentarAgarrar(other); }

    private void IntentarAgarrar(Collider other)
    {
        if (agarrada || Object == null || !Object.IsValid) return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null && inventory.Object.HasInputAuthority)
        {
            if (weaponSlot == WeaponSlot.Primary && inventory.currentPrimary != null) return;
            if (weaponSlot == WeaponSlot.Secondary && inventory.currentSecondary != null) return;
            if (weaponSlot == WeaponSlot.Bomb && inventory.currentBomb != null) return;

            agarrada = true;
            inventory.JuntarArmaDelPiso(Object, weaponSlot);
        }
    }

    public void SetFisicas(bool enElPiso)
    {
        agarrada = !enElPiso;
        if (rb != null)
        {
            rb.isKinematic = !enElPiso;
            rb.useGravity = enElPiso;
        }
        foreach (Collider col in allColliders)
        {
            col.enabled = enElPiso;
        }
    }
}