using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    private AudioSource unitAttackChannel;

    private AudioClip unitAttackClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        unitAttackChannel = gameObject.AddComponent<AudioSource>();
        unitAttackChannel.volume = 0.5f;
        unitAttackChannel.playOnAwake = false;
    }
    // this needs to be changed later to be more dynamic and not just one sound, and the add range of sound
    public void PlayUnitAttackSound()
    {
        if (unitAttackChannel.isPlaying == false)
        {
            unitAttackChannel.PlayOneShot(unitAttackClip);
        }
      
    }

}
