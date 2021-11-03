using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volumeControl : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource AudioSource;
    private float sliderVol = 1f;

    void Start()
    {
        AudioSource.Play();
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
}
