using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image imgAvatar;
    public TMP_Text txtNombre;

    private Sprite[] avatares;

    private void Awake()
    {
        avatares = Resources.LoadAll<Sprite>("Avatares");
    }

    public void Inicializar(string nombre, int avatar)
    {
        txtNombre.text = nombre;

        if (avatar >= 0 && avatar < avatares.Length)
        {
            imgAvatar.sprite = avatares[avatar];
        }
    }
}