using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Referencias de Disparo")]
    [Tooltip("La cámara principal (desde donde sale la bala central)")]
    public Camera mainCamera;

    [Header("Configuración del Arma Activa (Se llena por script)")]
    [Tooltip("El punto en la punta del cañón del arma actual")]
    public Transform currentMuzzlePoint;

    [Header("Assets Visuales (Prefabs)")]
    [Tooltip("El prefab del fogonazo")]
    public GameObject muzzleFlashPrefab;
    [Tooltip("El prefab del agujero de la pared")]
    public GameObject bulletHolePrefab;

    [Header("Configuración Física")]
    [Tooltip("A qué distancia llega la bala")]
    public float range = 100f;
    [Tooltip("Qué capas de físicas (Layers) pueden ser impactadas (ej: Paredes, Enemigos)")]
    public LayerMask impactLayers;

    
    private PlayerInventory inventory;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
           
            if (currentMuzzlePoint != null)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        
        if (muzzleFlashPrefab != null && currentMuzzlePoint != null)
        {
            
            Instantiate(muzzleFlashPrefab, currentMuzzlePoint.position, currentMuzzlePoint.rotation);
        }

        
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;

        
        if (Physics.Raycast(ray, out hit, range, impactLayers))
        {
            Debug.Log("Impacto en: " + hit.collider.name);

            
            if (bulletHolePrefab != null)
            {
                
                GameObject hole = Instantiate(bulletHolePrefab, hit.point, Quaternion.identity);

               
                hole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

               
                hole.transform.position += hit.normal * 0.001f;
            }
        }
    }
}