using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Play : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSources;
    

    public void PlayAudio(int audioSourceId)
    {
        audioSources[audioSourceId].Stop();
        audioSources[audioSourceId].Play();
    }

    public void StopAudio(int audioSourceId)
    {
        audioSources[audioSourceId].Stop();
    }
}
