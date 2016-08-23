using UnityEngine;
using System.Collections;

public class soundManagerScript : MonoBehaviour {
    AudioSource source;
    public UnityEngine.UI.Slider slider;
    float volume;
    
    void Start()
    {   
        source = gameObject.GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat("volume", 1.0f);
        source.volume = volume;
        slider.value = volume;
        DontDestroyOnLoad(gameObject);
    }

    public void playAudio()
    {
        source.Play();
    }

    public void setVolume()
    {
        volume = slider.value;
        PlayerPrefs.SetFloat("volume", volume);
        source.volume = volume;
    }
}
