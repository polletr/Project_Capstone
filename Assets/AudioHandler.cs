using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] EventReference audioClip;

    public void PlayOneShotAudio()
    {
        AudioManagerFMOD.Instance.PlayOneShot(audioClip, transform.position);
    }
}
