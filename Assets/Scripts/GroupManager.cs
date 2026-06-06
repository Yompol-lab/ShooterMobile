using TMPro;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public TextMeshProUGUI txtCodigoGrupo;
    public TextMeshProUGUI txtMiembros;

    public void CrearGrupo()
    {
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string codigo = "";

        for (int i = 0; i < 4; i++)
        {
            codigo += caracteres[Random.Range(0, caracteres.Length)];
        }

        txtCodigoGrupo.text = "Código: " + codigo;

        string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Jugador");

        txtMiembros.text =
            "Miembros:\n" +
            "- " + nombreJugador;
    }
}