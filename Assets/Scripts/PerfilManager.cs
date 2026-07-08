using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfilManager : MonoBehaviour
{
    [Header("Nombre")]
    public TMP_InputField inputNombre;

    [Header("Avatar")]
    public Image avatarImage;

    [Header("Paneles")]
    public GameObject panelPerfil;
    public GameObject panelMenuPrincipal;

    private Sprite[] avatares;
    private int avatarActual;

    void Start()
    {
        // Cargar avatares
        avatares = Resources.LoadAll<Sprite>("Avatares");

        // Cargar avatar guardado
        avatarActual = PlayerPrefs.GetInt("AvatarID", 0);

        if (avatares.Length > 0)
            avatarImage.sprite = avatares[avatarActual];

        // Cargar nombre guardado
        inputNombre.text = PlayerPrefs.GetString("NombreJugador", "");
    }

    public void SiguienteAvatar()
    {
        if (avatares.Length == 0)
            return;

        avatarActual++;

        if (avatarActual >= avatares.Length)
            avatarActual = 0;

        avatarImage.sprite = avatares[avatarActual];
    }

    public void AvatarAnterior()
    {
        if (avatares.Length == 0)
            return;

        avatarActual--;

        if (avatarActual < 0)
            avatarActual = avatares.Length - 1;

        avatarImage.sprite = avatares[avatarActual];
    }

    public void GuardarPerfil()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Escribí un nombre.");
            return;
        }

        PlayerPrefs.SetString("NombreJugador", nombre);
        PlayerPrefs.SetInt("AvatarID", avatarActual);
        PlayerPrefs.Save();

        Debug.Log("Perfil guardado correctamente.");

        StartCoroutine(CerrarPerfil());
    }

    IEnumerator CerrarPerfil()
    {
        yield return new WaitForSeconds(0.3f);

        panelPerfil.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public string ObtenerNombre()
    {
        return PlayerPrefs.GetString("NombreJugador");
    }

    public int ObtenerAvatar()
    {
        return avatarActual;
    }
}