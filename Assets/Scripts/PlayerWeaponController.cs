using Fusion;
using UnityEngine;

namespace StarterAssets
{
    public class PlayerWeaponController : NetworkBehaviour
    {
        [Header("Inputs")]
        private StarterAssetsInputs starterInputs;
        private bool isShooting = false;

        [Header("Configuración de Armas")]
        public Weapon currentWeapon;

        [Header("Network Damage Settings")]
        public Camera playerCamera;
        public LayerMask hitLayers;

        private float nextFireTime = 0f;

        public override void Spawned()
        {
            starterInputs = GetComponent<StarterAssetsInputs>();
            if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || currentWeapon == null || currentWeapon.weaponData == null) return;

            if (isShooting)
            {
                if (Time.time >= nextFireTime)
                {
                    Fire();

                    nextFireTime = Time.time + currentWeapon.weaponData.fireRate;

                    if (!currentWeapon.weaponData.automatic) isShooting = false;
                }
            }
        }

        private void Fire()
        {
            currentWeapon.OnFireLocal();

            
            ProcessNetworkHit(currentWeapon.weaponData.damage, currentWeapon.weaponData.range, currentWeapon.weaponData.pelletsPerShot);
        }

        private void ProcessNetworkHit(float damage, float range, int pellets)
        {
            if (playerCamera == null) return;

           
            for (int i = 0; i < pellets; i++)
            {
                Vector3 rayDirection = playerCamera.transform.forward;
                if (currentWeapon.weaponData.spread > 0)
                {
                   
                    rayDirection += new Vector3(
                        Random.Range(-currentWeapon.weaponData.spread, currentWeapon.weaponData.spread),
                        Random.Range(-currentWeapon.weaponData.spread, currentWeapon.weaponData.spread),
                        0f
                    );
                }

                Ray ray = new Ray(playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), rayDirection);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, range, hitLayers))
                {
                    if (currentWeapon.weaponData.bulletImpactPrefab != null)
                    {
                        Instantiate(currentWeapon.weaponData.bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    SaludJugadorRed targetSalud = hit.collider.GetComponentInParent<SaludJugadorRed>();

                    if (targetSalud != null)
                    {
                        
                        if (targetSalud.Object == Object) continue;

                        int finalDamage = Mathf.RoundToInt(damage);
                        targetSalud.RPC_TomarDanio(finalDamage, transform.position);
                    }
                }
            }
        }

        public void MobileFireDown() { isShooting = true; }
        public void MobileFireUp() { isShooting = false; }
        public void SetCurrentWeapon(Weapon weapon) { currentWeapon = weapon; }
    }
}