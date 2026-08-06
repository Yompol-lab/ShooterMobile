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

        public void ResetShooting()
        {
            isShooting = false;
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            PlayerInventory inv = GetComponent<PlayerInventory>();
            WeaponSlot slotActivo = inv != null ? inv.activeSlot : WeaponSlot.Primary;

            if (isShooting)
            {
                if (slotActivo == WeaponSlot.Fuego || slotActivo == WeaponSlot.Humo ||
                    slotActivo == WeaponSlot.Flash || slotActivo == WeaponSlot.Explosiva)
                {
                    if (Time.time >= nextFireTime)
                    {
                        ControladorGranadasRed controlGranadas = GetComponent<ControladorGranadasRed>();
                        if (controlGranadas != null)
                        {
                            TipoGranada tipo = TipoGranada.Fuego;
                            if (slotActivo == WeaponSlot.Humo) tipo = TipoGranada.Humo;
                            else if (slotActivo == WeaponSlot.Flash) tipo = TipoGranada.Flash;
                            else if (slotActivo == WeaponSlot.Explosiva) tipo = TipoGranada.Explosiva;

                            bool puedeTirar = false;
                            if (tipo == TipoGranada.Fuego && controlGranadas.granadasFuego > 0) puedeTirar = true;
                            else if (tipo == TipoGranada.Humo && controlGranadas.granadasHumo > 0) puedeTirar = true;
                            else if (tipo == TipoGranada.Flash && controlGranadas.granadasFlash > 0) puedeTirar = true;
                            else if (tipo == TipoGranada.Explosiva && controlGranadas.granadasExplosivas > 0) puedeTirar = true;

                            if (puedeTirar)
                            {
                                controlGranadas.IntentarLanzarGranada(tipo);
                                inv.ConsumirGranadaMano(slotActivo);

                                if (inv.currentPrimary != null) inv.EquipSlot(WeaponSlot.Primary);
                                else inv.EquipSlot(WeaponSlot.Knife);

                                nextFireTime = Time.time + 1f;
                            }
                            else
                            {
                                if (inv.currentPrimary != null) inv.EquipSlot(WeaponSlot.Primary);
                                else inv.EquipSlot(WeaponSlot.Knife);
                            }
                        }
                        isShooting = false;
                    }
                    return;
                }

                if (currentWeapon == null || currentWeapon.weaponData == null) return;

                if (Time.time >= nextFireTime)
                {
                    GameObject armaActiva = inv != null ? inv.GetActiveWeaponObject() : null;
                    bool tieneBalas = true;

                    if (armaActiva != null)
                    {
                        MunicionArma mun = armaActiva.GetComponent<MunicionArma>();
                        if (mun != null) tieneBalas = mun.IntentarDisparar();
                    }

                    if (tieneBalas)
                    {
                        Fire();
                        nextFireTime = Time.time + currentWeapon.weaponData.fireRate;
                        if (!currentWeapon.weaponData.automatic) isShooting = false;
                    }
                    else
                    {
                        if (!currentWeapon.weaponData.automatic) isShooting = false;
                    }
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

                RaycastHit[] hits = Physics.RaycastAll(ray, range, hitLayers);
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                bool impactoValidoEncontrado = false;
                RaycastHit impactoFinal = default;

                foreach (var hit in hits)
                {
                    SaludJugadorRed targetSalud = hit.collider.GetComponentInParent<SaludJugadorRed>();
                    if (targetSalud != null && targetSalud.Object == Object)
                    {
                        continue;
                    }

                    impactoFinal = hit;
                    impactoValidoEncontrado = true;
                    break;
                }

                if (impactoValidoEncontrado)
                {
                    if (currentWeapon.weaponData.bulletImpactPrefab != null)
                    {
                        Instantiate(currentWeapon.weaponData.bulletImpactPrefab, impactoFinal.point, Quaternion.LookRotation(impactoFinal.normal));
                    }

                    SaludJugadorRed targetSalud = impactoFinal.collider.GetComponentInParent<SaludJugadorRed>();
                    if (targetSalud != null)
                    {
                        int finalDamage = Mathf.RoundToInt(damage);
                        targetSalud.RPC_TomarDanio(finalDamage, transform.position);
                    }

                    DummyEntrenamiento dummy = impactoFinal.collider.GetComponentInParent<DummyEntrenamiento>();
                    if (dummy != null)
                    {
                        int finalDamage = Mathf.RoundToInt(damage);
                        dummy.RPC_TomarDanio(finalDamage, transform.position);
                    }

                    FireExtinguisher extintor = impactoFinal.collider.GetComponentInParent<FireExtinguisher>();
                    if (extintor != null)
                    {
                        extintor.TriggerSmoke();
                    }
                }
            }
        }

        public void MobileFireDown() { isShooting = true; }
        public void MobileFireUp() { isShooting = false; }
        public void SetCurrentWeapon(Weapon weapon) { currentWeapon = weapon; }
    }
}