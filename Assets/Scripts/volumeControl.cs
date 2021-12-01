using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeControl : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource AudioSource;

    public Slider ourSlider;
    static float sliderVol;

    static bool initialized = false;

    void Start()
    {
        AudioSource.Play();
        if (!initialized)
        {
            mainMenuVol();
            initialized = true;
        }

        ourSlider.value = sliderVol;
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume = sliderVol;
    }

    public void updateVolume(float volume)
    {
        sliderVol = volume;
    }

    public void mainMenuVol()
    {
        sliderVol = 1f;
    }
}
