using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntityAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] stepSounds, attackSounds, hurtSounds, customSounds;

    public void PlayFootstepSound()
    {
        audioSource.clip = stepSounds[Random.Range(0,stepSounds.Length)];
        audioSource.Play();
    }

    public void PlayAttackSound()
    {
        audioSource.clip = attackSounds[Random.Range(0, attackSounds.Length)];
        audioSource.Play();
    }

    public void PlayHurtSound()
    {
        audioSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
        audioSource.Play();
    }

    public void PlayCustomSound(int id)
    {
        audioSource.clip = customSounds[id];
        audioSource.Play();
    }
}
