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
