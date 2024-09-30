using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;


public class BGMTrigger : MonoBehaviour
{
    [SerializeField] EventReference BGMusic;
    public UnityEvent SoundEvent;
    private EventInstance BGMusicInstance;

    private void Awake()
    {
        BGMusicInstance = AudioManagerFMOD.Instance.CreateBGInstance(BGMusic);
    }

    public void PlayBGMusic()
    {
        PLAYBACK_STATE playbackState;
        BGMusicInstance.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            BGMusicInstance.start();
    }

    public void StopBGM()
    {
        BGMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BGMusicInstance.release();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundEvent.Invoke();
            Destroy(this.gameObject);
        }
    }
}
