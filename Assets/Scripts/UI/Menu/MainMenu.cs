using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    public GameEvent gameEvent;
    public PlayerSettings playerSettings;

    [Header("Audio"), SerializeField] private Image MuteButtonImage;
    
    [Header("Sensitivity"),SerializeField]
    private Slider sensitivitySlider;
    

    private FMOD.Studio.Bus masterBus;

    private void Start()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/"); //Get the master bus later
        _startActive = true;
        MuteButtonImage.enabled = _isMusted = PlayerPrefs.GetInt("Muted") == 1;
        playerSettings.cameraSensitivityMouse =  PlayerPrefs.GetFloat("MouseSensitivity", 25);
        sensitivitySlider.value = playerSettings.cameraSensitivityMouse;
        SetSensitivity();
        masterBus.setMute(_isMusted);
    }

    public void OnPlay()
    {
        Cursor.lockState = CursorLockMode.Locked; //Move this from here later
        Cursor.visible = false; //Move this from here later
        SceneManager.LoadScene("GameScene");
    }

    public void PlayClickSound()
    {
        //play click sound
    }

    public void OnGiveFeedback()
    {
        Application.OpenURL("https://forms.gle/9ptFvdmDXz5eSe5y9");
    }

    public void OnToggleMute()
    {
        MuteButtonImage.enabled = _isMusted = !_isMusted;
        PlayerPrefs.SetInt("Muted", _isMusted ? 1 : 0);
        masterBus.setMute(_isMusted);
    }

    public void SetSensitivity()
    {
        playerSettings.cameraSensitivityMouse = sensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
    }
}