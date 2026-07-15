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

    [Header("Audio Wwise")]
    public AK.Wwise.Event pauseMenuMusic;
    public AK.Wwise.Event resumeMenuMusic;
    [Tooltip("Arrastrá acá los 6 eventos de Play en el mismo orden que tus fotos")]
    public AK.Wwise.Event[] cancionesAvatares;
    public AK.Wwise.Event stopCancionesAvatares;

    private Sprite[] avatares;
    private int avatarActual;

    void Start()
    {
        
        avatares = Resources.LoadAll<Sprite>("Avatares");

       
        avatarActual = PlayerPrefs.GetInt("AvatarID", 0);

        if (avatares.Length > 0)
            avatarImage.sprite = avatares[avatarActual];

       
        inputNombre.text = PlayerPrefs.GetString("NombreJugador", "");
    }

  
    public void AbrirPanelPerfil()
    {
        panelMenuPrincipal.SetActive(false);
        panelPerfil.SetActive(true);

       
        pauseMenuMusic.Post(gameObject);

       
        ReproducirCancionAvatar();
    }

    public void SiguienteAvatar()
    {
        if (avatares.Length == 0)
            return;

        avatarActual++;

        if (avatarActual >= avatares.Length)
            avatarActual = 0;

        avatarImage.sprite = avatares[avatarActual];
        ReproducirCancionAvatar(); 
    }

    public void AvatarAnterior()
    {
        if (avatares.Length == 0)
            return;

        avatarActual--;

        if (avatarActual < 0)
            avatarActual = avatares.Length - 1;

        avatarImage.sprite = avatares[avatarActual];
        ReproducirCancionAvatar(); 
    }

    private void ReproducirCancionAvatar()
    {
        
        stopCancionesAvatares.Post(gameObject);

       
        if (avatarActual < cancionesAvatares.Length)
        {
            cancionesAvatares[avatarActual].Post(gameObject);
        }
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
        
        stopCancionesAvatares.Post(gameObject);

        yield return new WaitForSeconds(0.3f);

        panelPerfil.SetActive(false);
        panelMenuPrincipal.SetActive(true);

        
        resumeMenuMusic.Post(gameObject);
    }

    public string ObtenerNombre()
    {
        return PlayerPrefs.GetString("NombreJugador");
    }

    public int ObtenerAvatar()
    {
        return avatarActual;
    }

    
    public void CerrarSinGuardar()
    {
       
        stopCancionesAvatares.Post(gameObject);

        
        avatarActual = PlayerPrefs.GetInt("AvatarID", 0);
        if (avatares.Length > 0)
            avatarImage.sprite = avatares[avatarActual];

        
        inputNombre.text = PlayerPrefs.GetString("NombreJugador", "");

       
        panelPerfil.SetActive(false);
        panelMenuPrincipal.SetActive(true);

       
        resumeMenuMusic.Post(gameObject);
    }

}