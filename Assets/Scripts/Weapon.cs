using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Datos del Arma")]
    public WeaponData weaponData;

    [Header("Referencias")]
    public Transform firePoint;

    public void OnFireLocal()
    {
        if (weaponData == null) return;

        if (weaponData.muzzleFlashPrefab != null && firePoint != null)
        {
            
            GameObject flash = Instantiate(weaponData.muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);

            
            Destroy(flash, 0.05f);
        }
    }
}