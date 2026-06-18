using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Información")]
    public string weaponName;
    public WeaponSlot weaponSlot;

    [Header("Disparo")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.2f;
    public bool automatic = false;

    [Header("Escopetas (Perdigones)")]
    public int pelletsPerShot = 1;

    [Header("Precisión")]
    public float spread = 0.01f;

    [Header("Kamehameha (Solo Pistola de Agua)")]
    public float beamRadius = 0.5f;

    [Header("Efectos")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletImpactPrefab;

    
    [Header("Tienda")]
    public int precio = 2700;
    public GameObject prefabParaMano;
}