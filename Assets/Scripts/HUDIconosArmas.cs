using UnityEngine;
using UnityEngine.UI;

public class HUDIconosArmas : MonoBehaviour
{
    [Header("Referencias a tus Imágenes de la Pantalla")]
    public Image iconoPrincipal;
    public Image iconoSecundaria;
    public Image iconoCuchillo;
    public Image iconoGranada;
    public Image iconoBomba;

    [Header("Efecto Visual Armas Normales")]
    public Color colorEquipado = Color.red;
    public Color colorGuardado = new Color(1f, 1f, 1f, 0.4f);

    [Header("Alerta de Bomba")]
    public Color colorParpadeoPlantado = Color.red;
    public Color colorParpadeoDefuse = Color.blue;
    public float velocidadParpadeo = 6f;

    private PlayerInventory jugadorLocal;

    private Color colorOriginalBomba;
    private bool colorBombaGuardado = false;

    void Start()
    {
        if (iconoBomba != null)
        {
            colorOriginalBomba = iconoBomba.color;
            colorBombaGuardado = true;
        }
    }

    void Update()
    {
            
        if (jugadorLocal != null && (jugadorLocal.Object == null || !jugadorLocal.Object.IsValid))
        {
            jugadorLocal = null;
        }

        if (jugadorLocal == null)
        {
            BuscarJugadorLocal();
           
            if (jugadorLocal == null) return;
        }

        ActualizarSlot(iconoPrincipal, jugadorLocal.currentPrimary, jugadorLocal.activeSlot == WeaponSlot.Primary);
        ActualizarSlot(iconoSecundaria, jugadorLocal.currentSecondary, jugadorLocal.activeSlot == WeaponSlot.Secondary);
        ActualizarSlot(iconoCuchillo, jugadorLocal.currentKnife, jugadorLocal.activeSlot == WeaponSlot.Knife);

        GameObject granadaAMostrar = null;
        bool granadaEquipada = false;

        if (jugadorLocal.activeSlot == WeaponSlot.Explosiva) { granadaAMostrar = jugadorLocal.currentExplosiva; granadaEquipada = true; }
        else if (jugadorLocal.activeSlot == WeaponSlot.Flash) { granadaAMostrar = jugadorLocal.currentFlash; granadaEquipada = true; }
        else if (jugadorLocal.activeSlot == WeaponSlot.Humo) { granadaAMostrar = jugadorLocal.currentHumo; granadaEquipada = true; }
        else if (jugadorLocal.activeSlot == WeaponSlot.Fuego) { granadaAMostrar = jugadorLocal.currentFuego; granadaEquipada = true; }
        else
        {
            if (jugadorLocal.currentExplosiva != null) granadaAMostrar = jugadorLocal.currentExplosiva;
            else if (jugadorLocal.currentFlash != null) granadaAMostrar = jugadorLocal.currentFlash;
            else if (jugadorLocal.currentHumo != null) granadaAMostrar = jugadorLocal.currentHumo;
            else if (jugadorLocal.currentFuego != null) granadaAMostrar = jugadorLocal.currentFuego;
        }
        ActualizarSlot(iconoGranada, granadaAMostrar, granadaEquipada);

        if (iconoBomba != null)
        {
            if (!colorBombaGuardado)
            {
                colorOriginalBomba = iconoBomba.color;
                colorBombaGuardado = true;
            }

            ConfiguracionJugadorRed config = jugadorLocal.GetComponent<ConfiguracionJugadorRed>();
            bool esTerrorista = config != null && config.miEquipo == Team.Terrorist;
            bool esPolicia = config != null && config.miEquipo == Team.Police;
            bool tieneBombaEnInventario = (jugadorLocal.currentBomb != null);

            Color colorFinalBomba = colorOriginalBomba;

            if (esTerrorista && tieneBombaEnInventario && jugadorLocal.enZonaPlantar)
            {
                float oscilacion = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
                Color colorAlerta = colorParpadeoPlantado;
                colorAlerta.a = colorOriginalBomba.a;

                colorFinalBomba = Color.Lerp(colorOriginalBomba, colorAlerta, oscilacion);
            }
            else if (esPolicia && LogicaBombaPlantada.BombaActiva != null)
            {
                float distancia = Vector3.Distance(jugadorLocal.transform.position, LogicaBombaPlantada.BombaActiva.transform.position);

                if (distancia <= 3f)
                {
                    float oscilacion = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
                    Color colorAlerta = colorParpadeoDefuse;
                    colorAlerta.a = colorOriginalBomba.a;

                    colorFinalBomba = Color.Lerp(colorOriginalBomba, colorAlerta, oscilacion);
                }
            }

            iconoBomba.color = colorFinalBomba;
        }
    }

    private void BuscarJugadorLocal()
    {
        PlayerInventory[] jugadores = FindObjectsByType<PlayerInventory>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (PlayerInventory j in jugadores)
        {
            if (j.Object != null && j.Object.IsValid && j.HasStateAuthority)
            {
                jugadorLocal = j;
                break;
            }
        }
    }

    private void ActualizarSlot(Image imagenUI, GameObject armaObj, bool estaEquipada)
    {
        if (imagenUI == null) return;

        if (armaObj == null)
        {
            imagenUI.enabled = false;
            return;
        }

        Weapon scriptArma = armaObj.GetComponent<Weapon>();
        if (scriptArma != null && scriptArma.weaponData != null && scriptArma.weaponData.iconoArma != null)
        {
            imagenUI.sprite = scriptArma.weaponData.iconoArma;
            imagenUI.enabled = true;
            imagenUI.color = estaEquipada ? colorEquipado : colorGuardado;
            imagenUI.preserveAspect = true;
        }
        else
        {
            imagenUI.enabled = false;
        }
    }
}