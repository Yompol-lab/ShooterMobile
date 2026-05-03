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

    [Header("Precisión")]
    public float spread = 0.01f;

    [Header("Efectos")]
    public GameObject muzzleFlashPrefab;

    [Tooltip("Prefab que ya trae el impacto y el agujero de bala juntos")]
    public GameObject bulletImpactPrefab;
}