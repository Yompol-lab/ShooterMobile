using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public Image imagenTutorial;
    public TMP_Text tituloTutorial;
    public TMP_Text descripcionTutorial;

    [Header("Botones")]
    public Button btnAnterior;
    public Button btnSiguiente;
    public Button btnCerrar;
    public Button btnJugar;

    [Header("Contenido")]
    public Sprite[] imagenes;

    public string[] titulos;

    [TextArea(3, 10)]
    public string[] descripciones;

    private int pagina = 0;

    void Start()
    {
        btnAnterior.onClick.AddListener(PaginaAnterior);
        btnSiguiente.onClick.AddListener(PaginaSiguiente);
        btnCerrar.onClick.AddListener(CerrarTutorial);

        MostrarPagina();
    }

    void MostrarPagina()
    {
        imagenTutorial.sprite = imagenes[pagina];
        tituloTutorial.text = titulos[pagina];
        descripcionTutorial.text = descripciones[pagina];

        btnAnterior.gameObject.SetActive(pagina > 0);

        bool ultimaPagina = pagina == imagenes.Length - 1;

        btnSiguiente.gameObject.SetActive(!ultimaPagina);
        btnJugar.gameObject.SetActive(ultimaPagina);
    }

    public void PaginaSiguiente()
    {
        if (pagina < imagenes.Length - 1)
        {
            pagina++;
            MostrarPagina();
        }
    }

    public void PaginaAnterior()
    {
        if (pagina > 0)
        {
            pagina--;
            MostrarPagina();
        }
    }

    public void CerrarTutorial()
    {
        gameObject.SetActive(false);
    }
}