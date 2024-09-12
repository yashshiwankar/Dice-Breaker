using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundType
{
    WALL_COLLIDE,
    BLOCK_COLLIDE,
    BLOCK_DESTROY,
    BUTTON_CLICK,
    GAME_OVER
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private AudioSource audioSource;

    [SerializeField]
    private Sound[] audioClipList;


    void Awake()
    {
        if(instance == null)
            instance = this;


        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound)
    {
        Sound audioclip = GetAudio(sound);
        instance.audioSource.PlayOneShot(audioclip.audioClip, audioclip.volume);
        instance.audioSource.clip = null;
        print($"Sound Played: {audioclip.name.ToString()}");
    }

    public static Sound GetAudio(SoundType sound)
    {
        try
        {
            foreach (Sound audioClip in instance.audioClipList)
            {
                if (audioClip.name == sound)
                {
                    return audioClip;
                }
            }
            // If no matching Sound is found, throw an exception
            throw new NullReferenceException("Sound not found: " + sound.ToString());
        }

        catch (NullReferenceException err)
        {
            Debug.LogError(err);
            return null;
        }
    }
}
