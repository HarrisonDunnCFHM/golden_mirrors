using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    //config parameters
    [SerializeField] AudioClip bgMusic;
    [SerializeField] float effectsVolume;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioSource myAudioSource;

    private void Awake()
    {
        int numberOfManagers = FindObjectsOfType<AudioManager>().Length;
        if (numberOfManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        effectsVolume = effectsSlider.value;
        myAudioSource.volume = musicSlider.value;
    }

    public float GetFXVolume()
    {
        return effectsVolume;
    }

}
