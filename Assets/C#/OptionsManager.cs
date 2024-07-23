using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioManager;
    [SerializeField] Slider[] soundSlider;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MasterSound"))
        {
            PlayerPrefs.SetFloat("MasterSound", 0.75f);
            PlayerPrefs.SetFloat("BackgroundSound", 0.75f);
            PlayerPrefs.SetFloat("EffectSound", 0.75f);
        }

        soundSlider[0].value = PlayerPrefs.GetFloat("MasterSound") * 100;
        soundSlider[1].value = PlayerPrefs.GetFloat("BackgroundSound") * 100;
        soundSlider[2].value = PlayerPrefs.GetFloat("EffectSound") * 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager.SetFloat("MasterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterSound")) * 20);
        audioManager.SetFloat("BackgroundVol", Mathf.Log10(PlayerPrefs.GetFloat("BackgroundSound")) * 20);
        audioManager.SetFloat("EffectVol", Mathf.Log10(PlayerPrefs.GetFloat("EffectSound")) * 20);
        if (PlayerPrefs.GetFloat("MasterSound") == 0)
            audioManager.SetFloat("MasterSound", Mathf.Log10(0.001f) * 20);
        if (PlayerPrefs.GetFloat("BackgroundSound") == 0)
            audioManager.SetFloat("BackgroundVol", Mathf.Log10(0.001f) * 20);
        if (PlayerPrefs.GetFloat("EffectSound") == 0)
            audioManager.SetFloat("EffectVol", Mathf.Log10(0.001f) * 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MasterVolume(float val)
    {
        PlayerPrefs.SetFloat("MasterSound", val / 100f);
        audioManager.SetFloat("MasterVol", Mathf.Log10(val / 100f) * 20);
        if (val==0)
            audioManager.SetFloat("MasterVol", Mathf.Log10(0.001f) * 20);
    }

    public void BackgroundVolume(float val)
    {
        PlayerPrefs.SetFloat("BackgroundSound", val / 100f);
        audioManager.SetFloat("BackgroundVol", Mathf.Log10(val / 100f) * 20);
        if (val == 0)
            audioManager.SetFloat("BackgroundVol", Mathf.Log10(0.001f) * 20);
    }

    public void EffectVolume(float val)
    {
        PlayerPrefs.SetFloat("EffectSound", val / 100f);
        audioManager.SetFloat("EffectVol", Mathf.Log10(val / 100f) * 20);
        if (val == 0)
            audioManager.SetFloat("EffectVol", Mathf.Log10(0.001f) * 20);
    }
}
