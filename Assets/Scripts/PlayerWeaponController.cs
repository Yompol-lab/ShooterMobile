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
    public float impactLifeTime = 3f;

    private float nextTimeToFire = 0f;
    private ParticleSystem[] waterBeamParticles;


    private float currentChargeTime = 0f;
    public float waterTravelTime = 0.5f;

    void Update()
    {
        if (currentWeapon == null || currentWeapon.weaponData == null || currentWeapon.muzzlePoint == null) return;

        WeaponData data = currentWeapon.weaponData;


        if (data.weaponName == "PistolaAgua")
        {
            bool isHoldingClick = Mouse.current.leftButton.isPressed;


            HandleWaterBeamVisuals(isHoldingClick);

            if (isHoldingClick)
            {

                currentChargeTime += Time.deltaTime;


                if (currentChargeTime >= waterTravelTime)
                {
                    if (Time.time >= nextTimeToFire)
                    {
                        nextTimeToFire = Time.time + data.fireRate;
                        ShootKamehameha(data);
                    }
                }
            }
            else
            {

                currentChargeTime = 0f;
            }
        }

        else
        {
            if (data.automatic)
            {
                if (Mouse.current.leftButton.isPressed) TryShoot();
            }
            else
            {
                if (Mouse.current.leftButton.wasPressedThisFrame) TryShoot();
            }
        }
    }

    void HandleWaterBeamVisuals(bool isFiring)
    {

        if (waterBeamParticles == null || waterBeamParticles.Length == 0)
        {
            waterBeamParticles = currentWeapon.muzzlePoint.GetComponentsInChildren<ParticleSystem>();
        }

        foreach (var ps in waterBeamParticles)
        {
            if (isFiring && !ps.isPlaying) ps.Play(true);
            else if (!isFiring && ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void TryShoot()
    {
        WeaponData data = currentWeapon.weaponData;
        if (Time.time < nextTimeToFire) return;

        nextTimeToFire = Time.time + data.fireRate;


        if (data.weaponName == "PistolaAgua") ShootKamehameha(data);
        else ShootNormal(data);
    }

    void ShootNormal(WeaponData data)
    {
        if (data.muzzleFlashPrefab != null)
        {
            
            GameObject flash = Instantiate(data.muzzleFlashPrefab, currentWeapon.muzzlePoint.position, currentWeapon.muzzlePoint.rotation, currentWeapon.muzzlePoint);

            flash.transform.localScale = Vector3.one;

            
            Destroy(flash, 0.12f);
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 shootDirection = ray.direction;
        shootDirection += new Vector3(Random.Range(-data.spread, data.spread), Random.Range(-data.spread, data.spread), 0f);
        shootDirection.Normalize();

        if (Physics.Raycast(ray.origin, shootDirection, out RaycastHit hit, data.range, impactLayers))
        {
            ProcessImpact(hit, data);
            SpawnImpact(hit, data);
        }
    }

    void ShootKamehameha(WeaponData data)
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));


        RaycastHit[] hits = Physics.SphereCastAll(ray.origin, data.beamRadius, ray.direction, data.range, impactLayers);

        foreach (var hit in hits)
        {
            ProcessImpact(hit, data);
        }
    }

    void ProcessImpact(RaycastHit hit, WeaponData data)
    {

        Health health = hit.collider.GetComponentInParent<Health>();
        if (health != null) health.TakeDamage(data.damage);


        FireExtinguisher extintor = hit.collider.GetComponentInParent<FireExtinguisher>();
        if (extintor != null) extintor.TriggerSmoke();


        if (data.weaponName == "PistolaAgua")
        {
            FireTarget fuego = hit.collider.GetComponentInParent<FireTarget>();
            if (fuego != null) fuego.Extinguish();
        }
    }

    void SpawnImpact(RaycastHit hit, WeaponData data)
    {
        if (data.bulletImpactPrefab == null) return;
        GameObject impact = Instantiate(data.bulletImpactPrefab, hit.point + hit.normal * decalOffset, Quaternion.LookRotation(hit.normal));
        impact.transform.SetParent(hit.collider.transform);
        Destroy(impact, impactLifeTime);
    }

    public void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        nextTimeToFire = 0f;
        waterBeamParticles = null;
    }
}