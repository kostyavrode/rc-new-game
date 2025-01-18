using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioHelper : MonoBehaviour
{
    public static GameAudioHelper instance;


    public SettingsController settingsController;
    public bool isSound;

    public AudioSource moveSource;

    private void Awake()
    {
        instance = this;
    }

    public void PlayMoveSound()
    {
        isSound = settingsController.isVibrateEnabled;
        if (isSound)
        {
            moveSource.Play();
        }
    }
}
