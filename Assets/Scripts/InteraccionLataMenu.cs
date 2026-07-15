using UnityEngine;

public class InteraccionLataMenu : MonoBehaviour
{
    [Header("Las Dos Latas")]
    public GameObject lataMesa; // La de adorno
    public GameObject lataMano; // La que va a tirar

    [Header("Fuerza del Tiro")]
    public float fuerzaHaciaAdelante = 4f;
    public float fuerzaHaciaArriba = 2f;

    private Rigidbody rbLataMano;

    void Start()
    {
        if (lataMano != null)
        {
            rbLataMano = lataMano.GetComponent<Rigidbody>();
            if (rbLataMano != null) rbLataMano.isKinematic = true;

            // Al arrancar el menú, ocultamos la lata de la mano
            lataMano.SetActive(false);
        }

        if (lataMesa != null)
        {
            // Y nos aseguramos de que la de la mesa se vea
            lataMesa.SetActive(true);
        }
    }

    // El evento 1 de la animación llama acá
    public void EventoAgarrarLata()
    {
        if (lataMesa != null) lataMesa.SetActive(false); // Desaparece la ilusión de la mesa
        if (lataMano != null) lataMano.SetActive(true);  // Aparece la real en la mano
    }

    // El evento 2 de la animación llama acá
    public void EventoTirarLata()
    {
        if (lataMano == null || rbLataMano == null) return;

        // Desvinculamos la lata de la mano para que sea libre en el mundo
        lataMano.transform.SetParent(null);

        // Prendemos las físicas
        rbLataMano.isKinematic = false;

        // Le damos el empujón para revolearla
        Vector3 direccionTiro = (transform.forward * fuerzaHaciaAdelante) + (transform.up * fuerzaHaciaArriba);
        rbLataMano.AddForce(direccionTiro, ForceMode.Impulse);

        // Efecto de giro en el aire
        rbLataMano.AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));
    }
}