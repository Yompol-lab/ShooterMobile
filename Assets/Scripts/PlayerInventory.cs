using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponSlot { Primary, Secondary, Knife, Bomb, Utility }

public class PlayerInventory : MonoBehaviour
{
    [Header("Configuración Básica")]
    public Transform weaponContainer;
    public Transform dropPoint;
    public float dropForce = 5f;

    
    private GameObject currentPrimary;
    private GameObject currentSecondary;
    private GameObject currentKnife;
    private WeaponSlot activeSlot;

    void Update()
    {
        if (Keyboard.current != null)
        {
            
            if (Keyboard.current.digit1Key.wasPressedThisFrame && currentPrimary != null) EquipSlot(WeaponSlot.Primary);
            if (Keyboard.current.digit2Key.wasPressedThisFrame && currentSecondary != null) EquipSlot(WeaponSlot.Secondary);
            if (Keyboard.current.digit3Key.wasPressedThisFrame && currentKnife != null) EquipSlot(WeaponSlot.Knife);

            
            if (Keyboard.current.gKey.wasPressedThisFrame) DropCurrentWeapon();
        }
    }

    void EquipSlot(WeaponSlot slot)
    {
        if (currentPrimary != null) currentPrimary.SetActive(slot == WeaponSlot.Primary);
        if (currentSecondary != null) currentSecondary.SetActive(slot == WeaponSlot.Secondary);
        if (currentKnife != null) currentKnife.SetActive(slot == WeaponSlot.Knife);
        activeSlot = slot;
    }

    
    public bool PickupWeapon(GameObject physicalWeapon, WeaponSlot slot)
    {
        if (slot == WeaponSlot.Primary && currentPrimary != null) return false;
        if (slot == WeaponSlot.Secondary && currentSecondary != null) return false;

        
        physicalWeapon.transform.SetParent(weaponContainer);
        physicalWeapon.transform.localPosition = Vector3.zero;
        physicalWeapon.transform.localRotation = Quaternion.identity;

        
        if (slot == WeaponSlot.Primary) currentPrimary = physicalWeapon;
        else if (slot == WeaponSlot.Secondary) currentSecondary = physicalWeapon;
        else if (slot == WeaponSlot.Knife) currentKnife = physicalWeapon;

        
        physicalWeapon.SetActive(false);

        return true;
    }

    
    public void DropCurrentWeapon()
    {
        GameObject weaponToDrop = null;

        if (activeSlot == WeaponSlot.Primary && currentPrimary != null)
        {
            weaponToDrop = currentPrimary;
            currentPrimary = null;
            if (currentSecondary != null) EquipSlot(WeaponSlot.Secondary);
            else EquipSlot(WeaponSlot.Knife);
        }
        else if (activeSlot == WeaponSlot.Secondary && currentSecondary != null)
        {
            weaponToDrop = currentSecondary;
            currentSecondary = null;
            EquipSlot(WeaponSlot.Knife);
        }

        if (weaponToDrop != null)
        {
           
            weaponToDrop.transform.SetParent(null);
            weaponToDrop.transform.position = dropPoint.position;
            weaponToDrop.transform.rotation = dropPoint.rotation;

            
            GroundWeapon groundScript = weaponToDrop.GetComponent<GroundWeapon>();
            if (groundScript != null) groundScript.EnablePhysics();

            
            Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
        }
    }
}