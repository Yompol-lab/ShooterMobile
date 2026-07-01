using TMPro;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    // Variables que podremos usar desde otros scripts
    public static string CodigoGrupo = "";
    public static bool SoyLider = false;
    public static bool EnGrupo = false;
    // Referencias de la UI
    public TextMeshProUGUI txtCodigoGrupo;
    public TextMeshProUGUI txtMiembros;
    public TMP_InputField inputCodigo;

    // Crear un grupo
    public void CrearGrupo()
    {
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string codigo = "";

        for (int i = 0; i < 4; i++)
        {
            codigo += caracteres[Random.Range(0, caracteres.Length)];
        }

        // Guardar datos del grupo
        CodigoGrupo = codigo;
        SoyLider = true;
        EnGrupo = true;

        txtCodigoGrupo.text = "Código: " + codigo;

        string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Jugador");

        txtMiembros.text =
            "Miembros:\n" +
            "- " + nombreJugador;

        Debug.Log("Grupo creado. Código: " + CodigoGrupo);
    }

    // Unirse a un grupo
    public void UnirseGrupo()
    {
        string codigo = inputCodigo.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(codigo))
        {
            Debug.Log("No ingresó ningún código.");
            return;
        }

        // Guardar datos del grupo
        CodigoGrupo = codigo;
        SoyLider = false;
        EnGrupo = true;

        string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Jugador");

        txtCodigoGrupo.text = "Código: " + codigo;

        txtMiembros.text =
            "Miembros:\n" +
            "- " + nombreJugador;

        Debug.Log("Intentando unirse al grupo: " + CodigoGrupo);
    }
}