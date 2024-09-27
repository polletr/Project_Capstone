using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField]
    private GameObject exhibitionMenu;
    [SerializeField]
    private GameObject creditsMenu;
    [SerializeField]
    private GameObject controlsMenu;


    [Header("Mute button")]
    [SerializeField] private Image MuteButtonImage;
    [SerializeField] private Sprite MuteSprite;
    [SerializeField] private Sprite UnMuteSprite;
    
    [Header("Audio")]
    [SerializeField]
    private AudioClip menuClip;
    [SerializeField]
    private AudioClip buttonClip;

    [Header("Audio Souce")]
    [SerializeField, Tooltip("Audio Mixer form the Assets folder")]
    private AudioMixer _MasterAudioMixer;

    public GameEvent gameEvent;
    
   // private float _maxVolume = 1f;

    private void Start()
    {
        _startActive = true;
      // AudioManager.Instance.PlayAudio(menuClip,true);
    }

    public void OnPlay_01() 
    {
        SceneManager.LoadScene(1);
    } 

    public void playClickSound()
    {
        //AudioManager.Instance.PlayAudio(buttonClip);
    }

    public void OnOpenLeaderboard()
    {
      SceneManager.LoadScene(2);
    }

    public void OnToggleMute()
    {
        _isMusted = !_isMusted;
        MuteButtonImage.sprite = !_isMusted ? MuteSprite : UnMuteSprite;
       
       //AudioManager.Instance.MuteAudio(_isMusted, _maxVolume);
    }
}
