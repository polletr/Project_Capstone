using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;

    [SerializeField] private AudioClip pauseClip;
    [SerializeField] private AudioClip buttonClickClip;

    private bool _isPaused;
    private float _initialTimeScale;
    private void Awake()
    {
        _isPaused = false;
       _startActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnButtonClick()
    {
        //AudioManager.Instance.PlayAudio(buttonClickClip);
    }
   
    public void OnTogglePauseMenu()
    {
        _isPaused = !_isPaused;
        _currentMenu.gameObject.SetActive(_isPaused); 
        

        //AudioManager.Instance.PlayAudio(pauseClip);

        Cursor.lockState = _isPaused? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;

        if (_isPaused)
        {
            _initialTimeScale = Time.timeScale;
            //AudioManager.Instance.PauseBGAudio();
            Time.timeScale = 0; 
            OnPause.Invoke(); 
        }
        else
        {
            Time.timeScale = _initialTimeScale;
            //AudioManager.Instance.ResumeBGAudio();
            OnResume.Invoke(); 
        }

    }


    public void OnLoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
