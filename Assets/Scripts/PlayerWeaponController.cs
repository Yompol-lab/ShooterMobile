using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Referencias")]
    public Camera mainCamera;

    [Header("Arma Actual")]
    public Weapon currentWeapon;

    [Header("Capas")]
    public LayerMask impactLayers;

    [Header("Decal")]
    public float decalOffset = 0.01f;
    public float decalLifeTime = 25f;
    public float impactLifeTime = 3f;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (currentWeapon == null) return;
        if (currentWeapon.weaponData == null) return;
        if (currentWeapon.muzzlePoint == null) return;

        WeaponData data = currentWeapon.weaponData;

        if (data.automatic)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                TryShoot();
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryShoot();
            }
        }
    }

    void TryShoot()
    {
        WeaponData data = currentWeapon.weaponData;

        if (Time.time < nextTimeToFire) return;

        nextTimeToFire = Time.time + data.fireRate;

        Shoot();
    }

    void Shoot()
    {
        WeaponData data = currentWeapon.weaponData;

        if (data.muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(
                data.muzzleFlashPrefab,
                currentWeapon.muzzlePoint.position,
                currentWeapon.muzzlePoint.rotation
            );

            Destroy(flash, 1f);
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 shootDirection = ray.direction;

        shootDirection += new Vector3(
            Random.Range(-data.spread, data.spread),
            Random.Range(-data.spread, data.spread),
            0f
        );

        shootDirection.Normalize();

        RaycastHit hit;

        if (Physics.Raycast(ray.origin, shootDirection, out hit, data.range, impactLayers))
        {
            Debug.Log("Impacto en: " + hit.collider.name);

            ApplyDamage(hit, data.damage);
            SpawnImpact(hit, data);
        }
    }

    void ApplyDamage(RaycastHit hit, float damage)
    {
        Health health = hit.collider.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    void SpawnImpact(RaycastHit hit, WeaponData data)
    {
        if (data.bulletImpactPrefab == null) return;

        GameObject impact = Instantiate(
            data.bulletImpactPrefab,
            hit.point + hit.normal * decalOffset,
            Quaternion.LookRotation(hit.normal)
        );

        impact.transform.SetParent(hit.collider.transform);

        Destroy(impact, impactLifeTime);
    }


    void SpawnMuzzleFlash(WeaponData data)
    {
        if (data.muzzleFlashPrefab == null) return;
        if (currentWeapon == null || currentWeapon.muzzlePoint == null) return;

        GameObject flash = Instantiate(
            data.muzzleFlashPrefab,
            currentWeapon.muzzlePoint.position,
            currentWeapon.muzzlePoint.rotation,
            currentWeapon.muzzlePoint
        );

        flash.transform.localPosition = Vector3.zero;
        flash.transform.localRotation = Quaternion.identity;

        ParticleSystem[] particles = flash.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particles)
        {
            var main = ps.main;
            main.loop = false;
            main.playOnAwake = true;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }

        Light[] lights = flash.GetComponentsInChildren<Light>();

        foreach (Light light in lights)
        {
            light.enabled = true;
        }

        Destroy(flash, 0.07f);
    }

    public void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        nextTimeToFire = 0f;
    }
}