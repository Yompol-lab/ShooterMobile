using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [System.Serializable]
    public class Slot
    {
        public GameObject panel;
        public Image avatar;
        public TMP_Text nombre;
    }

    [Header("Lista de jugadores")]
    public GameObject listaJugadores;
    public Slot[] slots = new Slot[5];

    private Sprite[] avatares;

    private void Awake()
    {
        Instance = this;
        avatares = Resources.LoadAll<Sprite>("Avatares");

        if (listaJugadores != null)
            listaJugadores.SetActive(false);
    }

    private void Start()
    {
        InvokeRepeating(nameof(ActualizarLista), 0.5f, 0.5f);
    }

    public void MostrarLista()
    {
        if (listaJugadores != null)
            listaJugadores.SetActive(true);
    }

    public void OcultarLista()
    {
        if (listaJugadores != null)
            listaJugadores.SetActive(false);
    }

    void ActualizarLista()
    {
        if (listaJugadores == null || !listaJugadores.activeSelf)
            return;

        ConfiguracionJugadorRed[] jugadores =
            FindObjectsByType<ConfiguracionJugadorRed>(FindObjectsSortMode.None);

        ConfiguracionJugadorRed jugadorLocal = null;

        foreach (var j in jugadores)
        {
            if (j.HasStateAuthority)
            {
                jugadorLocal = j;
                break;
            }
        }

        if (jugadorLocal == null)
            return;

        List<ConfiguracionJugadorRed> mismoEquipo = new List<ConfiguracionJugadorRed>();

        foreach (var j in jugadores)
        {
            if (j.miEquipo == jugadorLocal.miEquipo)
            {
                mismoEquipo.Add(j);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < mismoEquipo.Count)
            {
                slots[i].panel.SetActive(true);

                slots[i].nombre.text = mismoEquipo[i].nombreJugador.ToString();

                int avatar = mismoEquipo[i].avatarID;

                if (avatar >= 0 && avatar < avatares.Length)
                    slots[i].avatar.sprite = avatares[avatar];
            }
            else
            {
                slots[i].panel.SetActive(false);
            }
        }
    }
}