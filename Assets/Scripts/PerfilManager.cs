using TMPro;
using UnityEngine;

public class PerfilManager : MonoBehaviour
{
    public TMP_InputField inputNombre;
    public TMP_Text txtNombre;

    void Start()
    {
        if (PlayerPrefs.HasKey("NombreJugador"))
        {
            txtNombre.text = PlayerPrefs.GetString("NombreJugador");
        }
    }

    public void GuardarNombre()
    {
        string nombre = inputNombre.text;

        if (nombre != "")
        {
            PlayerPrefs.SetString("NombreJugador", nombre);
            PlayerPrefs.Save();

            txtNombre.text = nombre;
        }
    }
}