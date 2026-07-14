using Fusion;
using UnityEngine;

public class ControladorMira : NetworkBehaviour
{
    [Header("Configuración de Zoom")]
    public float fovNormal = 60f;
    public float fovZoom = 15f;
    public float velocidadZoom = 15f;

    private Camera camaraJugador;
    private GameObject imagenMiraUI;
    private Transform weaponContainer;
    private bool estaApuntando = false;
    private PlayerInventory inventario;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        inventario = GetComponent<PlayerInventory>();
        if (inventario != null) weaponContainer = inventario.weaponContainer;

        camaraJugador = GetComponentInChildren<Camera>();
        if (camaraJugador != null) fovNormal = camaraJugador.fieldOfView;

        Canvas[] todosLosCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas canvas in todosLosCanvases)
        {
            Transform miraTrans = canvas.transform.Find("MiraSniperUI");
            if (miraTrans != null)
            {
                imagenMiraUI = miraTrans.gameObject;
                imagenMiraUI.SetActive(false); 
                break; 
            }
        }
    }

    private void Update()
    {
        if (!HasStateAuthority || camaraJugador == null) return;

        float fovObjetivo = estaApuntando ? fovZoom : fovNormal;
        camaraJugador.fieldOfView = Mathf.Lerp(camaraJugador.fieldOfView, fovObjetivo, Time.deltaTime * velocidadZoom);
    }

    public void AlternarMira()
    {
        if (!HasStateAuthority) return;

        if (inventario == null) return;

        
        if (inventario.activeSlot != WeaponSlot.Primary)
        {
            Debug.Log(" No podés apuntar con la pistola o el cuchillo.");
            return;
        }

        if (imagenMiraUI == null)
        {
            Debug.LogError(" ERROR: Revisé TODOS los Canvases y no encontré 'MiraSniperUI'.");
            return;
        }

        estaApuntando = !estaApuntando;

       
        imagenMiraUI.SetActive(estaApuntando);

       
        if (weaponContainer != null)
        {
            weaponContainer.localScale = estaApuntando ? Vector3.zero : Vector3.one;
        }
    }

    public void CancelarMira()
    {
        if (!estaApuntando) return;
        estaApuntando = false;

        if (imagenMiraUI != null) imagenMiraUI.SetActive(false);
        if (weaponContainer != null) weaponContainer.localScale = Vector3.one;
        if (camaraJugador != null) camaraJugador.fieldOfView = fovNormal;
    }
}