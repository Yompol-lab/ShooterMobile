using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class AlertaBotonRecarga : MonoBehaviour
{
    [Header("Componente Visual")]
    [Tooltip("El Image del botón de recarga. Si lo dejás vacío, el script lo busca solo.")]
    public Image imagenBoton;

    [Header("Configuración de Colores")]
    public Color colorNormal = Color.white;
    public Color colorAlerta = Color.red;

    [Header("Velocidad de Titileo")]
    public float velocidadAnimacion = 4f;

    private void Start()
    {
        if (imagenBoton == null)
        {
            imagenBoton = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (NetworkRunner.Instances.Count == 0) return;
        NetworkRunner runner = NetworkRunner.Instances[0];
        if (runner == null || !runner.IsRunning) return;

        NetworkObject jugadorLocalObj = runner.GetPlayerObject(runner.LocalPlayer);
        if (jugadorLocalObj == null)
        {
            imagenBoton.color = colorNormal;
            return;
        }

        PlayerInventory inv = jugadorLocalObj.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            GameObject arma = inv.GetActiveWeaponObject();
            if (arma != null)
            {
                MunicionArma municion = arma.GetComponent<MunicionArma>();
                if (municion != null)
                {
                   
                    if (municion.balasCargador <= 0)
                    {
                        float t = Mathf.PingPong(Time.time * velocidadAnimacion, 1f);
                        imagenBoton.color = Color.Lerp(colorNormal, colorAlerta, t);
                        return;
                    }
                }
            }
        }

      
        imagenBoton.color = colorNormal;
    }
}