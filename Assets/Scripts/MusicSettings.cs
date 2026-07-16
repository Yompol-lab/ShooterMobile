using UnityEngine;
using UnityEngine.UI;

public class MusicSettings : MonoBehaviour
{
    public Slider sliderMusica;

    [Tooltip("Tiene que llamarse EXACTAMENTE igual que en Wwise")]
    private string parametroWwise = "Volumen_Musica";

    void Start()
    {
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusicaGuardado", 100f);
        sliderMusica.value = volumenGuardado;
        AkSoundEngine.SetRTPCValue(parametroWwise, volumenGuardado);
        sliderMusica.onValueChanged.AddListener(CambiarVolumen);
    }

    public void CambiarVolumen(float nuevoValor)
    {
       
        AkSoundEngine.SetRTPCValue(parametroWwise, nuevoValor);

       
        PlayerPrefs.SetFloat("VolumenMusicaGuardado", nuevoValor);
        PlayerPrefs.Save();
    }
}