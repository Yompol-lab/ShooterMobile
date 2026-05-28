using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum WeaponSlot { Primary, Secondary, Knife, Bomb, Utility }

public class PlayerInventory : MonoBehaviour
{
    [Header("Configuración de Red")]
    public Transform weaponContainer;
    public Transform dropPoint;
    public float dropForce = 5f;

    [Header("Slots Actuales")]
    public GameObject currentPrimary;
    public GameObject currentSecondary;
    public GameObject currentKnife;
    public GameObject currentBomb;
    public List<GameObject> currentUtilities = new List<GameObject>();

    private WeaponSlot activeSlot;
    private int utilityIndex = 0;

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                EquipSlot(WeaponSlot.Primary);

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                EquipSlot(WeaponSlot.Secondary);

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                EquipSlot(WeaponSlot.Knife);

            if (Keyboard.current.digit4Key.wasPressedThisFrame && currentUtilities.Count > 0)
                CycleUtilities();

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
                EquipSlot(WeaponSlot.Bomb);

            if (Keyboard.current.gKey.wasPressedThisFrame)
                DropCurrentWeapon();
        }
    }

    public void EquipSlot(WeaponSlot slot)
    {
        GameObject equippedWeaponObject = null;

        switch (slot)
        {
            case WeaponSlot.Primary:
                if (currentPrimary == null)
                {
                    Debug.LogWarning("Todavía no agarraste arma primaria.");
                    return;
                }

                equippedWeaponObject = currentPrimary;
                break;

            case WeaponSlot.Secondary:
                if (currentSecondary == null)
                {
                    Debug.LogWarning("Todavía no agarraste arma secundaria.");
                    return;
                }

                equippedWeaponObject = currentSecondary;
                break;

            case WeaponSlot.Knife:
                if (currentKnife == null)
                {
                    Debug.LogWarning("Todavía no agarraste cuchillo.");
                    return;
                }

                equippedWeaponObject = currentKnife;
                break;

            case WeaponSlot.Bomb:
                if (currentBomb == null)
                {
                    Debug.LogWarning("Todavía no agarraste bomba.");
                    return;
                }

                equippedWeaponObject = currentBomb;
                break;

            case WeaponSlot.Utility:
                if (currentUtilities.Count <= 0)
                {
                    Debug.LogWarning("No tenés utilidades.");
                    return;
                }

                equippedWeaponObject = currentUtilities[utilityIndex];
                break;
        }

        HideAllWeapons();

        equippedWeaponObject.SetActive(true);
        activeSlot = slot;

        PlayerWeaponController weaponController = GetComponent<PlayerWeaponController>();

        if (weaponController != null)
        {
            Weapon weapon = equippedWeaponObject.GetComponent<Weapon>();

            if (weapon != null)
            {
                weaponController.SetCurrentWeapon(weapon);
                Debug.Log("Arma equipada: " + equippedWeaponObject.name);
            }
            else
            {
                Debug.LogWarning("El objeto equipado no tiene script Weapon.");
            }
        }
    }

    private void HideAllWeapons()
    {
        if (currentPrimary != null)
            currentPrimary.SetActive(false);

        if (currentSecondary != null)
            currentSecondary.SetActive(false);

        if (currentKnife != null)
            currentKnife.SetActive(false);

        if (currentBomb != null)
            currentBomb.SetActive(false);

        foreach (var util in currentUtilities)
        {
            if (util != null)
                util.SetActive(false);
        }
    }

    void CycleUtilities()
    {
        if (currentUtilities.Count <= 0)
            return;

        if (activeSlot == WeaponSlot.Utility)
        {
            utilityIndex = (utilityIndex + 1) % currentUtilities.Count;
        }

        EquipSlot(WeaponSlot.Utility);
    }

    public bool PickupWeapon(GameObject physicalWeapon, WeaponSlot slot)
    {
        if (physicalWeapon == null)
            return false;

        if (slot == WeaponSlot.Primary && currentPrimary != null)
            return false;

        if (slot == WeaponSlot.Secondary && currentSecondary != null)
            return false;

        if (slot == WeaponSlot.Knife && currentKnife != null)
            return false;

        if (slot == WeaponSlot.Bomb && currentBomb != null)
            return false;

        GroundWeapon groundWeapon = physicalWeapon.GetComponent<GroundWeapon>();

        physicalWeapon.transform.SetParent(weaponContainer);

        if (groundWeapon != null)
        {
            physicalWeapon.transform.localPosition = groundWeapon.holdPosition;
            physicalWeapon.transform.localRotation = Quaternion.Euler(groundWeapon.holdRotation);
            physicalWeapon.transform.localScale = groundWeapon.holdScale;
        }
        else
        {
            physicalWeapon.transform.localPosition = Vector3.zero;
            physicalWeapon.transform.localRotation = Quaternion.identity;
            physicalWeapon.transform.localScale = Vector3.one;
        }

        switch (slot)
        {
            case WeaponSlot.Primary:
                currentPrimary = physicalWeapon;
                break;

            case WeaponSlot.Secondary:
                currentSecondary = physicalWeapon;
                break;

            case WeaponSlot.Knife:
                currentKnife = physicalWeapon;
                break;

            case WeaponSlot.Bomb:
                currentBomb = physicalWeapon;
                break;

            case WeaponSlot.Utility:
                currentUtilities.Add(physicalWeapon);
                break;
        }

        physicalWeapon.SetActive(false);

        Debug.Log("Arma agarrada en slot: " + slot + " / " + physicalWeapon.name);

        EquipSlot(slot);

        return true;
    }

    public void DropCurrentWeapon()
    {
        GameObject weaponToDrop = null;

        if (activeSlot == WeaponSlot.Primary && currentPrimary != null)
        {
            weaponToDrop = currentPrimary;
            currentPrimary = null;
        }
        else if (activeSlot == WeaponSlot.Secondary && currentSecondary != null)
        {
            weaponToDrop = currentSecondary;
            currentSecondary = null;
        }
        else if (activeSlot == WeaponSlot.Bomb && currentBomb != null)
        {
            weaponToDrop = currentBomb;
            currentBomb = null;
        }

        if (weaponToDrop != null)
        {
            weaponToDrop.transform.SetParent(null);
            weaponToDrop.transform.position = dropPoint.position;
            weaponToDrop.transform.rotation = dropPoint.rotation;

            GroundWeapon groundScript = weaponToDrop.GetComponent<GroundWeapon>();

            if (groundScript != null)
                groundScript.EnablePhysics();

            Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);

            if (currentKnife != null)
                EquipSlot(WeaponSlot.Knife);
        }
    }
}