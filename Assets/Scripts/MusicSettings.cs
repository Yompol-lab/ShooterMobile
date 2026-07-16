using UnityEngine;
using UnityEngine.UI;

public class MusicSettings : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        float volumen = PlayerPrefs.GetFloat("MusicVolume", 100f);

        slider.value = volumen;
        AkSoundEngine.SetRTPCValue("MusicVolume", volumen);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float valor)
    {
        AkSoundEngine.SetRTPCValue("MusicVolume", valor);

        PlayerPrefs.SetFloat("MusicVolume", valor);
        PlayerPrefs.Save();
    }
}